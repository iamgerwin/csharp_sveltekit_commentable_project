<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import { reportsApi } from '$lib/api/reports.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import type { Report, ReportFilters, PaginatedResponse } from '$lib/types/entities.types';
	import { ReportStatus, ReportCategory } from '$lib/types/entities.types';

	let reports = $state<Report[]>([]);
	let isLoading = $state(true);
	let currentPage = $state(1);
	let totalPages = $state(1);
	let statusFilter = $state<ReportStatus | undefined>(undefined);

	async function loadReports(page = 1) {
		if (!authStore.isModerator()) {
			toastStore.error('You do not have permission to view reports');
			goto('/dashboard');
			return;
		}

		isLoading = true;

		try {
			const filters: ReportFilters = {
				page,
				pageSize: 20,
				reportStatus: statusFilter,
				sortBy: 'createdAt',
				sortOrder: 'desc'
			};

			const response: PaginatedResponse<Report> = await reportsApi.getReports(filters);

			// Handle undefined or invalid response
			if (!response || !response.data) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				reports = [];
				currentPage = 1;
				totalPages = 1;
				return;
			}

			reports = response.data;
			currentPage = response.page;
			totalPages = response.totalPages;
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load reports');
			reports = [];
		} finally {
			isLoading = false;
		}
	}

	function getStatusBadgeClass(status: ReportStatus): string {
		switch (status) {
			case ReportStatus.Pending:
				return 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-200';
			case ReportStatus.UnderReview:
				return 'bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-200';
			case ReportStatus.Resolved:
				return 'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-200';
			case ReportStatus.Dismissed:
				return 'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-200';
			default:
				return 'bg-gray-100 text-gray-700';
		}
	}

	function getStatusLabel(status: ReportStatus): string {
		return ReportStatus[status];
	}

	function getCategoryLabel(category: ReportCategory): string {
		return ReportCategory[category].replace(/([A-Z])/g, ' $1').trim();
	}

	function formatDate(dateString: string): string {
		return new Date(dateString).toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'short',
			day: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	onMount(() => {
		if (!authStore.isModerator()) {
			toastStore.error('You do not have permission to access this page');
			goto('/dashboard');
			return;
		}
		loadReports();
	});
</script>

<svelte:head>
	<title>Reports - Commentable</title>
</svelte:head>

{#if authStore.isModerator()}
	<div class="space-y-6">
		<div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
			<div>
				<h1 class="text-3xl font-bold text-gray-900 dark:text-white">Reports</h1>
				<p class="text-gray-600 dark:text-gray-400 mt-1">
					Manage user reports and content moderation
				</p>
			</div>
		</div>

		<!-- Filters -->
		<Card class="p-4">
			<div class="flex flex-wrap gap-2">
				<Button
					variant={statusFilter === undefined ? 'default' : 'outline'}
					size="sm"
					onclick={() => {
						statusFilter = undefined;
						loadReports(1);
					}}
				>
					All
				</Button>
				<Button
					variant={statusFilter === ReportStatus.Pending ? 'default' : 'outline'}
					size="sm"
					onclick={() => {
						statusFilter = ReportStatus.Pending;
						loadReports(1);
					}}
				>
					Pending
				</Button>
				<Button
					variant={statusFilter === ReportStatus.UnderReview ? 'default' : 'outline'}
					size="sm"
					onclick={() => {
						statusFilter = ReportStatus.UnderReview;
						loadReports(1);
					}}
				>
					Under Review
				</Button>
				<Button
					variant={statusFilter === ReportStatus.Resolved ? 'default' : 'outline'}
					size="sm"
					onclick={() => {
						statusFilter = ReportStatus.Resolved;
						loadReports(1);
					}}
				>
					Resolved
				</Button>
				<Button
					variant={statusFilter === ReportStatus.Dismissed ? 'default' : 'outline'}
					size="sm"
					onclick={() => {
						statusFilter = ReportStatus.Dismissed;
						loadReports(1);
					}}
				>
					Dismissed
				</Button>
			</div>
		</Card>

		<!-- Reports List -->
		{#if isLoading}
			<div class="space-y-4">
				{#each Array(5) as _}
					<Card class="p-6 animate-pulse">
						<div class="h-6 bg-gray-200 dark:bg-gray-700 rounded mb-3"></div>
						<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
					</Card>
				{/each}
			</div>
		{:else if reports.length === 0}
			<Card class="p-12 text-center">
				<div class="text-6xl mb-4">🚩</div>
				<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">
					No reports found
				</h3>
				<p class="text-gray-600 dark:text-gray-400">
					{statusFilter !== undefined ? 'No reports with this status' : 'All clear!'}
				</p>
			</Card>
		{:else}
			<div class="space-y-4">
				{#each reports as report (report.id)}
					<Card class="p-6 hover:shadow-lg transition-shadow">
						<div class="flex items-start justify-between mb-4">
							<div class="flex-1">
								<div class="flex items-center gap-2 mb-2">
									<span class="px-2 py-1 rounded text-xs font-medium {getStatusBadgeClass(report.reportStatus)}">
										{getStatusLabel(report.reportStatus)}
									</span>
									<span class="px-2 py-1 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded text-xs font-medium">
										{getCategoryLabel(report.category)}
									</span>
								</div>
								<h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-1">
									Report #{report.id.slice(0, 8)}
								</h3>
								<p class="text-sm text-gray-600 dark:text-gray-400">
									Reported by <span class="font-medium">{report.reporterUsername}</span>
									on {formatDate(report.createdAt)}
								</p>
							</div>
							<Button
								size="sm"
								onclick={() => goto(`/dashboard/reports/${report.id}`)}
							>
								View Details
							</Button>
						</div>

						<div class="bg-gray-50 dark:bg-gray-800 rounded-lg p-4">
							<p class="text-sm text-gray-700 dark:text-gray-300 whitespace-pre-wrap">
								{report.description}
							</p>
						</div>

						{#if report.reviewerUsername}
							<div class="mt-3 text-sm text-gray-600 dark:text-gray-400">
								Reviewed by <span class="font-medium">{report.reviewerUsername}</span>
								{#if report.reviewedAt}
									on {formatDate(report.reviewedAt)}
								{/if}
							</div>
						{/if}
					</Card>
				{/each}
			</div>

			<!-- Pagination -->
			{#if totalPages > 1}
				<div class="flex justify-center items-center gap-2">
					<Button
						variant="outline"
						disabled={currentPage === 1}
						onclick={() => loadReports(currentPage - 1)}
					>
						Previous
					</Button>
					<span class="text-sm text-gray-600 dark:text-gray-400 px-4">
						Page {currentPage} of {totalPages}
					</span>
					<Button
						variant="outline"
						disabled={currentPage === totalPages}
						onclick={() => loadReports(currentPage + 1)}
					>
						Next
					</Button>
				</div>
			{/if}
		{/if}
	</div>
{/if}
