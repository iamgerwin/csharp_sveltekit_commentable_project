<script lang="ts">
	import { commentsApi } from '$lib/api/comments.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import Reactions from '$lib/components/reactions.svelte';
	import type { Comment, PaginatedResponse } from '$lib/types/entities.types';
	import { CommentableType } from '$lib/types/entities.types';
	import CommentItem from './comment-item.svelte';

	interface CommentItemProps {
		comment: Comment;
		onCommentDeleted?: (commentId: string) => void;
		onCommentUpdated?: (comment: Comment) => void;
		depth?: number;
	}

	let {
		comment,
		onCommentDeleted,
		onCommentUpdated,
		depth = 0
	}: CommentItemProps = $props();

	let isEditing = $state(false);
	let isDeleting = $state(false);
	let showReplies = $state(false);
	let showReplyForm = $state(false);
	let editedContent = $state(comment.content);
	let replyContent = $state('');
	let replies = $state<Comment[]>([]);
	let isLoadingReplies = $state(false);
	let isSubmittingReply = $state(false);
	let isSubmittingEdit = $state(false);

	async function loadReplies() {
		if (replies.length > 0) {
			showReplies = !showReplies;
			return;
		}

		isLoadingReplies = true;
		showReplies = true;

		try {
			const response: PaginatedResponse<Comment> = await commentsApi.getCommentReplies(
				comment.id,
				{ pageSize: 50 }
			);

			if (!response || !response.data) {
				console.warn('API returned empty or invalid response');
				replies = [];
				return;
			}

			replies = response.data;
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load replies');
		} finally {
			isLoadingReplies = false;
		}
	}

	async function handleSubmitReply() {
		if (!replyContent.trim()) return;

		isSubmittingReply = true;

		try {
			const newReply = await commentsApi.createComment({
				content: replyContent,
				commentableId: comment.commentableId,
				commentableType: comment.commentableType,
				parentCommentId: comment.id
			});

			replies = [newReply, ...replies];
			replyContent = '';
			showReplyForm = false;
			showReplies = true;
			comment.replyCount = (comment.replyCount || 0) + 1;
			toastStore.success('Reply posted!');
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to post reply');
		} finally {
			isSubmittingReply = false;
		}
	}

	async function handleUpdateComment() {
		if (!editedContent.trim()) return;

		isSubmittingEdit = true;

		try {
			const updatedComment = await commentsApi.updateComment(comment.id, {
				content: editedContent
			});

			comment.content = updatedComment.content;
			comment.updatedAt = updatedComment.updatedAt;
			isEditing = false;
			toastStore.success('Comment updated!');

			if (onCommentUpdated) {
				onCommentUpdated(comment);
			}
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to update comment');
		} finally {
			isSubmittingEdit = false;
		}
	}

	async function handleDeleteComment() {
		if (!confirm('Are you sure you want to delete this comment?')) return;

		isDeleting = true;

		try {
			await commentsApi.deleteComment(comment.id);
			toastStore.success('Comment deleted!');

			if (onCommentDeleted) {
				onCommentDeleted(comment.id);
			}
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to delete comment');
			isDeleting = false;
		}
	}

	function formatDate(dateString: string): string {
		const date = new Date(dateString);
		const now = new Date();
		const diff = now.getTime() - date.getTime();
		const minutes = Math.floor(diff / 60000);
		const hours = Math.floor(diff / 3600000);
		const days = Math.floor(diff / 86400000);

		if (minutes < 1) return 'just now';
		if (minutes < 60) return `${minutes}m ago`;
		if (hours < 24) return `${hours}h ago`;
		if (days < 7) return `${days}d ago`;

		return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
	}

	const canEdit = $derived(
		authStore.user?.id === comment.userId || authStore.isModerator()
	);

	const maxDepth = 5; // Maximum nesting level
</script>

