<script lang="ts">
	import { onMount } from 'svelte';
	import { _ } from 'svelte-i18n';
	import Button from '$lib/components/ui/button.svelte';
	import Card from '$lib/components/ui/card.svelte';

	interface CookiePreferences {
		necessary: boolean;
		analytics: boolean;
		marketing: boolean;
		preferences: boolean;
	}

	let showBanner = $state(false);
	let showCustomize = $state(false);
	let cookiePreferences = $state<CookiePreferences>({
		necessary: true, // Always required
		analytics: false,
		marketing: false,
		preferences: false
	});

	onMount(() => {
		const consent = localStorage.getItem('cookieConsent');
		if (!consent) {
			showBanner = true;
		} else {
			try {
				cookiePreferences = JSON.parse(consent);
			} catch (e) {
				showBanner = true;
			}
		}
	});

	function acceptAll() {
		cookiePreferences = {
			necessary: true,
			analytics: true,
			marketing: true,
			preferences: true
		};
		saveConsent();
	}

	function rejectAll() {
		cookiePreferences = {
			necessary: true,
			analytics: false,
			marketing: false,
			preferences: false
		};
		saveConsent();
	}

	function saveConsent() {
		localStorage.setItem('cookieConsent', JSON.stringify(cookiePreferences));
		localStorage.setItem('cookieConsentDate', new Date().toISOString());
		showBanner = false;
		showCustomize = false;
	}

	function toggleCustomize() {
		showCustomize = !showCustomize;
	}
</script>

{#if showBanner}
	<div
		class="fixed bottom-0 left-0 right-0 z-50 p-4 bg-black/50 backdrop-blur-sm"
		role="dialog"
		aria-labelledby="cookie-consent-title"
		aria-describedby="cookie-consent-description"
	>
		<Card class="max-w-4xl mx-auto p-6">
			<h2 id="cookie-consent-title" class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
				{$_('gdpr.cookieConsent.title')}
			</h2>

			{#if !showCustomize}
				<p id="cookie-consent-description" class="text-gray-600 dark:text-gray-400 mb-6">
					{$_('gdpr.cookieConsent.message')}
					<a href="/privacy" class="text-primary hover:underline">
						{$_('gdpr.cookieConsent.privacyPolicy')}
					</a>.
				</p>

				<div class="flex flex-col sm:flex-row gap-3">
					<Button onclick={acceptAll} aria-label={$_('gdpr.cookieConsent.acceptAll')}>
						{$_('gdpr.cookieConsent.acceptAll')}
					</Button>
					<Button variant="outline" onclick={rejectAll} aria-label={$_('gdpr.cookieConsent.rejectAll')}>
						{$_('gdpr.cookieConsent.rejectAll')}
					</Button>
					<Button variant="ghost" onclick={toggleCustomize} aria-label={$_('gdpr.cookieConsent.customize')}>
						{$_('gdpr.cookieConsent.customize')}
					</Button>
				</div>
			{:else}
				<div class="space-y-4 mb-6">
					<!-- Necessary Cookies (Always Required) -->
					<div class="flex items-start justify-between p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
						<div class="flex-1">
							<h3 class="font-semibold text-gray-900 dark:text-white mb-1">
								{$_('gdpr.cookieConsent.necessary')}
							</h3>
							<p class="text-sm text-gray-600 dark:text-gray-400">
								{$_('gdpr.cookieConsent.necessaryDesc')}
							</p>
						</div>
						<input
							type="checkbox"
							checked
							disabled
							aria-label={$_('gdpr.cookieConsent.necessary')}
							class="mt-1 h-5 w-5 rounded border-gray-300 text-primary focus:ring-primary disabled:opacity-50"
						/>
					</div>

					<!-- Analytics Cookies -->
					<div class="flex items-start justify-between p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
						<div class="flex-1">
							<h3 class="font-semibold text-gray-900 dark:text-white mb-1">
								{$_('gdpr.cookieConsent.analytics')}
							</h3>
							<p class="text-sm text-gray-600 dark:text-gray-400">
								{$_('gdpr.cookieConsent.analyticsDesc')}
							</p>
						</div>
						<input
							type="checkbox"
							bind:checked={cookiePreferences.analytics}
							aria-label={$_('gdpr.cookieConsent.analytics')}
							class="mt-1 h-5 w-5 rounded border-gray-300 text-primary focus:ring-primary"
						/>
					</div>

					<!-- Marketing Cookies -->
					<div class="flex items-start justify-between p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
						<div class="flex-1">
							<h3 class="font-semibold text-gray-900 dark:text-white mb-1">
								{$_('gdpr.cookieConsent.marketing')}
							</h3>
							<p class="text-sm text-gray-600 dark:text-gray-400">
								{$_('gdpr.cookieConsent.marketingDesc')}
							</p>
						</div>
						<input
							type="checkbox"
							bind:checked={cookiePreferences.marketing}
							aria-label={$_('gdpr.cookieConsent.marketing')}
							class="mt-1 h-5 w-5 rounded border-gray-300 text-primary focus:ring-primary"
						/>
					</div>

					<!-- Preference Cookies -->
					<div class="flex items-start justify-between p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
						<div class="flex-1">
							<h3 class="font-semibold text-gray-900 dark:text-white mb-1">
								{$_('gdpr.cookieConsent.preferences')}
							</h3>
							<p class="text-sm text-gray-600 dark:text-gray-400">
								{$_('gdpr.cookieConsent.preferencesDesc')}
							</p>
						</div>
						<input
							type="checkbox"
							bind:checked={cookiePreferences.preferences}
							aria-label={$_('gdpr.cookieConsent.preferences')}
							class="mt-1 h-5 w-5 rounded border-gray-300 text-primary focus:ring-primary"
						/>
					</div>
				</div>

				<div class="flex flex-col sm:flex-row gap-3">
					<Button onclick={saveConsent} aria-label={$_('gdpr.cookieConsent.savePreferences')}>
						{$_('gdpr.cookieConsent.savePreferences')}
					</Button>
					<Button variant="outline" onclick={toggleCustomize} aria-label={$_('common.cancel')}>
						{$_('common.cancel')}
					</Button>
				</div>
			{/if}
		</Card>
	</div>
{/if}
