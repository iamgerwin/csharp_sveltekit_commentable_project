/// <reference types="@sveltejs/kit" />
/// <reference no-default-lib="true"/>
/// <reference lib="esnext" />
/// <reference lib="webworker" />

import { build, files, version } from '$service-worker';

const sw = self as unknown as ServiceWorkerGlobalScope;

const CACHE_NAME = `commentable-cache-${version}`;
const ASSETS = [...build, ...files];

// Install event - cache assets
sw.addEventListener('install', (event) => {
	async function addFilesToCache() {
		const cache = await caches.open(CACHE_NAME);
		await cache.addAll(ASSETS);
	}

	event.waitUntil(addFilesToCache());
	sw.skipWaiting();
});

// Activate event - clean up old caches
sw.addEventListener('activate', (event) => {
	async function deleteOldCaches() {
		const keys = await caches.keys();
		await Promise.all(
			keys.map((key) => {
				if (key !== CACHE_NAME) {
					return caches.delete(key);
				}
			})
		);
	}

	event.waitUntil(deleteOldCaches());
	sw.clients.claim();
});

// Fetch event - serve from cache, fallback to network
sw.addEventListener('fetch', (event) => {
	// Skip cross-origin requests
	if (!event.request.url.startsWith(sw.location.origin)) {
		return;
	}

	// Skip API requests - always fetch from network
	if (event.request.url.includes('/api/')) {
		event.respondWith(
			fetch(event.request).catch(() => {
				return new Response(
					JSON.stringify({
						message: 'Unable to connect to server. Please check your internet connection.'
					}),
					{
						status: 503,
						headers: { 'Content-Type': 'application/json' }
					}
				);
			})
		);
		return;
	}

	// For navigation requests, use network-first strategy
	if (event.request.mode === 'navigate') {
		event.respondWith(
			fetch(event.request).catch(async () => {
				const cache = await caches.open(CACHE_NAME);
				const cachedResponse = await cache.match('/');
				return cachedResponse || new Response('Offline - Please check your connection');
			})
		);
		return;
	}

	// For other requests, use cache-first strategy
	event.respondWith(
		caches.match(event.request).then((response) => {
			return (
				response ||
				fetch(event.request).then((fetchResponse) => {
					// Cache successful GET requests
					if (event.request.method === 'GET' && fetchResponse.status === 200) {
						const responseToCache = fetchResponse.clone();
						caches.open(CACHE_NAME).then((cache) => {
							cache.put(event.request, responseToCache);
						});
					}
					return fetchResponse;
				})
			);
		})
	);
});

// Background sync for offline actions (future enhancement)
sw.addEventListener('sync', (event) => {
	if (event.tag === 'sync-comments') {
		event.waitUntil(syncComments());
	}
});

async function syncComments() {
	// Placeholder for future background sync implementation
	console.log('Background sync: comments');
}

// Push notification handler (future enhancement)
sw.addEventListener('push', (event) => {
	const data = event.data ? event.data.json() : {};
	const title = data.title || 'Commentable';
	const options = {
		body: data.body || 'You have a new notification',
		icon: '/icons/icon-192x192.png',
		badge: '/icons/icon-96x96.png',
		data: data.url || '/'
	};

	event.waitUntil(sw.registration.showNotification(title, options));
});

// Notification click handler
sw.addEventListener('notificationclick', (event) => {
	event.notification.close();
	event.waitUntil(sw.clients.openWindow(event.notification.data || '/'));
});

// Handle skip waiting message from clients
sw.addEventListener('message', (event) => {
	if (event.data && event.data.type === 'SKIP_WAITING') {
		sw.skipWaiting();
	}
});
