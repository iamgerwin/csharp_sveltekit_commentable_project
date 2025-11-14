<script lang="ts">
	import { goto } from '$app/navigation';
	import { postsApi } from '$lib/api/posts.api';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Label from '$lib/components/ui/label.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import type { CreatePostRequest } from '$lib/types/entities.types';

	let formData = $state<CreatePostRequest>({
		title: '',
		content: '',
		tags: []
	});

	let tagsInput = $state('');
	let errors = $state<Partial<CreatePostRequest>>({});
	let isSubmitting = $state(false);

	function validateForm(): boolean {
		errors = {};
		let isValid = true;

		if (!formData.title?.trim()) {
			errors.title = 'Title is required';
			isValid = false;
		}

		if (!formData.content?.trim()) {
			errors.content = 'Content is required';
			isValid = false;
		}

		return isValid;
	}

	function parseTags() {
		formData.tags = tagsInput
			.split(',')
			.map((tag) => tag.trim())
			.filter((tag) => tag.length > 0);
	}

	async function handleSubmit(e: SubmitEvent) {
		e.preventDefault();

		parseTags();

		if (!validateForm()) {
			return;
		}

		isSubmitting = true;

		try {
			const post = await postsApi.createPost(formData);
			toastStore.success('Post created successfully!');
			goto(`/dashboard/posts/${post.id}`);
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to create post');
		} finally {
			isSubmitting = false;
		}
	}
</script>

<svelte:head>
	<title>Create Post - Commentable</title>
</svelte:head>

<div class="max-w-3xl mx-auto space-y-6">
	<Button variant="outline" onclick={() => goto('/dashboard/posts')}>
		← Back to Posts
	</Button>

	<div>
		<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">Create Post</h1>
		<p class="text-gray-600 dark:text-gray-400">Share your thoughts with the community</p>
	</div>

	<Card class="p-6">
		<form onsubmit={handleSubmit} class="space-y-6">
			<div class="space-y-2">
				<Label for="title">Title *</Label>
				<Input
					id="title"
					placeholder="Enter post title"
					bind:value={formData.title}
					disabled={isSubmitting}
					required
				/>
				{#if errors.title}
					<p class="text-sm text-red-500">{errors.title}</p>
				{/if}
			</div>

			<div class="space-y-2">
				<Label for="content">Content *</Label>
				<Textarea
					id="content"
					placeholder="Write your post content..."
					bind:value={formData.content}
					rows={12}
					disabled={isSubmitting}
					required
				/>
				{#if errors.content}
					<p class="text-sm text-red-500">{errors.content}</p>
				{/if}
			</div>

			<div class="space-y-2">
				<Label for="tags">Tags (Optional)</Label>
				<Input
					id="tags"
					placeholder="tech, programming, javascript (comma-separated)"
					bind:value={tagsInput}
					disabled={isSubmitting}
				/>
				<p class="text-sm text-gray-500 dark:text-gray-400">
					Enter tags separated by commas
				</p>
			</div>

			<div class="flex gap-4 pt-4">
				<Button type="submit" disabled={isSubmitting} class="flex-1">
					{#if isSubmitting}
						<span class="flex items-center justify-center gap-2">
							<span class="animate-spin">⏳</span>
							Creating...
						</span>
					{:else}
						Create Post
					{/if}
				</Button>
				<Button
					type="button"
					variant="outline"
					onclick={() => goto('/dashboard/posts')}
					disabled={isSubmitting}
				>
					Cancel
				</Button>
			</div>
		</form>
	</Card>
</div>
