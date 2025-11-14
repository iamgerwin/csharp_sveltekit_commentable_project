<script lang="ts">
	import { goto } from '$app/navigation';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import Input from '$lib/components/ui/input.svelte';
	import Label from '$lib/components/ui/label.svelte';
	import Card from '$lib/components/ui/card.svelte';
	import type { RegisterRequest } from '$lib/types/auth.types';

	let formData = $state<RegisterRequest>({
		username: '',
		email: '',
		password: '',
		confirmPassword: ''
	});

	let errors = $state<Partial<RegisterRequest>>({});
	let isSubmitting = $state(false);

	function validateForm(): boolean {
		errors = {};
		let isValid = true;

		if (!formData.username) {
			errors.username = 'Username is required';
			isValid = false;
		} else if (formData.username.length < 3) {
			errors.username = 'Username must be at least 3 characters';
			isValid = false;
		}

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
		} else if (formData.password.length < 8) {
			errors.password = 'Password must be at least 8 characters';
			isValid = false;
		}

		if (!formData.confirmPassword) {
			errors.confirmPassword = 'Please confirm your password';
			isValid = false;
		} else if (formData.password !== formData.confirmPassword) {
			errors.confirmPassword = 'Passwords do not match';
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
			const success = await authStore.register(formData);

			if (success) {
				toastStore.success('Registration successful! Welcome to Commentable.');
				goto('/dashboard');
			} else {
				toastStore.error(authStore.error || 'Registration failed. Please try again.');
			}
		} catch (error: any) {
			toastStore.error(error.message || 'An unexpected error occurred');
		} finally {
			isSubmitting = false;
		}
	}
</script>

<svelte:head>
	<title>Register - Commentable</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 p-4">
	<Card class="w-full max-w-md p-8">
		<div class="text-center mb-8">
			<h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">Create Account</h1>
			<p class="text-gray-600 dark:text-gray-400">Join Commentable today</p>
		</div>

		<form onsubmit={handleSubmit} class="space-y-6">
			<div class="space-y-2">
				<Label for="username">Username</Label>
				<Input
					id="username"
					name="username"
					type="text"
					placeholder="johndoe"
					bind:value={formData.username}
					disabled={isSubmitting}
					required
				/>
				{#if errors.username}
					<p class="text-sm text-red-500">{errors.username}</p>
				{/if}
			</div>

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

			<div class="space-y-2">
				<Label for="confirmPassword">Confirm Password</Label>
				<Input
					id="confirmPassword"
					name="confirmPassword"
					type="password"
					placeholder="••••••••"
					bind:value={formData.confirmPassword}
					disabled={isSubmitting}
					required
				/>
				{#if errors.confirmPassword}
					<p class="text-sm text-red-500">{errors.confirmPassword}</p>
				{/if}
			</div>

			<Button type="submit" class="w-full" disabled={isSubmitting}>
				{#if isSubmitting}
					<span class="flex items-center justify-center gap-2">
						<span class="animate-spin">⏳</span>
						Creating account...
					</span>
				{:else}
					Create Account
				{/if}
			</Button>
		</form>

		<div class="mt-6 text-center">
			<p class="text-sm text-gray-600 dark:text-gray-400">
				Already have an account?
				<a href="/login" class="text-blue-600 hover:text-blue-500 font-medium">
					Sign in
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
