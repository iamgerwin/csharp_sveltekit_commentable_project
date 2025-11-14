/// <reference types="@sveltejs/kit" />
import { build, files, version } from '$service-worker';

const CACHE_NAME = `commentable-cache-${version}`;
const ASSETS = [...build, ...files];

// Install event - cache assets
self.addEventListener('install', (event) => {
	async function addFilesToCache() {
		const cache = await caches.open(CACHE_NAME);
		await cache.addAll(ASSETS);
	}

	event.waitUntil(addFilesToCache());
	self.skipWaiting();
});

// Activate event - clean up old caches
self.addEventListener('activate', (event) => {
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
	self.clients.claim();
});

// Fetch event - serve from cache, fallback to network
self.addEventListener('fetch', (event) => {
	// Skip cross-origin requests
	if (!event.request.url.startsWith(self.location.origin)) {
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
				return cache.match('/') || new Response('Offline - Please check your connection');
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
self.addEventListener('sync', (event) => {
	if (event.tag === 'sync-comments') {
		event.waitUntil(syncComments());
	}
});

async function syncComments() {
	// Placeholder for future background sync implementation
	console.log('Background sync: comments');
}

// Push notification handler (future enhancement)
self.addEventListener('push', (event) => {
	const data = event.data ? event.data.json() : {};
	const title = data.title || 'Commentable';
	const options = {
		body: data.body || 'You have a new notification',
		icon: '/icons/icon-192x192.png',
		badge: '/icons/icon-96x96.png',
		data: data.url || '/'
	};

	event.waitUntil(self.registration.showNotification(title, options));
});

// Notification click handler
self.addEventListener('notificationclick', (event) => {
	event.notification.close();
	event.waitUntil(
		clients.openWindow(event.notification.data || '/')
	);
});
