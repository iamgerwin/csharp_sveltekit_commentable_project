<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { videosApi } from '$lib/api/videos.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Label from '$lib/components/ui/label.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import type { Video, UpdateVideoRequest } from '$lib/types/entities.types';

	let videoId = $derived($page.params.id as string);
	let video = $state<Video | null>(null);
	let isLoading = $state(true);

	let formData = $state<UpdateVideoRequest>({
		title: '',
		description: '',
		videoUrl: '',
		thumbnailUrl: '',
		duration: undefined
	});

	let durationStr = $state('');
	let errors = $state<Partial<UpdateVideoRequest>>({});
	let isSubmitting = $state(false);

	async function loadVideo() {
		isLoading = true;
		try {
			const response = await videosApi.getVideo(videoId);

			if (!response) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				goto('/dashboard/videos');
				return;
			}

			video = response;

			// Check permissions
			if (video.userId !== authStore.user?.id && !authStore.isModerator()) {
				toastStore.error('You do not have permission to edit this video');
				goto(`/dashboard/videos/${videoId}`);
				return;
			}

			// Populate form
			formData.title = video.title;
			formData.description = video.description;
			formData.videoUrl = video.videoUrl;
			formData.thumbnailUrl = video.thumbnailUrl || '';
			formData.duration = video.duration;
			durationStr = video.duration ? String(video.duration) : '';
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load video');
			goto('/dashboard/videos');
		} finally {
			isLoading = false;
		}
	}

	function validateForm(): boolean {
		errors = {};
		let isValid = true;

		if (!formData.title?.trim()) {
			errors.title = 'Title is required';
			isValid = false;
		}

		if (!formData.description?.trim()) {
			errors.description = 'Description is required';
			isValid = false;
		}

		if (!formData.videoUrl?.trim()) {
			errors.videoUrl = 'Video URL is required';
			isValid = false;
		}

		return isValid;
	}

	async function handleSubmit(e: SubmitEvent) {
		e.preventDefault();

		// Convert duration string to number
		if (durationStr) {
			formData.duration = parseInt(durationStr, 10);
		}

		if (!validateForm()) {
			return;
		}

		isSubmitting = true;

		try {
			await videosApi.updateVideo(videoId, formData);
			toastStore.success('Video updated successfully!');
			goto(`/dashboard/videos/${videoId}`);
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to update video');
		} finally {
			isSubmitting = false;
		}
	}

	onMount(() => {
		loadVideo();
	});
</script>

<svelte:head>
	<title>Edit Video - Commentable</title>
</svelte:head>

<div class="max-w-3xl mx-auto space-y-6">
	<Button variant="outline" onclick={() => goto(`/dashboard/videos/${videoId}`)}>
		← Back to Video
	</Button>

	{#if isLoading}
		<Card class="p-6">
			<div class="animate-pulse space-y-4">
				{#each Array(5) as _}
					<div class="h-10 bg-gray-200 dark:bg-gray-700 rounded"></div>
				{/each}
			</div>
		</Card>
	{:else}
		<div>
			<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
				Edit Video
			</h1>
			<p class="text-gray-600 dark:text-gray-400">
				Update your video information
			</p>
		</div>

		<Card class="p-6">
			<form onsubmit={handleSubmit} class="space-y-6">
				<div class="space-y-2">
					<Label for="title">Title *</Label>
					<Input
						id="title"
						bind:value={formData.title}
						disabled={isSubmitting}
						required
					/>
					{#if errors.title}
						<p class="text-sm text-red-500">{errors.title}</p>
					{/if}
				</div>

				<div class="space-y-2">
					<Label for="description">Description *</Label>
					<Textarea
						id="description"
						bind:value={formData.description}
						rows={5}
						disabled={isSubmitting}
						required
					/>
					{#if errors.description}
						<p class="text-sm text-red-500">{errors.description}</p>
					{/if}
				</div>

				<div class="space-y-2">
					<Label for="videoUrl">Video URL *</Label>
					<Input
						id="videoUrl"
						type="url"
						bind:value={formData.videoUrl}
						disabled={isSubmitting}
						required
					/>
					{#if errors.videoUrl}
						<p class="text-sm text-red-500">{errors.videoUrl}</p>
					{/if}
				</div>

				<div class="space-y-2">
					<Label for="thumbnailUrl">Thumbnail URL</Label>
					<Input
						id="thumbnailUrl"
						type="url"
						bind:value={formData.thumbnailUrl}
						disabled={isSubmitting}
					/>
				</div>

				<div class="space-y-2">
					<Label for="duration">Duration (seconds)</Label>
					<Input
						id="duration"
						type="number"
						bind:value={durationStr}
						disabled={isSubmitting}
						min="1"
					/>
				</div>

				<div class="flex gap-4 pt-4">
					<Button type="submit" disabled={isSubmitting} class="flex-1">
						{isSubmitting ? 'Saving...' : 'Save Changes'}
					</Button>
					<Button
						type="button"
						variant="outline"
						onclick={() => goto(`/dashboard/videos/${videoId}`)}
						disabled={isSubmitting}
					>
						Cancel
					</Button>
				</div>
			</form>
		</Card>
	{/if}
</div>
