<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { videosApi } from '$lib/api/videos.api';
	import { commentsApi } from '$lib/api/comments.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import Reactions from '$lib/components/reactions.svelte';
	import type { Video, Comment, PaginatedResponse } from '$lib/types/entities.types';
	import { CommentableType } from '$lib/types/entities.types';

	let videoId = $derived($page.params.id as string);
	let video = $state<Video | null>(null);
	let comments = $state<Comment[]>([]);
	let isLoadingVideo = $state(true);
	let isLoadingComments = $state(true);
	let isDeleting = $state(false);

	// Comment form
	let newCommentText = $state('');
	let isSubmittingComment = $state(false);
	let replyToCommentId = $state<string | null>(null);

	async function loadVideo() {
		isLoadingVideo = true;
		try {
			const response = await videosApi.getVideo(videoId);

			if (!response) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				goto('/dashboard/videos');
				return;
			}

			video = response;
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load video');
			goto('/dashboard/videos');
		} finally {
			isLoadingVideo = false;
		}
	}

	async function loadComments() {
		isLoadingComments = true;
		try {
			const response: PaginatedResponse<Comment> = await commentsApi.getComments({
				commentableId: videoId,
				commentableType: CommentableType.Video,
				parentCommentId: null,
				sortBy: 'createdAt',
				sortOrder: 'desc'
			});

			if (!response || !response.data) {
				console.warn('API returned empty or invalid response');
				comments = [];
				return;
			}

			comments = response.data;
		} catch (error: any) {
			console.error('Failed to load comments:', error);
			toastStore.error(error.message || 'Failed to load comments');
		} finally {
			isLoadingComments = false;
		}
	}

	async function handleSubmitComment() {
		if (!newCommentText.trim()) {
			toastStore.warning('Please enter a comment');
			return;
		}

		isSubmittingComment = true;

		try {
			const newComment = await commentsApi.createComment({
				content: newCommentText,
				commentableId: videoId,
				commentableType: CommentableType.Video,
				parentCommentId: replyToCommentId
			});

			// Add to comments list optimistically
			if (!replyToCommentId) {
				comments = [newComment, ...comments];
			}

			newCommentText = '';
			replyToCommentId = null;
			toastStore.success('Comment posted!');

			// Reload to get updated counts
			await loadVideo();
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to post comment');
		} finally {
			isSubmittingComment = false;
		}
	}

	async function handleDeleteVideo() {
		if (!confirm('Are you sure you want to delete this video? This action cannot be undone.')) {
			return;
		}

		isDeleting = true;

		try {
			await videosApi.deleteVideo(videoId);
			toastStore.success('Video deleted successfully');
			goto('/dashboard/videos');
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to delete video');
			isDeleting = false;
		}
	}

	function formatDate(dateString: string): string {
		const date = new Date(dateString);
		return date.toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'long',
			day: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	onMount(() => {
		loadVideo();
		loadComments();
	});
</script>

<svelte:head>
	<title>{video?.title || 'Video'} - Commentable</title>
</svelte:head>

<div class="space-y-6">
	<!-- Back Button -->
	<Button variant="outline" onclick={() => goto('/dashboard/videos')}>
		← Back to Videos
	</Button>

	{#if isLoadingVideo}
		<Card class="p-8">
			<div class="animate-pulse space-y-4">
				<div class="aspect-video bg-gray-200 dark:bg-gray-700 rounded-lg"></div>
				<div class="h-8 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
				<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded"></div>
			</div>
		</Card>
	{:else if video}
		<!-- Video Player -->
		<Card class="overflow-hidden">
			<div class="aspect-video bg-gray-900">
				{#if video.videoUrl}
					<video
						controls
						class="w-full h-full"
						poster={video.thumbnailUrl}
					>
						<source src={video.videoUrl} type="video/mp4" />
						<track kind="captions" />
						Your browser does not support the video tag.
					</video>
				{:else}
					<div class="w-full h-full flex items-center justify-center text-white">
						<div class="text-center">
							<div class="text-6xl mb-4">🎥</div>
							<p>Video not available</p>
						</div>
					</div>
				{/if}
			</div>

			<div class="p-6">
				<!-- Title and Actions -->
				<div class="flex items-start justify-between mb-4">
					<div class="flex-1">
						<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
							{video.title}
						</h1>
						<div class="flex items-center gap-4 text-sm text-gray-600 dark:text-gray-400">
							<span>{video.username}</span>
							<span>•</span>
							<span>{formatDate(video.createdAt)}</span>
							<span>•</span>
							<span>👁️ {video.viewCount} views</span>
						</div>
					</div>

					{#if authStore.user?.id === video.userId || authStore.isModerator()}
						<div class="flex gap-2">
							<Button
								variant="outline"
								size="sm"
								onclick={() => goto(`/dashboard/videos/${video?.id}/edit`)}
							>
								Edit
							</Button>
							<Button
								variant="destructive"
								size="sm"
								onclick={handleDeleteVideo}
								disabled={isDeleting}
							>
								{isDeleting ? 'Deleting...' : 'Delete'}
							</Button>
						</div>
					{/if}
				</div>

				<!-- Description -->
				<div class="prose dark:prose-invert max-w-none">
					<p class="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">
						{video.description}
					</p>
				</div>
			</div>
		</Card>

		<!-- Comments Section -->
		<Card class="p-6">
			<h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">
				Comments ({video.commentCount})
			</h2>

			<!-- Comment Form -->
			{#if authStore.isAuthenticated}
				<div class="mb-8">
					<Textarea
						bind:value={newCommentText}
						placeholder="Add a comment..."
						rows={3}
						disabled={isSubmittingComment}
					/>
					<div class="flex justify-end gap-2 mt-2">
						{#if replyToCommentId}
							<Button
								variant="outline"
								size="sm"
								onclick={() => {
									replyToCommentId = null;
									newCommentText = '';
								}}
							>
								Cancel
							</Button>
						{/if}
						<Button
							size="sm"
							onclick={handleSubmitComment}
							disabled={isSubmittingComment || !newCommentText.trim()}
						>
							{isSubmittingComment ? 'Posting...' : 'Post Comment'}
						</Button>
					</div>
				</div>
			{:else}
				<div class="mb-8 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg text-center">
					<p class="text-gray-600 dark:text-gray-400">
						Please <a href="/login" class="text-blue-600 hover:underline">login</a> to comment
					</p>
				</div>
			{/if}

			<!-- Comments List -->
			{#if isLoadingComments}
				<div class="space-y-4">
					{#each Array(3) as _}
						<div class="animate-pulse flex gap-4">
							<div class="w-10 h-10 bg-gray-200 dark:bg-gray-700 rounded-full"></div>
							<div class="flex-1 space-y-2">
								<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-1/4"></div>
								<div class="h-3 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
							</div>
						</div>
					{/each}
				</div>
			{:else if comments.length === 0}
				<div class="text-center py-8">
					<div class="text-4xl mb-2">💬</div>
					<p class="text-gray-600 dark:text-gray-400">
						No comments yet. Be the first to comment!
					</p>
				</div>
			{:else}
				<div class="space-y-6">
					{#each comments as comment (comment.id)}
						<div class="flex gap-4">
							<div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-500 rounded-full flex items-center justify-center text-white font-semibold">
								{comment.username.charAt(0).toUpperCase()}
							</div>
							<div class="flex-1">
								<div class="bg-gray-50 dark:bg-gray-800 rounded-lg p-4">
									<div class="flex items-center justify-between mb-2">
										<span class="font-semibold text-gray-900 dark:text-white">
											{comment.username}
										</span>
										<span class="text-sm text-gray-500 dark:text-gray-400">
											{formatDate(comment.createdAt)}
										</span>
									</div>
									<p class="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">
										{comment.content}
									</p>
								</div>
								<div class="flex items-center gap-4 mt-2">
									<Reactions
										commentableId={comment.id}
										commentableType={CommentableType.Comment}
										reactionCounts={comment.reactionCounts}
										userReaction={comment.userReaction}
										size="sm"
									/>
									{#if comment.replyCount > 0}
										<span class="text-sm text-gray-600 dark:text-gray-400">
											{comment.replyCount} {comment.replyCount === 1 ? 'reply' : 'replies'}
										</span>
									{/if}
								</div>
							</div>
						</div>
					{/each}
				</div>
			{/if}
		</Card>
	{/if}
</div>
