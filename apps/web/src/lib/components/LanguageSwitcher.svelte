<script lang="ts">
	import { locale } from 'svelte-i18n';
	import { onMount } from 'svelte';

	interface Language {
		code: string;
		name: string;
		nativeName: string;
		flag: string;
	}

	const languages: Language[] = [
		{ code: 'en', name: 'English', nativeName: 'English', flag: '🇺🇸' },
		{ code: 'es', name: 'Spanish', nativeName: 'Español', flag: '🇪🇸' },
		{ code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' },
		{ code: 'de', name: 'German', nativeName: 'Deutsch', flag: '🇩🇪' },
		{ code: 'tl', name: 'Filipino', nativeName: 'Tagalog', flag: '🇵🇭' }
	];

	let currentLocale = $state('en');
	let isOpen = $state(false);
	let buttonRef: HTMLButtonElement;

	// Subscribe to locale changes
	$effect(() => {
		const unsubscribe = locale.subscribe((value) => {
			if (value) {
				currentLocale = value;
			}
		});
		return unsubscribe;
	});

	function changeLanguage(langCode: string) {
		locale.set(langCode);
		localStorage.setItem('preferred-language', langCode);
		isOpen = false;
	}

	function toggleDropdown() {
		isOpen = !isOpen;
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Escape') {
			isOpen = false;
			buttonRef?.focus();
		}
	}

	function handleClickOutside(event: MouseEvent) {
		const target = event.target as HTMLElement;
		if (isOpen && !target.closest('.language-switcher')) {
			isOpen = false;
		}
	}

	onMount(() => {
		document.addEventListener('click', handleClickOutside);
		return () => {
			document.removeEventListener('click', handleClickOutside);
		};
	});

	function getCurrentLanguage(): Language {
		return languages.find((lang) => lang.code === currentLocale) || languages[0];
	}
</script>

<div class="language-switcher relative" onkeydown={handleKeydown}>
	<button
		bind:this={buttonRef}
		onclick={toggleDropdown}
		aria-label="Select language. Current: {getCurrentLanguage().name}"
		aria-expanded={isOpen}
		aria-haspopup="true"
		aria-controls="language-menu"
		class="flex items-center gap-2 px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 dark:focus:ring-offset-gray-900"
	>
		<span class="text-xl" role="img" aria-hidden="true">
			{getCurrentLanguage().flag}
		</span>
		<span class="hidden sm:inline">
			{getCurrentLanguage().nativeName}
		</span>
		<svg
			class="w-4 h-4 transition-transform {isOpen ? 'rotate-180' : ''}"
			fill="none"
			stroke="currentColor"
			viewBox="0 0 24 24"
			aria-hidden="true"
		>
			<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
		</svg>
	</button>

	{#if isOpen}
		<div
			id="language-menu"
			role="menu"
			aria-label="Language options"
			class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg z-50 py-1"
		>
			{#each languages as lang, index}
				<button
					onclick={() => changeLanguage(lang.code)}
					role="menuitem"
					tabindex={isOpen ? 0 : -1}
					aria-label="Switch to {lang.name}"
					class="w-full flex items-center gap-3 px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors focus:outline-none focus:bg-gray-100 dark:focus:bg-gray-700 {currentLocale ===
					lang.code
						? 'bg-gray-50 dark:bg-gray-700/50 font-semibold'
						: ''}"
				>
					<span class="text-xl" role="img" aria-hidden="true">
						{lang.flag}
					</span>
					<span class="flex-1 text-left">
						{lang.nativeName}
					</span>
					{#if currentLocale === lang.code}
						<svg
							class="w-5 h-5 text-primary"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
							aria-hidden="true"
						>
							<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
						</svg>
						<span class="sr-only">Selected</span>
					{/if}
				</button>
			{/each}
		</div>
	{/if}
</div>

<style>
	.language-switcher {
		position: relative;
	}
</style>
