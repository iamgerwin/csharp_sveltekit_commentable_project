<script lang="ts">
	import { browser } from '$app/environment';
	import { onMount } from 'svelte';
	import Button from './ui/button.svelte';

	let deferredPrompt: any = null;
	let showInstallPrompt = $state(false);
	let isInstalled = $state(false);

	onMount(() => {
		if (!browser) return;

		// Check if already installed
		if (window.matchMedia('(display-mode: standalone)').matches) {
			isInstalled = true;
			return;
		}

		// Check if user has dismissed the prompt before
		const dismissed = localStorage.getItem('pwa-install-dismissed');
		if (dismissed) {
			const dismissedDate = new Date(dismissed);
			const daysSinceDismissed = (Date.now() - dismissedDate.getTime()) / (1000 * 60 * 60 * 24);

			// Don't show prompt again for 30 days after dismissal
			if (daysSinceDismissed < 30) {
				return;
			}
		}

		// Listen for beforeinstallprompt event
		window.addEventListener('beforeinstallprompt', (e) => {
			e.preventDefault();
			deferredPrompt = e;

			// Show prompt after a short delay (better UX)
			setTimeout(() => {
				showInstallPrompt = true;
			}, 5000);
		});

		// Listen for app installed event
		window.addEventListener('appinstalled', () => {
			isInstalled = true;
			showInstallPrompt = false;
			deferredPrompt = null;
		});
	});

	async function handleInstall() {
		if (!deferredPrompt) return;

		deferredPrompt.prompt();
		const { outcome } = await deferredPrompt.userChoice;

		if (outcome === 'accepted') {
			console.log('PWA installed successfully');
		}

		deferredPrompt = null;
		showInstallPrompt = false;
	}

	function handleDismiss() {
		showInstallPrompt = false;
		if (browser) {
			localStorage.setItem('pwa-install-dismissed', new Date().toISOString());
		}
	}
</script>

{#if showInstallPrompt && !isInstalled}
	<div
		class="fixed bottom-4 left-4 right-4 md:left-auto md:right-4 md:max-w-md z-50 bg-white dark:bg-gray-800 rounded-lg shadow-2xl border border-gray-200 dark:border-gray-700 p-4 animate-slide-up"
	>
		<div class="flex items-start gap-3">
			<div class="flex-shrink-0">
				<img src="/icons/icon-96x96.png" alt="App Icon" class="w-12 h-12 rounded-lg" />
			</div>
			<div class="flex-1 min-w-0">
				<h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-1">
					Install Commentable
				</h3>
				<p class="text-sm text-gray-600 dark:text-gray-400 mb-3">
					Install our app for a better experience with offline access and faster loading.
				</p>
				<div class="flex gap-2">
					<Button onclick={handleInstall} size="sm" class="flex-1">
						Install
					</Button>
					<Button onclick={handleDismiss} variant="outline" size="sm">
						Not now
					</Button>
				</div>
			</div>
			<button
				onclick={handleDismiss}
				class="flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
				aria-label="Close"
			>
				✕
			</button>
		</div>
	</div>
{/if}

<style>
	@keyframes slide-up {
		from {
			transform: translateY(100%);
			opacity: 0;
		}
		to {
			transform: translateY(0);
			opacity: 1;
		}
	}

	.animate-slide-up {
		animation: slide-up 0.3s ease-out;
	}
</style>
