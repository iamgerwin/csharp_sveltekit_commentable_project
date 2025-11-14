import { register, init, getLocaleFromNavigator } from 'svelte-i18n';

// Register translation files
register('en', () => import('./locales/en.json'));
register('es', () => import('./locales/es.json'));
register('fr', () => import('./locales/fr.json'));
register('de', () => import('./locales/de.json'));
register('tl', () => import('./locales/tl.json'));

// Initialize i18n
export function initI18n() {
	// Check for saved language preference in localStorage first
	const savedLanguage =
		typeof localStorage !== 'undefined' ? localStorage.getItem('preferred-language') : null;

	init({
		fallbackLocale: 'en',
		initialLocale: savedLanguage || getLocaleFromNavigator()
	});
}
