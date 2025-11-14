<script lang="ts">
	import { onMount } from 'svelte';
	import { locale, waitLocale } from 'svelte-i18n';
	import { initI18n } from '$lib/i18n';
	import favicon from '$lib/assets/favicon.svg';
	import Toast from '$lib/components/ui/toast.svelte';
	import CookieConsent from '$lib/components/gdpr/CookieConsent.svelte';
	import PWAManager from '$lib/components/PWAManager.svelte';
	import '../app.css';

	let { children } = $props();
	let isDark = $state(false);
	let i18nReady = $state(false);

	// Initialize i18n
	initI18n();
	waitLocale().then(() => {
		i18nReady = true;
	});

	onMount(() => {
		// Check for saved theme preference or default to system preference
		const savedTheme = localStorage.getItem('theme');
		const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

		isDark = savedTheme === 'dark' || (!savedTheme && prefersDark);

		// Apply dark class to document element
		if (isDark) {
			document.documentElement.classList.add('dark');
		} else {
			document.documentElement.classList.remove('dark');
		}

		// Listen for system theme changes
		const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
		const handleChange = (e: MediaQueryListEvent) => {
			if (!localStorage.getItem('theme')) {
				isDark = e.matches;
				if (isDark) {
					document.documentElement.classList.add('dark');
				} else {
					document.documentElement.classList.remove('dark');
				}
			}
		};

		mediaQuery.addEventListener('change', handleChange);
		return () => mediaQuery.removeEventListener('change', handleChange);
	});
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<script>
		// Prevent flash of unstyled content by applying theme immediately
		if (localStorage.getItem('theme') === 'dark' || (!localStorage.getItem('theme') && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
			document.documentElement.classList.add('dark');
		}
	</script>
</svelte:head>

<div class="min-h-screen bg-background font-sans antialiased" lang={$locale || 'en'}>
	{#if i18nReady}
		<!-- Skip to main content link for accessibility -->
		<a href="#main-content" class="skip-link">
			Skip to main content
		</a>

		{@render children()}
		<Toast />
		<CookieConsent />
		<PWAManager />
	{:else}
		<div class="flex items-center justify-center min-h-screen">
			<div class="text-center">
				<div class="text-4xl mb-4">⏳</div>
				<p class="text-gray-600 dark:text-gray-400">Loading...</p>
			</div>
		</div>
	{/if}
</div>
