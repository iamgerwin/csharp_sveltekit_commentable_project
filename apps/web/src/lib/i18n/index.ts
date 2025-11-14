import { register, init, getLocaleFromNavigator } from 'svelte-i18n';
import { browser } from '$app/environment';

// Register translation files
register('en', () => import('./locales/en.json'));
register('es', () => import('./locales/es.json'));
register('fr', () => import('./locales/fr.json'));
register('de', () => import('./locales/de.json'));
register('tl', () => import('./locales/tl.json'));

// Initialize i18n
export function initI18n() {
	// Check for saved language preference in localStorage first (only on client)
	const savedLanguage = browser ? localStorage.getItem('preferred-language') : null;

	init({
		fallbackLocale: 'en',
		initialLocale: savedLanguage || (browser ? getLocaleFromNavigator() : 'en')
	});
}
