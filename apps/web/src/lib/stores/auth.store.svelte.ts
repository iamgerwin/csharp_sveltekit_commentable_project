import { browser } from '$app/environment';
import { goto } from '$app/navigation';
import { authApi } from '$lib/api/auth.api';
import type { User, LoginRequest, RegisterRequest } from '$lib/types/auth.types';

interface AuthState {
	user: User | null;
	isAuthenticated: boolean;
	isLoading: boolean;
	error: string | null;
}

function createAuthStore() {
	let state = $state<AuthState>({
		user: null,
		isAuthenticated: false,
		isLoading: true,
		error: null
	});

	// Initialize from localStorage
	if (browser) {
		const storedUser = localStorage.getItem('user');
		if (storedUser) {
			try {
				state.user = JSON.parse(storedUser);
				state.isAuthenticated = true;
			} catch (e) {
				localStorage.removeItem('user');
			}
		}
		state.isLoading = false;
	}

	return {
		get user() {
			return state.user;
		},
		get isAuthenticated() {
			return state.isAuthenticated;
		},
		get isLoading() {
			return state.isLoading;
		},
		get error() {
			return state.error;
		},

		/**
		 * Login with email and password
		 */
		async login(credentials: LoginRequest): Promise<boolean> {
			state.isLoading = true;
			state.error = null;

			try {
				const response = await authApi.login(credentials);
				state.user = response.user;
				state.isAuthenticated = true;

				if (browser) {
					localStorage.setItem('user', JSON.stringify(response.user));
				}

				return true;
			} catch (error: any) {
				state.error = error.message || 'Login failed';
				state.isAuthenticated = false;
				state.user = null;
				return false;
			} finally {
				state.isLoading = false;
			}
		},

		/**
		 * Register new user
		 */
		async register(data: RegisterRequest): Promise<boolean> {
			state.isLoading = true;
			state.error = null;

			try {
				const response = await authApi.register(data);
				state.user = response.user;
				state.isAuthenticated = true;

				if (browser) {
					localStorage.setItem('user', JSON.stringify(response.user));
				}

				return true;
			} catch (error: any) {
				state.error = error.message || 'Registration failed';
				state.isAuthenticated = false;
				state.user = null;
				return false;
			} finally {
				state.isLoading = false;
			}
		},

		/**
		 * Logout current user
		 */
		async logout(): Promise<void> {
			try {
				await authApi.logout();
			} catch (error) {
				console.error('Logout error:', error);
			} finally {
				state.user = null;
				state.isAuthenticated = false;
				state.error = null;

				if (browser) {
					localStorage.removeItem('user');
					goto('/login');
				}
			}
		},

		/**
		 * Refresh current user data
		 */
		async refreshUser(): Promise<void> {
			if (!state.isAuthenticated) return;

			try {
				const user = await authApi.getCurrentUser();
				state.user = user;

				if (browser) {
					localStorage.setItem('user', JSON.stringify(user));
				}
			} catch (error: any) {
				console.error('Failed to refresh user:', error);
				// If refresh fails (e.g., token expired), logout
				if (error.status === 401) {
					await this.logout();
				}
			}
		},

		/**
		 * Clear error message
		 */
		clearError(): void {
			state.error = null;
		},

		/**
		 * Check if user has specific role
		 */
		hasRole(role: number): boolean {
			return state.user ? state.user.role >= role : false;
		},

		/**
		 * Check if user is moderator or admin
		 */
		isModerator(): boolean {
			return this.hasRole(2); // Moderator = 2
		},

		/**
		 * Check if user is admin
		 */
		isAdmin(): boolean {
			return this.hasRole(3); // Admin = 3
		}
	};
}

export const authStore = createAuthStore();
