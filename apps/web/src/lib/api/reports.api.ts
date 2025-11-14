import { apiClient } from './client';
import type {
	Report,
	CreateReportRequest,
	ReviewReportRequest,
	ReportFilters,
	PaginatedResponse
} from '$lib/types/entities.types';

export const reportsApi = {
	/**
	 * Get paginated list of reports (moderators only)
	 */
	async getReports(filters?: ReportFilters): Promise<PaginatedResponse<Report>> {
		return apiClient.get<PaginatedResponse<Report>>('/reports', filters);
	},

	/**
	 * Get report by ID
	 */
	async getReport(id: string): Promise<Report> {
		return apiClient.get<Report>(`/reports/${id}`);
	},

	/**
	 * Create a new report
	 */
	async createReport(data: CreateReportRequest): Promise<Report> {
		return apiClient.post<Report>('/reports', data);
	},

	/**
	 * Review a report (moderators only)
	 */
	async reviewReport(id: string, data: ReviewReportRequest): Promise<Report> {
		return apiClient.put<Report>(`/reports/${id}/review`, data);
	}
};
