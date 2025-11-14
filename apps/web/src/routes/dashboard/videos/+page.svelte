<script lang="ts">
	import { onMount } from 'svelte';
	import { videosApi } from '$lib/api/videos.api';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Reactions from '$lib/components/reactions.svelte';
	import type { Video, VideoFilters, PaginatedResponse } from '$lib/types/entities.types';
	import { CommentableType } from '$lib/types/entities.types';

	let videos = $state<Video[]>([]);
	let isLoading = $state(true);
	let currentPage = $state(1);
	let totalPages = $state(1);
	let searchQuery = $state('');
	let searchTimeout: ReturnType<typeof setTimeout>;

	async function loadVideos(page = 1) {
		isLoading = true;

		try {
			const filters: VideoFilters = {
				page,
				pageSize: 12,
				search: searchQuery || undefined,
				sortBy: 'createdAt',
				sortOrder: 'desc'
			};

			const response: PaginatedResponse<Video> = await videosApi.getVideos(filters);

			// Handle undefined or invalid response
			if (!response || !response.data) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				videos = [];
				currentPage = 1;
				totalPages = 1;
				return;
			}

			videos = response.data;
			currentPage = response.page;
			totalPages = response.totalPages;
		} catch (error: any) {
			console.error('Failed to load videos:', error);
			toastStore.error(error.message || 'Failed to load videos');
			videos = [];
		} finally {
			isLoading = false;
		}
	}

	function handleSearch() {
		clearTimeout(searchTimeout);
		searchTimeout = setTimeout(() => {
			loadVideos(1);
		}, 500);
	}

	function formatDuration(seconds?: number): string {
		if (!seconds) return '--:--';
		const mins = Math.floor(seconds / 60);
		const secs = seconds % 60;
		return `${mins}:${secs.toString().padStart(2, '0')}`;
	}

	function formatDate(dateString: string): string {
		const date = new Date(dateString);
		return date.toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'short',
			day: 'numeric'
		});
	}

	onMount(() => {
		loadVideos();
	});
</script>

<svelte:head>
	<title>Videos - Commentable</title>
</svelte:head>

<div class="space-y-6">
	<!-- Header -->
	<div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
		<div>
			<h1 class="text-3xl font-bold text-gray-900 dark:text-white">Videos</h1>
			<p class="text-gray-600 dark:text-gray-400 mt-1">
				Browse and discover videos
			</p>
		</div>
		<Button onclick={() => window.location.href = '/dashboard/videos/new'}>
			<span class="mr-2">+</span>
			Upload Video
		</Button>
	</div>

	<!-- Search and Filters -->
	<Card class="p-4">
		<div class="flex flex-col sm:flex-row gap-4">
			<div class="flex-1">
				<Input
					type="search"
					placeholder="Search videos..."
					bind:value={searchQuery}
					oninput={handleSearch}
				/>
			</div>
		</div>
	</Card>

	<!-- Videos Grid -->
	{#if isLoading}
		<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
			{#each Array(6) as _}
				<Card class="p-4 animate-pulse">
					<div class="aspect-video bg-gray-200 dark:bg-gray-700 rounded-lg mb-4"></div>
					<div class="h-6 bg-gray-200 dark:bg-gray-700 rounded mb-2"></div>
					<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
				</Card>
			{/each}
		</div>
	{:else if videos.length === 0}
		<Card class="p-12 text-center">
			<div class="text-6xl mb-4">🎥</div>
			<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">
				No videos found
			</h3>
			<p class="text-gray-600 dark:text-gray-400 mb-6">
				{searchQuery ? 'Try adjusting your search query' : 'Be the first to upload a video!'}
			</p>
			<Button onclick={() => window.location.href = '/dashboard/videos/new'}>
				Upload Video
			</Button>
		</Card>
	{:else}
		<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
			{#each videos as video (video.id)}
				<a href="/dashboard/videos/{video.id}" class="block h-full">
					<Card class="overflow-hidden hover:shadow-lg transition-shadow h-full flex flex-col">
						<!-- Thumbnail -->
						<div class="aspect-video bg-gray-900 relative flex-shrink-0">
							{#if video.thumbnailUrl}
								<img
									src={video.thumbnailUrl}
									alt={video.title}
									class="w-full h-full object-cover"
								/>
							{:else}
								<div class="w-full h-full flex items-center justify-center text-6xl">
									🎥
								</div>
							{/if}
							<div class="absolute bottom-2 right-2 bg-black bg-opacity-75 text-white text-xs px-2 py-1 rounded">
								{formatDuration(video.duration)}
							</div>
						</div>

						<!-- Content -->
						<div class="p-4 flex flex-col flex-1">
							<h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2 line-clamp-2 min-h-[3.5rem]">
								{video.title}
							</h3>

							<p class="text-sm text-gray-600 dark:text-gray-400 mb-3 line-clamp-2 min-h-[2.5rem]">
								{video.description}
							</p>

							<div class="flex items-center justify-between text-sm text-gray-500 dark:text-gray-400 mb-3">
								<span class="truncate mr-2">{video.username}</span>
								<span class="flex-shrink-0">{formatDate(video.createdAt)}</span>
							</div>

							<div class="flex items-center justify-between mt-auto">
								<div class="flex items-center gap-3 text-sm text-gray-600 dark:text-gray-400">
									<span class="flex items-center gap-1">👁️ {video.viewCount}</span>
									<span class="flex items-center gap-1">💬 {video.commentCount}</span>
								</div>
								<Reactions
									commentableId={video.id}
									commentableType={CommentableType.Video}
									reactionCounts={video.reactionCounts}
									userReaction={video.userReaction}
									size="sm"
									showTotal={false}
								/>
							</div>
						</div>
					</Card>
				</a>
			{/each}
		</div>

		<!-- Pagination -->
		{#if totalPages > 1}
			<div class="flex justify-center items-center gap-2 mt-8">
				<Button
					variant="outline"
					disabled={currentPage === 1}
					onclick={() => loadVideos(currentPage - 1)}
				>
					Previous
				</Button>

				<span class="text-sm text-gray-600 dark:text-gray-400 px-4">
					Page {currentPage} of {totalPages}
				</span>

				<Button
					variant="outline"
					disabled={currentPage === totalPages}
					onclick={() => loadVideos(currentPage + 1)}
				>
					Next
				</Button>
			</div>
		{/if}
	{/if}
</div>
