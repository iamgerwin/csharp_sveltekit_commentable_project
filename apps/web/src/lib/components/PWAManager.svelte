<script lang="ts">
	import { browser } from '$app/environment';
	import { onMount } from 'svelte';
	import { toastStore } from '$lib/stores/toast.store.svelte';
	import PWAInstallPrompt from './PWAInstallPrompt.svelte';

	let updateAvailable = $state(false);
	let registration: ServiceWorkerRegistration | null = null;

	onMount(() => {
		if (!browser || !('serviceWorker' in navigator)) {
			return;
		}

		// Only register service worker in production
		// Development uses Vite which conflicts with service worker
		if (import.meta.env.PROD) {
			registerServiceWorker();
		}
	});

	async function registerServiceWorker() {
		try {
			// In development, service worker is built by SvelteKit
			// In production, it will be at the root
			registration = await navigator.serviceWorker.register('/service-worker.js', {
				type: 'module'
			});

			console.log('Service Worker registered successfully');

			// Check for updates
			registration.addEventListener('updatefound', () => {
				const newWorker = registration?.installing;

				newWorker?.addEventListener('statechange', () => {
					if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
						updateAvailable = true;
						toastStore.info('A new version is available. Refresh to update.');
					}
				});
			});

			// Check for updates periodically (every hour)
			setInterval(
				() => {
					registration?.update();
				},
				60 * 60 * 1000
			);
		} catch (error) {
			console.error('Service Worker registration failed:', error);
		}
	}

	function handleUpdate() {
		if (registration?.waiting) {
			registration.waiting.postMessage({ type: 'SKIP_WAITING' });
			window.location.reload();
		}
	}
</script>

<PWAInstallPrompt />

{#if updateAvailable}
	<div
		class="fixed top-4 left-1/2 transform -translate-x-1/2 z-50 bg-blue-600 text-white rounded-lg shadow-lg p-4 max-w-md"
	>
		<div class="flex items-center justify-between gap-4">
			<p class="text-sm font-medium">A new version is available!</p>
			<button
				onclick={handleUpdate}
				class="px-4 py-2 bg-white text-blue-600 rounded-md text-sm font-medium hover:bg-gray-100 transition-colors"
			>
				Update Now
			</button>
		</div>
	</div>
{/if}
