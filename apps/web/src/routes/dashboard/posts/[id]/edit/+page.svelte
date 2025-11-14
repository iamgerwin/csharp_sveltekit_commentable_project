<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { postsApi } from '$lib/api/posts.api';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Label from '$lib/components/ui/label.svelte';
	import Textarea from '$lib/components/ui/textarea.svelte';
	import type { Post, UpdatePostRequest } from '$lib/types/entities.types';

	let postId = $derived($page.params.id as string);
	let post = $state<Post | null>(null);
	let isLoading = $state(true);

	let formData = $state<UpdatePostRequest>({
		title: '',
		content: '',
		tags: []
	});

	let tagsInput = $state('');
	let errors = $state<Partial<UpdatePostRequest>>({});
	let isSubmitting = $state(false);

	async function loadPost() {
		isLoading = true;
		try {
			const response = await postsApi.getPost(postId);

			if (!response) {
				console.warn('API returned empty or invalid response');
				toastStore.error('API is not available. Please ensure the backend is running.');
				goto('/dashboard/posts');
				return;
			}

			post = response;

			if (post.userId !== authStore.user?.id && !authStore.isModerator()) {
				toastStore.error('You do not have permission to edit this post');
				goto(`/dashboard/posts/${postId}`);
				return;
			}

			formData.title = post.title;
			formData.content = post.content;
			formData.tags = post.tags || [];
			tagsInput = (post.tags || []).join(', ');
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to load post');
			goto('/dashboard/posts');
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
			await postsApi.updatePost(postId, formData);
			toastStore.success('Post updated successfully!');
			goto(`/dashboard/posts/${postId}`);
		} catch (error: any) {
			toastStore.error(error.message || 'Failed to update post');
		} finally {
			isSubmitting = false;
		}
	}

	onMount(() => loadPost());
</script>

<svelte:head>
	<title>Edit Post - Commentable</title>
</svelte:head>

<div class="max-w-3xl mx-auto space-y-6">
	<Button variant="outline" onclick={() => goto(`/dashboard/posts/${postId}`)}>
		← Back to Post
	</Button>

	{#if isLoading}
		<Card class="p-6">
			<div class="animate-pulse space-y-4">
				{#each Array(3) as _}
					<div class="h-10 bg-gray-200 dark:bg-gray-700 rounded"></div>
				{/each}
			</div>
		</Card>
	{:else}
		<div>
			<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">Edit Post</h1>
			<p class="text-gray-600 dark:text-gray-400">Update your post</p>
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
					<Label for="content">Content *</Label>
					<Textarea
						id="content"
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
					<Label for="tags">Tags</Label>
					<Input
						id="tags"
						placeholder="tech, programming, javascript"
						bind:value={tagsInput}
						disabled={isSubmitting}
					/>
				</div>

				<div class="flex gap-4 pt-4">
					<Button type="submit" disabled={isSubmitting} class="flex-1">
						{isSubmitting ? 'Saving...' : 'Save Changes'}
					</Button>
					<Button
						type="button"
						variant="outline"
						onclick={() => goto(`/dashboard/posts/${postId}`)}
						disabled={isSubmitting}
					>
						Cancel
					</Button>
				</div>
			</form>
		</Card>
	{/if}
</div>
