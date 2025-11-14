import { apiClient } from './client';
import type {
	Comment,
	CreateCommentRequest,
	UpdateCommentRequest,
	CommentFilters,
	PaginatedResponse
} from '$lib/types/entities.types';

export const commentsApi = {
	/**
	 * Get paginated list of comments
	 */
	async getComments(filters?: CommentFilters): Promise<PaginatedResponse<Comment>> {
		return apiClient.get<PaginatedResponse<Comment>>('/comments', filters);
	},

	/**
	 * Get comment by ID
	 */
	async getComment(id: string): Promise<Comment> {
		return apiClient.get<Comment>(`/comments/${id}`);
	},

	/**
	 * Create new comment
	 */
	async createComment(data: CreateCommentRequest): Promise<Comment> {
		return apiClient.post<Comment>('/comments', data);
	},

	/**
	 * Update existing comment
	 */
	async updateComment(id: string, data: UpdateCommentRequest): Promise<Comment> {
		return apiClient.put<Comment>(`/comments/${id}`, data);
	},

	/**
	 * Delete comment (soft delete)
	 */
	async deleteComment(id: string): Promise<void> {
		return apiClient.delete<void>(`/comments/${id}`);
	},

	/**
	 * Get replies for a comment
	 */
	async getCommentReplies(
		id: string,
		params?: { page?: number; pageSize?: number }
	): Promise<PaginatedResponse<Comment>> {
		return apiClient.get<PaginatedResponse<Comment>>(`/comments/${id}/replies`, params);
	}
};
