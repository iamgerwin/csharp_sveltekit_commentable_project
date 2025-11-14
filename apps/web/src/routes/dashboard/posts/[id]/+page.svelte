<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { postsApi } from '$lib/api/posts.api';
	import { commentsApi } from '$lib/api/comments.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import Reactions from '$lib/components/reactions.svelte';
	import type { Post, Comment, PaginatedResponse } from '$lib/types/entities.types';
	import { CommentableType } from '$lib/types/entities.types';

	let postId = $derived($page.params.id as string);
	let post = $state<Post | null>(null);
	let comments = $state<Comment[]>([]);
	let isLoadingPost = $state(true);
	let isLoadingComments = $state(true);
	let isDeleting = $state(false);
	let newCommentText = $state('');
	let isSubmittingComment = $state(false);

	async function loadPost() {
		isLoadingPost = true;
		try {
			const response = await postsApi.getPost(postId);

			if (!response) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				goto('/dashboard/posts');
				return;
			}

			post = response;
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load post');
			goto('/dashboard/posts');
		} finally {
			isLoadingPost = false;
		}
	}

	async function loadComments() {
		isLoadingComments = true;
		try {
			const response: PaginatedResponse<Comment> = await commentsApi.getComments({
				commentableId: postId,
				commentableType: CommentableType.Post,
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
			toastStore.error(error.message || 'Failed to load comments');
		} finally {
			isLoadingComments = false;
		}
	}

	async function handleSubmitComment() {
		if (!newCommentText.trim()) return;

		isSubmittingComment = true;
		try {
			const newComment = await commentsApi.createComment({
				content: newCommentText,
				commentableId: postId,
				commentableType: CommentableType.Post,
				parentCommentId: null
			});

			comments = [newComment, ...comments];
			newCommentText = '';
			toastStore.success('Comment posted!');
			await loadPost();
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to post comment');
		} finally {
			isSubmittingComment = false;
		}
	}

	async function handleDeletePost() {
		if (!confirm('Are you sure you want to delete this post?')) return;

		isDeleting = true;
		try {
			await postsApi.deletePost(postId);
			toastStore.success('Post deleted successfully');
			goto('/dashboard/posts');
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to delete post');
			isDeleting = false;
		}
	}

	function formatDate(dateString: string): string {
		return new Date(dateString).toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'long',
			day: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	onMount(() => {
		loadPost();
		loadComments();
	});
</script>

<svelte:head>
	<title>{post?.title || 'Post'} - Commentable</title>
</svelte:head>

<div class="space-y-6">
	<Button variant="outline" onclick={() => goto('/dashboard/posts')}>
		← Back to Posts
	</Button>

	{#if isLoadingPost}
		<Card class="p-8 animate-pulse space-y-4">
			<div class="h-8 bg-gray-200 dark:bg-gray-700 rounded"></div>
			<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded"></div>
		</Card>
	{:else if post}
		<Card class="p-8">
			<div class="flex items-start justify-between mb-6">
				<div class="flex-1">
					<h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">
						{post.title}
					</h1>
					<div class="flex items-center gap-4 text-sm text-gray-600 dark:text-gray-400">
						<span>{post.username}</span>
						<span>•</span>
						<span>{formatDate(post.createdAt)}</span>
					</div>
				</div>

				{#if authStore.user?.id === post.userId || authStore.isModerator()}
					<div class="flex gap-2">
						<Button variant="outline" size="sm" onclick={() => goto(`/dashboard/posts/${post?.id}/edit`)}>
							Edit
						</Button>
						<Button variant="destructive" size="sm" onclick={handleDeletePost} disabled={isDeleting}>
							{isDeleting ? 'Deleting...' : 'Delete'}
						</Button>
					</div>
				{/if}
			</div>

			{#if post.tags && post.tags.length > 0}
				<div class="flex flex-wrap gap-2 mb-6">
					{#each post.tags as tag}
						<span class="px-3 py-1 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-200 rounded-full text-sm">
							#{tag}
						</span>
					{/each}
				</div>
			{/if}

			<div class="mb-6">
				<Reactions
					commentableId={post.id}
					commentableType={CommentableType.Post}
					bind:reactionCounts={post.reactionCounts}
					bind:userReaction={post.userReaction}
					size="lg"
				/>
			</div>

			<div class="prose dark:prose-invert max-w-none">
				<p class="text-gray-700 dark:text-gray-300 whitespace-pre-wrap text-lg leading-relaxed">
					{post.content}
				</p>
			</div>
		</Card>

		<Card class="p-6">
			<h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">
				Comments ({post.commentCount})
			</h2>

			{#if authStore.isAuthenticated}
				<div class="mb-8">
					<Textarea
						bind:value={newCommentText}
						placeholder="Add a comment..."
						rows={3}
						disabled={isSubmittingComment}
					/>
					<div class="flex justify-end mt-2">
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

			{#if isLoadingComments}
				<div class="space-y-4">
					{#each Array(3) as _}
						<div class="animate-pulse flex gap-4">
							<div class="w-10 h-10 bg-gray-200 dark:bg-gray-700 rounded-full"></div>
							<div class="flex-1 space-y-2">
								<div class="h-4 bg-gray-200 dark:bg-gray-700 rounded"></div>
								<div class="h-3 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
							</div>
						</div>
					{/each}
				</div>
			{:else if comments.length === 0}
				<div class="text-center py-8">
					<div class="text-4xl mb-2">💬</div>
					<p class="text-gray-600 dark:text-gray-400">No comments yet. Be the first!</p>
				</div>
			{:else}
				<div class="space-y-6">
					{#each comments as comment (comment.id)}
						<div class="flex gap-4">
							<div class="w-10 h-10 bg-gradient-to-br from-green-500 to-teal-500 rounded-full flex items-center justify-center text-white font-semibold">
								{comment.username.charAt(0).toUpperCase()}
							</div>
							<div class="flex-1">
								<div class="bg-gray-50 dark:bg-gray-800 rounded-lg p-4">
									<div class="flex items-center justify-between mb-2">
										<span class="font-semibold text-gray-900 dark:text-white">{comment.username}</span>
										<span class="text-sm text-gray-500 dark:text-gray-400">{formatDate(comment.createdAt)}</span>
									</div>
									<p class="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">{comment.content}</p>
								</div>
								<div class="flex items-center gap-4 mt-2">
									<Reactions
										commentableId={comment.id}
										commentableType={CommentableType.Comment}
										reactionCounts={comment.reactionCounts}
										userReaction={comment.userReaction}
										size="sm"
									/>
								</div>
							</div>
						</div>
					{/each}
				</div>
			{/if}
		</Card>
	{/if}
</div>
