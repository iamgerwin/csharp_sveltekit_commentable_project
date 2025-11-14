<script lang="ts">
	import { goto } from '$app/navigation';
	import { videosApi } from '$lib/api/videos.api';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Label from '$lib/components/ui/label.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import type { CreateVideoRequest } from '$lib/types/entities.types';

	let formData = $state<CreateVideoRequest>({
		title: '',
		description: '',
		videoUrl: '',
		thumbnailUrl: '',
		duration: undefined
	});

	let durationStr = $state('');
	let errors = $state<Partial<CreateVideoRequest>>({});
	let isSubmitting = $state(false);

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
			const video = await videosApi.createVideo(formData);
			toastStore.success('Video uploaded successfully!');
			goto(`/dashboard/videos/${video.id}`);
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to upload video');
		} finally {
			isSubmitting = false;
		}
	}
</script>

<svelte:head>
	<title>Upload Video - Commentable</title>
</svelte:head>

<div class="max-w-3xl mx-auto space-y-6">
	<!-- Back Button -->
	<Button variant="outline" onclick={() => goto('/dashboard/videos')}>
		← Back to Videos
	</Button>

	<div>
		<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
			Upload Video
		</h1>
		<p class="text-gray-600 dark:text-gray-400">
			Share your video with the community
		</p>
	</div>

	<Card class="p-6">
		<form onsubmit={handleSubmit} class="space-y-6">
			<!-- Title -->
			<div class="space-y-2">
				<Label for="title">Title *</Label>
				<Input
					id="title"
					name="title"
					placeholder="Enter video title"
					bind:value={formData.title}
					disabled={isSubmitting}
					required
				/>
				{#if errors.title}
					<p class="text-sm text-red-500">{errors.title}</p>
				{/if}
			</div>

			<!-- Description -->
			<div class="space-y-2">
				<Label for="description">Description *</Label>
				<Textarea
					id="description"
					name="description"
					placeholder="Describe your video"
					bind:value={formData.description}
					rows={5}
					disabled={isSubmitting}
					required
				/>
				{#if errors.description}
					<p class="text-sm text-red-500">{errors.description}</p>
				{/if}
			</div>

			<!-- Video URL -->
			<div class="space-y-2">
				<Label for="videoUrl">Video URL *</Label>
				<Input
					id="videoUrl"
					name="videoUrl"
					type="url"
					placeholder="https://example.com/video.mp4"
					bind:value={formData.videoUrl}
					disabled={isSubmitting}
					required
				/>
				{#if errors.videoUrl}
					<p class="text-sm text-red-500">{errors.videoUrl}</p>
				{/if}
				<p class="text-sm text-gray-500 dark:text-gray-400">
					Enter the direct URL to your video file
				</p>
			</div>

			<!-- Thumbnail URL -->
			<div class="space-y-2">
				<Label for="thumbnailUrl">Thumbnail URL (Optional)</Label>
				<Input
					id="thumbnailUrl"
					name="thumbnailUrl"
					type="url"
					placeholder="https://example.com/thumbnail.jpg"
					bind:value={formData.thumbnailUrl}
					disabled={isSubmitting}
				/>
				<p class="text-sm text-gray-500 dark:text-gray-400">
					Enter the URL to a thumbnail image for your video
				</p>
			</div>

			<!-- Duration -->
			<div class="space-y-2">
				<Label for="duration">Duration (seconds)</Label>
				<Input
					id="duration"
					name="duration"
					type="number"
					placeholder="120"
					bind:value={durationStr}
					disabled={isSubmitting}
					min="1"
				/>
				<p class="text-sm text-gray-500 dark:text-gray-400">
					Enter the video duration in seconds
				</p>
			</div>

			<!-- Actions -->
			<div class="flex gap-4 pt-4">
				<Button type="submit" disabled={isSubmitting} class="flex-1">
					{#if isSubmitting}
						<span class="flex items-center justify-center gap-2">
							<span class="animate-spin">⏳</span>
							Uploading...
						</span>
					{:else}
						Upload Video
					{/if}
				</Button>
				<Button
					type="button"
					variant="outline"
					onclick={() => goto('/dashboard/videos')}
					disabled={isSubmitting}
				>
					Cancel
				</Button>
			</div>
		</form>
	</Card>
</div>
