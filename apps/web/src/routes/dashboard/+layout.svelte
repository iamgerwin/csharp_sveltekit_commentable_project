<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import { authStore } from '$lib/stores/auth.store.svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import Button from '$lib/components/ui/button.svelte';
	import LanguageSwitcher from '$lib/components/LanguageSwitcher.svelte';
	import { onMount } from 'svelte';

	let { children } = $props();

	let isMobileMenuOpen = $state(false);

	onMount(() => {
		// Redirect to login if not authenticated
		if (!authStore.isAuthenticated) {
			toastStore.warning('Please login to access the dashboard');
			goto('/login');
		}
	});

	async function handleLogout() {
		await authStore.logout();
		toastStore.success('Logged out successfully');
	}

	function isActive(path: string): boolean {
		return $page.url.pathname === path || $page.url.pathname.startsWith(path + '/');
	}

	const navItems = [
		{ label: 'Dashboard', path: '/dashboard', icon: '📊' },
		{ label: 'Videos', path: '/dashboard/videos', icon: '🎥' },
		{ label: 'Posts', path: '/dashboard/posts', icon: '📝' },
		{ label: 'My Content', path: '/dashboard/my-content', icon: '📁' }
	];

	const moderatorNavItems = [
		{ label: 'Reports', path: '/dashboard/reports', icon: '🚩' },
		{ label: 'Moderation', path: '/dashboard/moderation', icon: '🛡️' }
	];
</script>

<svelte:head>
	<title>Dashboard - Commentable</title>
</svelte:head>

{#if authStore.isAuthenticated}
	<div class="min-h-screen bg-gray-50 dark:bg-gray-900">
		<!-- Top Navigation Bar -->
		<nav class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
			<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
				<div class="flex justify-between h-16">
					<div class="flex">
						<!-- Logo -->
						<div class="flex-shrink-0 flex items-center">
							<a href="/dashboard" class="text-2xl font-bold text-blue-600">
								Commentable
							</a>
						</div>

						<!-- Desktop Navigation -->
						<div class="hidden sm:ml-8 sm:flex sm:space-x-4">
							{#each navItems as item}
								<a
									href={item.path}
									class="inline-flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors {isActive(
										item.path
									)
										? 'bg-blue-50 text-blue-700 dark:bg-blue-900 dark:text-blue-200'
										: 'text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'}"
								>
									<span class="mr-2">{item.icon}</span>
									{item.label}
								</a>
							{/each}

							{#if authStore.isModerator()}
								{#each moderatorNavItems as item}
									<a
										href={item.path}
										class="inline-flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors {isActive(
											item.path
										)
											? 'bg-blue-50 text-blue-700 dark:bg-blue-900 dark:text-blue-200'
											: 'text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'}"
									>
										<span class="mr-2">{item.icon}</span>
										{item.label}
									</a>
								{/each}
							{/if}
						</div>
					</div>

					<!-- User Menu -->
					<div class="hidden sm:ml-6 sm:flex sm:items-center gap-4">
						<LanguageSwitcher />
						<div class="text-sm text-gray-700 dark:text-gray-300">
							<span class="font-medium">{authStore.user?.username}</span>
							{#if authStore.isAdmin()}
								<span class="ml-2 px-2 py-1 text-xs bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-200 rounded">
									Admin
								</span>
							{:else if authStore.isModerator()}
								<span class="ml-2 px-2 py-1 text-xs bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-200 rounded">
									Moderator
								</span>
							{/if}
						</div>
						<Button variant="outline" size="sm" onclick={handleLogout}>
							Logout
						</Button>
					</div>

					<!-- Mobile menu button -->
					<div class="flex items-center sm:hidden">
						<button
							onclick={() => (isMobileMenuOpen = !isMobileMenuOpen)}
							class="inline-flex items-center justify-center p-2 rounded-md text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
						>
							<span class="sr-only">Open main menu</span>
							{#if isMobileMenuOpen}
								✕
							{:else}
								☰
							{/if}
						</button>
					</div>
				</div>
			</div>

			<!-- Mobile Navigation Menu -->
			{#if isMobileMenuOpen}
				<div class="sm:hidden border-t border-gray-200 dark:border-gray-700">
					<div class="px-2 pt-2 pb-3 space-y-1">
						{#each navItems as item}
							<a
								href={item.path}
								class="block px-3 py-2 rounded-md text-base font-medium {isActive(item.path)
									? 'bg-blue-50 text-blue-700 dark:bg-blue-900 dark:text-blue-200'
									: 'text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'}"
								onclick={() => (isMobileMenuOpen = false)}
							>
								<span class="mr-2">{item.icon}</span>
								{item.label}
							</a>
						{/each}

						{#if authStore.isModerator()}
							{#each moderatorNavItems as item}
								<a
									href={item.path}
									class="block px-3 py-2 rounded-md text-base font-medium {isActive(item.path)
										? 'bg-blue-50 text-blue-700 dark:bg-blue-900 dark:text-blue-200'
										: 'text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'}"
									onclick={() => (isMobileMenuOpen = false)}
								>
									<span class="mr-2">{item.icon}</span>
									{item.label}
								</a>
							{/each}
						{/if}
					</div>

					<div class="pt-4 pb-3 border-t border-gray-200 dark:border-gray-700">
						<div class="px-4 mb-3">
							<div class="text-base font-medium text-gray-800 dark:text-gray-200">
								{authStore.user?.username}
							</div>
							<div class="text-sm text-gray-500 dark:text-gray-400">
								{authStore.user?.email}
							</div>
						</div>
						<div class="px-3 mb-3">
							<LanguageSwitcher />
						</div>
						<div class="px-3">
							<Button variant="outline" class="w-full" onclick={handleLogout}>
								Logout
							</Button>
						</div>
					</div>
				</div>
			{/if}
		</nav>

		<!-- Main Content -->
		<main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
			{@render children()}
		</main>
	</div>
{:else}
	<div class="min-h-screen flex items-center justify-center">
		<div class="text-center">
			<div class="animate-spin text-6xl mb-4">⏳</div>
			<p class="text-gray-600 dark:text-gray-400">Loading...</p>
		</div>
	</div>
{/if}
