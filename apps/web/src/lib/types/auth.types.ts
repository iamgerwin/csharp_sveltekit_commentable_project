// Authentication Types
export interface User {
	id: string;
	username: string;
	email: string;
	role: UserRole;
	createdAt: string;
	updatedAt: string;
}

export enum UserRole {
	Guest = 0,
	User = 1,
	Moderator = 2,
	Admin = 3
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface RegisterRequest {
	username: string;
	email: string;
	password: string;
	confirmPassword: string;
}

export interface AuthResponse {
	token: string;
	refreshToken: string;
	user: User;
	expiresIn: number;
}

export interface RefreshTokenRequest {
	refreshToken: string;
}

export interface AuthState {
	user: User | null;
	token: string | null;
	refreshToken: string | null;
	isAuthenticated: boolean;
	isLoading: boolean;
}
