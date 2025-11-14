<script lang="ts">
	import { reactionsApi } from '$lib/api/reactions.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import { ReactionType, CommentableType, type ReactionCounts } from '$lib/types/entities.types';
	import { cn } from '$lib/utils';

	interface ReactionsProps {
		commentableId: string;
		commentableType: CommentableType;
		reactionCounts: ReactionCounts;
		userReaction?: ReactionType | null;
		onReactionChange?: (counts: ReactionCounts, userReaction: ReactionType | null) => void;
		size?: 'sm' | 'md' | 'lg';
		showTotal?: boolean;
	}

	let {
		commentableId,
		commentableType,
		reactionCounts = $bindable({
			[ReactionType.Like]: 0,
			[ReactionType.Love]: 0,
			[ReactionType.Laugh]: 0,
			[ReactionType.Wow]: 0,
			[ReactionType.Sad]: 0,
			[ReactionType.Angry]: 0,
			total: 0
		}),
		userReaction = $bindable(null),
		onReactionChange,
		size = 'md',
		showTotal = true
	}: ReactionsProps = $props();

	let isSubmitting = $state(false);
	let showAllReactions = $state(false);

	const reactions = [
		{ type: ReactionType.Like, emoji: '👍', label: 'Like' },
		{ type: ReactionType.Love, emoji: '❤️', label: 'Love' },
		{ type: ReactionType.Laugh, emoji: '😂', label: 'Laugh' },
		{ type: ReactionType.Wow, emoji: '😮', label: 'Wow' },
		{ type: ReactionType.Sad, emoji: '😢', label: 'Sad' },
		{ type: ReactionType.Angry, emoji: '😠', label: 'Angry' }
	];

	const sizeClasses = {
		sm: 'text-lg',
		md: 'text-2xl',
		lg: 'text-3xl'
	};

	// Optimistic update helper
	function optimisticUpdate(newReactionType: ReactionType | null) {
		const oldCounts = { ...reactionCounts };
		const oldUserReaction = userReaction;

		// Update counts optimistically
		if (oldUserReaction !== null && oldUserReaction !== undefined) {
			reactionCounts[oldUserReaction] = Math.max(0, reactionCounts[oldUserReaction] - 1);
			reactionCounts.total = Math.max(0, reactionCounts.total - 1);
		}

		if (newReactionType !== null) {
			reactionCounts[newReactionType] = (reactionCounts[newReactionType] || 0) + 1;
			reactionCounts.total = (reactionCounts.total || 0) + 1;
		}

		userReaction = newReactionType;

		// Notify parent component
		if (onReactionChange) {
			onReactionChange(reactionCounts, userReaction);
		}

		return { oldCounts, oldUserReaction };
	}

	// Rollback helper
	function rollback(oldCounts: ReactionCounts, oldUserReaction: ReactionType | null) {
		reactionCounts = oldCounts;
		userReaction = oldUserReaction;

		if (onReactionChange) {
			onReactionChange(reactionCounts, userReaction);
		}
	}

	async function handleReaction(reactionType: ReactionType) {
		if (!authStore.isAuthenticated) {
			toastStore.warning('Please login to react');
			return;
		}

		if (isSubmitting) return;

		isSubmitting = true;

		// Determine the new reaction state
		const isSameReaction = userReaction === reactionType;
		const newReactionType = isSameReaction ? null : reactionType;

		// Optimistic update
		const { oldCounts, oldUserReaction } = optimisticUpdate(newReactionType);

		try {
			// Make API call
			await reactionsApi.upsertReaction({
				commentableId,
				commentableType,
				reactionType
			});

			// Success - optimistic update was correct
			showAllReactions = false;
		} catch (error: any) {
			// Rollback optimistic update
			rollback(oldCounts, oldUserReaction);

			console.error('Reaction failed:', error);
			toastStore.error(error.message || 'Failed to update reaction');
		} finally {
			isSubmitting = false;
		}
	}

	// Get top 3 reactions to display
	const topReactions = $derived(() => {
		return reactions
			.map((r) => ({
				...r,
				count: reactionCounts[r.type] || 0
			}))
			.filter((r) => r.count > 0)
			.sort((a, b) => b.count - a.count)
			.slice(0, 3);
	});
</script>

