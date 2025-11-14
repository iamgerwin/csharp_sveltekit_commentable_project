import { apiClient } from './client';
import type {
	Reaction,
	UpsertReactionRequest,
	PaginatedResponse
} from '$lib/types/entities.types';

export const reactionsApi = {
	/**
	 * Get reactions (with optional filters)
	 */
	async getReactions(params?: {
		commentableId?: string;
		commentableType?: number;
		page?: number;
		pageSize?: number;
	}): Promise<PaginatedResponse<Reaction>> {
		return apiClient.get<PaginatedResponse<Reaction>>('/reactions', params);
	},

	/**
	 * Create or update a reaction (upsert)
	 * If user already reacted with same type, it removes the reaction
	 * If user reacted with different type, it updates to new type
	 * If user hasn't reacted, it creates new reaction
	 */
	async upsertReaction(data: UpsertReactionRequest): Promise<Reaction | null> {
		return apiClient.post<Reaction | null>('/reactions/upsert', data);
	},

	/**
	 * Delete a reaction
	 */
	async deleteReaction(id: string): Promise<void> {
		return apiClient.delete<void>(`/reactions/${id}`);
	}
};
