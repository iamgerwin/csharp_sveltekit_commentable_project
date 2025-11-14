<script lang="ts">
	import { goto } from '$app/navigation';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Label from '$lib/components/ui/label.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import type { LoginRequest } from '$lib/types/auth.types';

	let formData = $state<LoginRequest>({
		email: '',
		password: ''
	});

	let errors = $state<Partial<LoginRequest>>({});
	let isSubmitting = $state(false);

	function validateForm(): boolean {
		errors = {};
		let isValid = true;

		if (!formData.email) {
			errors.email = 'Email is required';
			isValid = false;
		} else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
			errors.email = 'Invalid email format';
			isValid = false;
		}

		if (!formData.password) {
			errors.password = 'Password is required';
			isValid = false;
		} else if (formData.password.length < 6) {
			errors.password = 'Password must be at least 6 characters';
			isValid = false;
		}

		return isValid;
	}

	async function handleSubmit(e: SubmitEvent) {
		e.preventDefault();

		if (!validateForm()) {
			return;
		}

		isSubmitting = true;

		try {
			const success = await authStore.login(formData);

			if (success) {
				toastStore.success('Login successful! Welcome back.');
				goto('/dashboard');
			} else {
				toastStore.error(authStore.error || 'Login failed. Please try again.');
			}
		} catch (error: any) {
			toastStore.error(error.message || 'An unexpected error occurred');
		} finally {
			isSubmitting = false;
		}
	}
</script>

<svelte:head>
	<title>Login - Commentable</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 p-4">
	<Card class="w-full max-w-md p-8">
		<div class="text-center mb-8">
			<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">Welcome Back</h1>
			<p class="text-gray-600 dark:text-gray-400">Sign in to your account</p>
		</div>

		<form onsubmit={handleSubmit} class="space-y-6">
			<div class="space-y-2">
				<Label for="email">Email</Label>
				<Input
					id="email"
					name="email"
					type="email"
					placeholder="you@example.com"
					bind:value={formData.email}
					disabled={isSubmitting}
					required
				/>
				{#if errors.email}
					<p class="text-sm text-red-500">{errors.email}</p>
				{/if}
			</div>

			<div class="space-y-2">
				<Label for="password">Password</Label>
				<Input
					id="password"
					name="password"
					type="password"
					placeholder="••••••••"
					bind:value={formData.password}
					disabled={isSubmitting}
					required
				/>
				{#if errors.password}
					<p class="text-sm text-red-500">{errors.password}</p>
				{/if}
			</div>

			<Button type="submit" class="w-full" disabled={isSubmitting}>
				{#if isSubmitting}
					<span class="flex items-center justify-center gap-2">
						<span class="animate-spin">⏳</span>
						Signing in...
					</span>
				{:else}
					Sign In
				{/if}
			</Button>
		</form>

		<div class="mt-6 text-center">
			<p class="text-sm text-gray-600 dark:text-gray-400">
				Don't have an account?
				<a href="/register" class="text-blue-600 hover:text-blue-500 font-medium">
					Sign up
				</a>
			</p>
		</div>

		<div class="mt-4 text-center">
			<a href="/" class="text-sm text-gray-600 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200">
				← Back to home
			</a>
		</div>
	</Card>
</div>
