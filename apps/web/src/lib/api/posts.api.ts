import { apiClient } from './client';
import type {
	Post,
	CreatePostRequest,
	UpdatePostRequest,
	PostFilters,
	PaginatedResponse
} from '$lib/types/entities.types';

export const postsApi = {
	/**
	 * Get paginated list of posts
	 */
	async getPosts(filters?: PostFilters): Promise<PaginatedResponse<Post>> {
		return apiClient.get<PaginatedResponse<Post>>('/posts', filters);
	},

	/**
	 * Get post by ID
	 */
	async getPost(id: string): Promise<Post> {
		return apiClient.get<Post>(`/posts/${id}`);
	},

	/**
	 * Create new post
	 */
	async createPost(data: CreatePostRequest): Promise<Post> {
		return apiClient.post<Post>('/posts', data);
	},

	/**
	 * Update existing post
	 */
	async updatePost(id: string, data: UpdatePostRequest): Promise<Post> {
		return apiClient.put<Post>(`/posts/${id}`, data);
	},

	/**
	 * Delete post (soft delete)
	 */
	async deletePost(id: string): Promise<void> {
		return apiClient.delete<void>(`/posts/${id}`);
	},

	/**
	 * Get comments for a post
	 */
	async getPostComments(id: string, params?: { page?: number; pageSize?: number }) {
		return apiClient.get(`/posts/${id}/comments`, params);
	}
};