<div class="inline-flex items-center gap-2">
	<!-- Reaction Button/Display -->
	<div
		class="relative"
		onmouseleave={() => setTimeout(() => (showAllReactions = false), 200)}
	>
		<button
			onclick={() => (showAllReactions = !showAllReactions)}
			onmouseenter={() => (showAllReactions = true)}
			onkeydown={(e) => {
				if (e.key === 'Enter' || e.key === ' ') {
					e.preventDefault();
					showAllReactions = !showAllReactions;
				} else if (e.key === 'Escape') {
					showAllReactions = false;
				}
			}}
			class={cn(
				'inline-flex items-center gap-2 px-3 py-2 rounded-full border transition-all',
				userReaction !== null
					? 'bg-blue-50 border-blue-300 dark:bg-blue-900 dark:border-blue-700'
					: 'bg-gray-50 border-gray-300 dark:bg-gray-800 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700',
				'disabled:opacity-50 disabled:cursor-not-allowed'
			)}
			disabled={isSubmitting}
			aria-label={userReaction !== null
				? `${reactions.find((r) => r.type === userReaction)?.label} reaction selected. ${reactionCounts.total} total reactions. Press to change reaction.`
				: `Add reaction. ${reactionCounts.total} total reactions.`}
			aria-expanded={showAllReactions}
			aria-haspopup="true"
			aria-controls="reaction-picker"
		>
			{#if userReaction !== null}
				<span class={sizeClasses[size]} role="img" aria-hidden="true">
					{reactions.find((r) => r.type === userReaction)?.emoji}
				</span>
			{:else}
				<span class={sizeClasses[size]} role="img" aria-hidden="true">👍</span>
			{/if}

			{#if showTotal && reactionCounts.total > 0}
				<span class="text-sm font-medium text-gray-700 dark:text-gray-300">
					{reactionCounts.total}
				</span>
			{/if}
		</button>

		<!-- Reaction Picker (Hover/Click) -->
		{#if showAllReactions}
			<div
				id="reaction-picker"
				role="toolbar"
				aria-label="Choose a reaction"
				class="absolute bottom-full left-0 mb-2 bg-white dark:bg-gray-800 rounded-lg shadow-xl border border-gray-200 dark:border-gray-700 p-2 flex gap-1 z-10"
			>
				{#each reactions as reaction, index}
					<button
						onclick={() => handleReaction(reaction.type)}
						onkeydown={(e) => {
							if (e.key === 'Enter' || e.key === ' ') {
								e.preventDefault();
								handleReaction(reaction.type);
							} else if (e.key === 'Escape') {
								showAllReactions = false;
							} else if (e.key === 'ArrowRight' && index < reactions.length - 1) {
								e.preventDefault();
								const nextButton = e.currentTarget.nextElementSibling as HTMLButtonElement;
								nextButton?.focus();
							} else if (e.key === 'ArrowLeft' && index > 0) {
								e.preventDefault();
								const prevButton = e.currentTarget.previousElementSibling as HTMLButtonElement;
								prevButton?.focus();
							}
						}}
						class={cn(
							'relative p-2 rounded-lg transition-all hover:scale-125 hover:bg-gray-100 dark:hover:bg-gray-700',
							userReaction === reaction.type && 'bg-blue-50 dark:bg-blue-900'
						)}
						aria-label={`React with ${reaction.label}${(reactionCounts[reaction.type] || 0) > 0 ? `. ${reactionCounts[reaction.type]} people reacted with this.` : ''}`}
						aria-pressed={userReaction === reaction.type}
						disabled={isSubmitting}
						tabindex={index === 0 ? 0 : -1}
					>
						<span class={sizeClasses[size]} role="img" aria-hidden="true">{reaction.emoji}</span>
						{#if (reactionCounts[reaction.type] || 0) > 0}
							<span class="absolute -top-1 -right-1 text-xs bg-blue-600 text-white rounded-full px-1.5 py-0.5 min-w-[1.25rem] text-center" aria-hidden="true">
								{reactionCounts[reaction.type]}
							</span>
						{/if}
					</button>
				{/each}
			</div>
		{/if}
	</div>

	<!-- Top Reactions Display -->
	{#if topReactions().length > 0 && !showAllReactions}
		<div class="flex items-center gap-1" role="group" aria-label="Top reactions summary">
			{#each topReactions() as reaction}
				<span class="text-sm text-gray-600 dark:text-gray-400">
					<span role="img" aria-label={reaction.label}>{reaction.emoji}</span>
					<span class="sr-only">{reaction.label}:</span> {reaction.count}
				</span>
			{/each}
		</div>
	{/if}
</div>
