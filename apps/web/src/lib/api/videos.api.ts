import { apiClient } from './client';
import type {
	Video,
	CreateVideoRequest,
	UpdateVideoRequest,
	VideoFilters,
	PaginatedResponse
} from '$lib/types/entities.types';

export const videosApi = {
	/**
	 * Get paginated list of videos
	 */
	async getVideos(filters?: VideoFilters): Promise<PaginatedResponse<Video>> {
		return apiClient.get<PaginatedResponse<Video>>('/videos', filters);
	},

	/**
	 * Get video by ID
	 */
	async getVideo(id: string): Promise<Video> {
		return apiClient.get<Video>(`/videos/${id}`);
	},

	/**
	 * Create new video
	 */
	async createVideo(data: CreateVideoRequest): Promise<Video> {
		return apiClient.post<Video>('/videos', data);
	},

	/**
	 * Update existing video
	 */
	async updateVideo(id: string, data: UpdateVideoRequest): Promise<Video> {
		return apiClient.put<Video>(`/videos/${id}`, data);
	},

	/**
	 * Delete video (soft delete)
	 */
	async deleteVideo(id: string): Promise<void> {
		return apiClient.delete<void>(`/videos/${id}`);
	},

	/**
	 * Get comments for a video
	 */
	async getVideoComments(id: string, params?: { page?: number; pageSize?: number }) {
		return apiClient.get(`/videos/${id}/comments`, params);
	}
};
