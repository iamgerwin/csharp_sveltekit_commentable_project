import { apiClient } from './client';
import type {
	LoginRequest,
	RegisterRequest,
	AuthResponse,
	RefreshTokenRequest,
	User
} from '$lib/types/auth.types';

export const authApi = {
	/**
	 * Register a new user
	 */
	async register(data: RegisterRequest): Promise<AuthResponse> {
		const response = await apiClient.post<AuthResponse>('/auth/register', data);
		apiClient.saveTokens(response.token, response.refreshToken);
		return response;
	},

	/**
	 * Login with email and password
	 */
	async login(data: LoginRequest): Promise<AuthResponse> {
		const response = await apiClient.post<AuthResponse>('/auth/login', data);
		apiClient.saveTokens(response.token, response.refreshToken);
		return response;
	},

	/**
	 * Logout current user
	 */
	async logout(): Promise<void> {
		try {
			await apiClient.post<void>('/auth/logout');
		} finally {
			apiClient.logout();
		}
	},

	/**
	 * Refresh access token
	 */
	async refreshToken(data: RefreshTokenRequest): Promise<AuthResponse> {
		const response = await apiClient.post<AuthResponse>('/auth/refresh', data);
		apiClient.saveTokens(response.token, response.refreshToken);
		return response;
	},

	/**
	 * Get current authenticated user
	 */
	async getCurrentUser(): Promise<User> {
		return apiClient.get<User>('/auth/me');
	}
};
