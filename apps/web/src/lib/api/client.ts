import { goto } from '$app/navigation';
import { browser } from '$app/environment';

export class ApiError extends Error {
	constructor(
		public status: number,
		public message: string,
		public errors?: Record<string, string[]>
	) {
		super(message);
		this.name = 'ApiError';
	}
}

export interface ApiClientConfig {
	baseURL: string;
	timeout?: number;
	headers?: Record<string, string>;
}

class ApiClient {
	private baseURL: string;
	private timeout: number;
	private defaultHeaders: Record<string, string>;
	private tokenRefreshPromise: Promise<string> | null = null;

	constructor(config: ApiClientConfig) {
		this.baseURL = config.baseURL;
		this.timeout = config.timeout || 30000;
		this.defaultHeaders = config.headers || {};
	}

	private getToken(): string | null {
		if (!browser) return null;
		return localStorage.getItem('auth_token');
	}

	private getRefreshToken(): string | null {
		if (!browser) return null;
		return localStorage.getItem('refresh_token');
	}

	private setTokens(token: string, refreshToken: string): void {
		if (!browser) return;
		localStorage.setItem('auth_token', token);
		localStorage.setItem('refresh_token', refreshToken);
	}

	private clearTokens(): void {
		if (!browser) return;
		localStorage.removeItem('auth_token');
		localStorage.removeItem('refresh_token');
		localStorage.removeItem('user');
	}

	private async refreshAccessToken(): Promise<string> {
		// Prevent multiple simultaneous refresh requests
		if (this.tokenRefreshPromise) {
			return this.tokenRefreshPromise;
		}

		this.tokenRefreshPromise = (async () => {
			try {
				const refreshToken = this.getRefreshToken();
				if (!refreshToken) {
					throw new Error('No refresh token available');
				}

				const response = await fetch(`${this.baseURL}/auth/refresh`, {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ refreshToken })
				});

				if (!response.ok) {
					throw new Error('Token refresh failed');
				}

				const data = await response.json();
				this.setTokens(data.token, data.refreshToken);
				return data.token;
			} catch (error) {
				this.clearTokens();
				if (browser) {
					goto('/login?session=expired');
				}
				throw error;
			} finally {
				this.tokenRefreshPromise = null;
			}
		})();

		return this.tokenRefreshPromise;
	}

	private async request<T>(
		endpoint: string,
		options: RequestInit = {},
		retry = true
	): Promise<T> {
		const url = `${this.baseURL}${endpoint}`;
		const token = this.getToken();

		const headers: Record<string, string> = {
			...this.defaultHeaders,
			...(options.headers as Record<string, string>)
		};

		// Add auth token if available
		if (token) {
			headers['Authorization'] = `Bearer ${token}`;
		}

		// Add content-type for non-GET requests if not already set
		if (options.method && options.method !== 'GET' && !headers['Content-Type']) {
			headers['Content-Type'] = 'application/json';
		}

		const controller = new AbortController();
		const timeoutId = setTimeout(() => controller.abort(), this.timeout);

		try {
			const response = await fetch(url, {
				...options,
				headers,
				signal: controller.signal
			});

			clearTimeout(timeoutId);

			// Handle 401 Unauthorized - attempt token refresh
			if (response.status === 401 && retry) {
				try {
					const newToken = await this.refreshAccessToken();
					// Retry the request with new token
					return this.request<T>(endpoint, options, false);
				} catch (refreshError) {
					throw new ApiError(401, 'Authentication failed. Please login again.');
				}
			}

			// Handle other error responses
			if (!response.ok) {
				const contentType = response.headers.get('content-type');
				let errorData: any;

				if (contentType && contentType.includes('application/json')) {
					errorData = await response.json();
				} else {
					errorData = { message: await response.text() };
				}

				throw new ApiError(
					response.status,
					errorData.message || errorData.title || 'Request failed',
					errorData.errors
				);
			}

			// Handle 204 No Content
			if (response.status === 204) {
				return undefined as T;
			}

			// Parse JSON response
			const contentType = response.headers.get('content-type');
			if (contentType && contentType.includes('application/json')) {
				// Check if response has content before parsing
				const text = await response.text();
				if (text.trim().length === 0) {
					return undefined as T;
				}
				try {
					return JSON.parse(text);
				} catch (jsonError) {
					throw new ApiError(
						response.status,
						'Invalid JSON response from server'
					);
				}
			}

			return undefined as T;
		} catch (error) {
			clearTimeout(timeoutId);

			if (error instanceof ApiError) {
				throw error;
			}

			if (error instanceof Error) {
				if (error.name === 'AbortError') {
					throw new ApiError(408, 'Request timeout');
				}
				throw new ApiError(0, error.message);
			}

			throw new ApiError(0, 'An unexpected error occurred');
		}
	}

	async get<T>(endpoint: string, params?: Record<string, any>): Promise<T> {
		const queryString = params ? this.buildQueryString(params) : '';
		const url = queryString ? `${endpoint}?${queryString}` : endpoint;
		return this.request<T>(url, { method: 'GET' });
	}

	async post<T>(endpoint: string, data?: any): Promise<T> {
		return this.request<T>(endpoint, {
			method: 'POST',
			body: data ? JSON.stringify(data) : undefined
		});
	}

	async put<T>(endpoint: string, data?: any): Promise<T> {
		return this.request<T>(endpoint, {
			method: 'PUT',
			body: data ? JSON.stringify(data) : undefined
		});
	}

	async patch<T>(endpoint: string, data?: any): Promise<T> {
		return this.request<T>(endpoint, {
			method: 'PATCH',
			body: data ? JSON.stringify(data) : undefined
		});
	}

	async delete<T>(endpoint: string): Promise<T> {
		return this.request<T>(endpoint, { method: 'DELETE' });
	}

	private buildQueryString(params: Record<string, any>): string {
		const searchParams = new URLSearchParams();

		Object.entries(params).forEach(([key, value]) => {
			if (value !== undefined && value !== null) {
				if (Array.isArray(value)) {
					value.forEach((v) => searchParams.append(key, String(v)));
				} else {
					searchParams.append(key, String(value));
				}
			}
		});

		return searchParams.toString();
	}

	// Public method to manually set tokens (for login)
	public saveTokens(token: string, refreshToken: string): void {
		this.setTokens(token, refreshToken);
	}

	// Public method to logout
	public logout(): void {
		this.clearTokens();
	}
}

// Create singleton instance
export const apiClient = new ApiClient({
	baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5043/api'
});
