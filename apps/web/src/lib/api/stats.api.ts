import { apiClient } from './client';
import type { Stats } from '$lib/types/entities.types';

export const statsApi = {
	/**
	 * Get dashboard statistics
	 * Cached for 60 seconds on the backend
	 */
	async getStats(): Promise<Stats> {
		return apiClient.get<Stats>('/stats');
	}
};