<div class="flex gap-3" style="margin-left: {depth > 0 ? '2rem' : '0'}">
	<!-- Avatar -->
	<div class="w-8 h-8 bg-gradient-to-br from-purple-500 to-pink-500 rounded-full flex items-center justify-center text-white font-semibold text-sm flex-shrink-0">
		{comment.username.charAt(0).toUpperCase()}
	</div>

	<!-- Content -->
	<div class="flex-1 min-w-0">
		<div class="bg-gray-50 dark:bg-gray-800 rounded-lg p-3">
			<div class="flex items-center justify-between mb-1">
				<span class="font-semibold text-sm text-gray-900 dark:text-white">
					{comment.username}
				</span>
				<span class="text-xs text-gray-500 dark:text-gray-400">
					{formatDate(comment.createdAt)}
				</span>
			</div>

			{#if isEditing}
				<div class="space-y-2">
					<Textarea
						bind:value={editedContent}
						rows={3}
						disabled={isSubmittingEdit}
					/>
					<div class="flex gap-2">
						<Button
							size="sm"
							onclick={handleUpdateComment}
							disabled={isSubmittingEdit || !editedContent.trim()}
						>
							{isSubmittingEdit ? 'Saving...' : 'Save'}
						</Button>
						<Button
							size="sm"
							variant="outline"
							onclick={() => {
								isEditing = false;
								editedContent = comment.content;
							}}
							disabled={isSubmittingEdit}
						>
							Cancel
						</Button>
					</div>
				</div>
			{:else}
				<p class="text-sm text-gray-700 dark:text-gray-300 whitespace-pre-wrap break-words">
					{comment.content}
				</p>
			{/if}
		</div>

		<!-- Actions -->
		<div class="flex items-center gap-3 mt-2 flex-wrap">
			<Reactions
				commentableId={comment.id}
				commentableType={CommentableType.Comment}
				bind:reactionCounts={comment.reactionCounts}
				bind:userReaction={comment.userReaction}
				size="sm"
			/>

			{#if comment.replyCount > 0}
				<button
					onclick={loadReplies}
					class="text-xs text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300 font-medium"
				>
					{showReplies ? '−' : '+'} {comment.replyCount} {comment.replyCount === 1 ? 'reply' : 'replies'}
				</button>
			{/if}

			{#if authStore.isAuthenticated && depth < maxDepth}
				<button
					onclick={() => (showReplyForm = !showReplyForm)}
					class="text-xs text-gray-600 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300 font-medium"
				>
					Reply
				</button>
			{/if}

			{#if canEdit}
				<button
					onclick={() => {
						isEditing = true;
						editedContent = comment.content;
					}}
					class="text-xs text-gray-600 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300 font-medium"
				>
					Edit
				</button>
				<button
					onclick={handleDeleteComment}
					class="text-xs text-red-600 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300 font-medium"
					disabled={isDeleting}
				>
					{isDeleting ? 'Deleting...' : 'Delete'}
				</button>
			{/if}
		</div>

		<!-- Reply Form -->
		{#if showReplyForm}
			<div class="mt-3 space-y-2">
				<Textarea
					bind:value={replyContent}
					placeholder="Write a reply..."
					rows={2}
					disabled={isSubmittingReply}
				/>
				<div class="flex gap-2">
					<Button
						size="sm"
						onclick={handleSubmitReply}
						disabled={isSubmittingReply || !replyContent.trim()}
					>
						{isSubmittingReply ? 'Posting...' : 'Post Reply'}
					</Button>
					<Button
						size="sm"
						variant="outline"
						onclick={() => {
							showReplyForm = false;
							replyContent = '';
						}}
						disabled={isSubmittingReply}
					>
						Cancel
					</Button>
				</div>
			</div>
		{/if}

		<!-- Nested Replies -->
		{#if showReplies}
			<div class="mt-4 space-y-4">
				{#if isLoadingReplies}
					<div class="text-sm text-gray-500 dark:text-gray-400">Loading replies...</div>
				{:else if replies.length > 0}
					{#each replies as reply (reply.id)}
						<CommentItem
							comment={reply}
							{onCommentDeleted}
							{onCommentUpdated}
							depth={depth + 1}
						/>
					{/each}
				{/if}
			</div>
		{/if}
	</div>
</div>
