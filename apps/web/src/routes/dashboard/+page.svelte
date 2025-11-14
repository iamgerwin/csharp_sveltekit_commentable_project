<script lang="ts">
	import { onMount } from 'svelte';
	import { t } from 'svelte-i18n';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { statsApi } from '$lib/api/stats.api';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import type { Stats } from '$lib/types/entities.types';

	let stats = $state<Stats | null>(null);
	let isLoading = $state(true);

	const quickActions = [
		{
			title: 'Create Video',
			description: 'Upload a new video to share',
			icon: '🎥',
			href: '/dashboard/videos/new',
			color: 'bg-blue-500'
		},
		{
			title: 'Create Post',
			description: 'Write a new post',
			icon: '📝',
			href: '/dashboard/posts/new',
			color: 'bg-green-500'
		},
		{
			title: 'View Videos',
			description: 'Browse all videos',
			icon: '🎬',
			href: '/dashboard/videos',
			color: 'bg-purple-500'
		},
		{
			title: 'View Posts',
			description: 'Browse all posts',
			icon: '📚',
			href: '/dashboard/posts',
			color: 'bg-orange-500'
		}
	];

	async function loadStats() {
		try {
			const response = await statsApi.getStats();

			// Handle undefined or invalid response
			if (!response) {
				console.warn('Stats API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				stats = null;
				return;
			}

			stats = response;
		} catch (error: any) {
			console.error('Failed to load stats:', error);
			toastStore.error(error.message || 'Failed to load statistics');
		} finally {
			isLoading = false;
		}
	}

	onMount(() => {
		loadStats();
	});
</script>

<div class="space-y-8">
	<!-- Welcome Section -->
	<div class="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-lg shadow-lg p-8 text-white">
		<h1 class="text-3xl font-bold mb-2">
			{$t('dashboard.welcome', { values: { username: authStore.user?.username } })}
		</h1>
		<p class="text-blue-100">
			{$t('dashboard.subtitle')}
		</p>
	</div>

	<!-- Stats Grid -->
	<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
		{#if isLoading}
			{#each Array(4) as _}
				<Card class="p-6 animate-pulse">
					<div class="h-20 bg-gray-200 dark:bg-gray-700 rounded"></div>
				</Card>
			{/each}
		{:else if stats}
			<Card class="p-6">
				<div class="flex items-center justify-between">
					<div>
						<p class="text-sm text-gray-600 dark:text-gray-400 mb-1">
							{$t('dashboard.stats.totalVideos')}
						</p>
						<p class="text-3xl font-bold text-gray-900 dark:text-white">
							{stats.totalVideos}
						</p>
					</div>
					<div class="text-4xl">🎥</div>
				</div>
			</Card>

			<Card class="p-6">
				<div class="flex items-center justify-between">
					<div>
						<p class="text-sm text-gray-600 dark:text-gray-400 mb-1">
							{$t('dashboard.stats.totalPosts')}
						</p>
						<p class="text-3xl font-bold text-gray-900 dark:text-white">
							{stats.totalPosts}
						</p>
					</div>
					<div class="text-4xl">📝</div>
				</div>
			</Card>

			<Card class="p-6">
				<div class="flex items-center justify-between">
					<div>
						<p class="text-sm text-gray-600 dark:text-gray-400 mb-1">
							{$t('dashboard.stats.totalComments')}
						</p>
						<p class="text-3xl font-bold text-gray-900 dark:text-white">
							{stats.totalComments}
						</p>
					</div>
					<div class="text-4xl">💬</div>
				</div>
			</Card>

			<Card class="p-6">
				<div class="flex items-center justify-between">
					<div>
						<p class="text-sm text-gray-600 dark:text-gray-400 mb-1">
							{$t('dashboard.stats.totalReactions')}
						</p>
						<p class="text-3xl font-bold text-gray-900 dark:text-white">
							{stats.totalReactions}
						</p>
					</div>
					<div class="text-4xl">❤️</div>
				</div>
			</Card>
		{/if}
	</div>

	<!-- Quick Actions -->
	<div>
		<h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
			Quick Actions
		</h2>
		<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
			{#each quickActions as action}
				<a href={action.href}>
					<Card class="p-6 hover:shadow-lg transition-shadow cursor-pointer h-full">
						<div class="flex flex-col items-center text-center">
							<div class="w-16 h-16 {action.color} rounded-full flex items-center justify-center text-3xl mb-4">
								{action.icon}
							</div>
							<h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">
								{action.title}
							</h3>
							<p class="text-sm text-gray-600 dark:text-gray-400">
								{action.description}
							</p>
						</div>
					</Card>
				</a>
			{/each}
		</div>
	</div>

	<!-- Recent Activity (Placeholder) -->
	<div>
		<h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
			Recent Activity
		</h2>
		<Card class="p-8 text-center">
			<div class="text-6xl mb-4">📊</div>
			<p class="text-gray-600 dark:text-gray-400">
				No recent activity yet. Start by creating your first video or post!
			</p>
			<div class="mt-6 flex gap-4 justify-center">
				<Button onclick={() => window.location.href = '/dashboard/videos/new'}>
					Create Video
				</Button>
				<Button variant="outline" onclick={() => window.location.href = '/dashboard/posts/new'}>
					Create Post
				</Button>
			</div>
		</Card>
	</div>

	{#if authStore.isModerator()}
		<!-- Moderator Section -->
		<div>
			<h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
				Moderation
			</h2>
			<div class="grid grid-cols-1 md:grid-cols-2 gap-6">
				{#if isLoading}
					{#each Array(2) as _}
						<Card class="p-6 animate-pulse">
							<div class="h-32 bg-gray-200 dark:bg-gray-700 rounded"></div>
						</Card>
					{/each}
				{:else if stats}
					<Card class="p-6">
						<div class="flex items-center justify-between">
							<div>
								<h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">
									Pending Reports
								</h3>
								<p class="text-3xl font-bold text-yellow-600">{stats.pendingReports}</p>
							</div>
							<div class="text-4xl">🚩</div>
						</div>
						<Button class="w-full mt-4" variant="outline" onclick={() => window.location.href = '/dashboard/reports'}>
							View Reports
						</Button>
					</Card>

					<Card class="p-6">
						<div class="flex items-center justify-between">
							<div>
								<h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">
									Flagged Content
								</h3>
								<p class="text-3xl font-bold text-red-600">{stats.flaggedContent}</p>
							</div>
							<div class="text-4xl">⚠️</div>
						</div>
						<Button class="w-full mt-4" variant="outline" onclick={() => window.location.href = '/dashboard/moderation'}>
							View Flagged
						</Button>
					</Card>
				{/if}
			</div>
		</div>
	{/if}
</div>
