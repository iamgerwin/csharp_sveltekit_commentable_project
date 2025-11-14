<script lang="ts">
	import { onMount } from 'svelte';
	import { postsApi } from '$lib/api/posts.api';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Reactions from '$lib/components/reactions.svelte';
	import type { Post, PostFilters, PaginatedResponse } from '$lib/types/entities.types';
	import { CommentableType } from '$lib/types/entities.types';

	let posts = $state<Post[]>([]);
	let isLoading = $state(true);
	let currentPage = $state(1);
	let totalPages = $state(1);
	let searchQuery = $state('');
	let searchTimeout: ReturnType<typeof setTimeout>;

	async function loadPosts(page = 1) {
		isLoading = true;
		try {
			const filters: PostFilters = {
				page,
				pageSize: 10,
				search: searchQuery || undefined,
				sortBy: 'createdAt',
				sortOrder: 'desc'
			};

			const response: PaginatedResponse<Post> = await postsApi.getPosts(filters);

			// Handle undefined or invalid response
			if (!response || !response.data) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				posts = [];
				currentPage = 1;
				totalPages = 1;
				return;
			}

			posts = response.data;
			currentPage = response.page;
			totalPages = response.totalPages;
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load posts');
			posts = [];
		} finally {
			isLoading = false;
		}
	}

	function handleSearch() {
		clearTimeout(searchTimeout);
		searchTimeout = setTimeout(() => loadPosts(1), 500);
	}

	function formatDate(dateString: string): string {
		return new Date(dateString).toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'short',
			day: 'numeric'
		});
	}

	onMount(() => loadPosts());
</script>

<svelte:head>
	<title>Posts - Commentable</title>
</svelte:head>

<div class="space-y-6">
	<div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
		<div>
			<h1 class="text-3xl font-bold text-gray-900 dark:text-white">Posts</h1>
			<p class="text-gray-600 dark:text-gray-400 mt-1">Browse and discover posts</p>
		</div>
		<Button onclick={() => window.location.href = '/dashboard/posts/new'}>
			<span class="mr-2">+</span>
			Create Post
		</Button>
	</div>

	<Card class="p-4">
		<Input
			type="search"
			placeholder="Search posts..."
			bind:value={searchQuery}
			oninput={handleSearch}
		/>
	</Card>

	{#if isLoading}
		<div class="space-y-4">
			{#each Array(5) as _}
				<Card class="p-6 animate-pulse">
					<div class="h-6 bg-gray-200 dark:bg-gray-700 rounded mb-3"></div>
					<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
				</Card>
			{/each}
		</div>
	{:else if posts.length === 0}
		<Card class="p-12 text-center">
			<div class="text-6xl mb-4">📝</div>
			<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">No posts found</h3>
			<p class="text-gray-600 dark:text-gray-400 mb-6">
				{searchQuery ? 'Try adjusting your search query' : 'Be the first to create a post!'}
			</p>
			<Button onclick={() => window.location.href = '/dashboard/posts/new'}>Create Post</Button>
		</Card>
	{:else}
		<div class="space-y-4">
			{#each posts as post (post.id)}
				<a href="/dashboard/posts/{post.id}">
					<Card class="p-6 hover:shadow-lg transition-shadow">
						<h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">
							{post.title}
						</h3>
						<p class="text-gray-600 dark:text-gray-300 mb-4 line-clamp-2">
							{post.content}
						</p>

						{#if post.tags && post.tags.length > 0}
							<div class="flex flex-wrap gap-2 mb-4">
								{#each post.tags as tag}
									<span class="px-2 py-1 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-200 rounded text-sm">
										#{tag}
									</span>
								{/each}
							</div>
						{/if}

						<div class="flex items-center justify-between text-sm">
							<div class="flex items-center gap-3 text-gray-500 dark:text-gray-400">
								<span>{post.username}</span>
								<span>•</span>
								<span>{formatDate(post.createdAt)}</span>
								<span>•</span>
								<span>💬 {post.commentCount}</span>
							</div>
							<Reactions
								commentableId={post.id}
								commentableType={CommentableType.Post}
								reactionCounts={post.reactionCounts}
								userReaction={post.userReaction}
								size="sm"
								showTotal={false}
							/>
						</div>
					</Card>
				</a>
			{/each}
		</div>

		{#if totalPages > 1}
			<div class="flex justify-center items-center gap-2">
				<Button variant="outline" disabled={currentPage === 1} onclick={() => loadPosts(currentPage - 1)}>
					Previous
				</Button>
				<span class="text-sm text-gray-600 dark:text-gray-400 px-4">
					Page {currentPage} of {totalPages}
				</span>
				<Button variant="outline" disabled={currentPage === totalPages} onclick={() => loadPosts(currentPage + 1)}>
					Next
				</Button>
			</div>
		{/if}
	{/if}
</div>
