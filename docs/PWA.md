# Progressive Web App (PWA) Documentation

## Overview

The Commentable application is now a fully functional Progressive Web App (PWA) that can be installed on desktop and mobile devices, works offline, and provides a native app-like experience.

## Features Implemented

### 1. **Installability**
- Users can install the app on their devices (desktop, mobile, tablet)
- Custom install prompt appears after 5 seconds on first visit
- Install prompt respects user preferences (dismissal persists for 30 days)
- Detects if app is already installed to avoid redundant prompts

### 2. **Offline Support**
- Service worker caches static assets for offline access
- Smart caching strategies:
  - **Cache-first** for static assets (JS, CSS, images)
  - **Network-first** for navigation requests
  - **Network-only** for API calls with graceful error handling
- Offline fallback page when network is unavailable

### 3. **App Update Management**
- Automatic update detection
- User-friendly update notification
- Manual update trigger option
- Periodic update checks (every hour)

### 4. **Native App Features**
- Custom app icons for all platforms (72px to 512px)
- Splash screens and app shortcuts
- Proper theme color integration
- Full-screen standalone display mode

### 5. **Future Enhancements Ready**
- Push notification handlers (placeholder)
- Background sync support (placeholder)
- Share target API ready for integration

## File Structure

```
apps/web/
├── src/
│   ├── service-worker.ts              # Service worker with caching logic
│   ├── app.html                        # PWA meta tags
│   ├── lib/
│   │   └── components/
│   │       ├── PWAManager.svelte       # Service worker manager
│   │       └── PWAInstallPrompt.svelte # Install prompt component
│   └── routes/
│       └── +layout.svelte             # Root layout with PWA integration
├── static/
│   ├── manifest.json                   # Web app manifest
│   └── icons/
│       ├── icon.svg                    # Source icon
│       ├── icon-72x72.png             # App icons (various sizes)
│       ├── icon-96x96.png
│       ├── icon-128x128.png
│       ├── icon-144x144.png
│       ├── icon-152x152.png
│       ├── icon-192x192.png
│       ├── icon-384x384.png
│       ├── icon-512x512.png
│       ├── apple-touch-icon.png       # iOS icon
│       ├── favicon-32x32.png          # Browser favicons
│       └── favicon-16x16.png
└── scripts/
    └── generate-icons.js              # Icon generation script
```

## Technical Implementation

### Service Worker (`src/service-worker.ts`)

The service worker implements:

1. **Install Event**: Caches all static assets on first installation
2. **Activate Event**: Cleans up old caches when updating
3. **Fetch Event**: Serves content based on request type
   - Static assets: Cache-first
   - Navigation: Network-first with offline fallback
   - API calls: Network-only with error handling
4. **Update Handling**: Skip waiting message support
5. **Future Ready**: Push notifications and background sync handlers

### Web App Manifest (`static/manifest.json`)

Defines:
- App name and description
- Display mode (standalone)
- Theme colors
- App icons (all required sizes)
- Start URL
- App shortcuts for quick access
- Categories for app stores

### PWA Manager (`lib/components/PWAManager.svelte`)

Handles:
- Service worker registration
- Update detection
- Update notifications
- Periodic update checks
- Integration with toast system

### Install Prompt (`lib/components/PWAInstallPrompt.svelte`)

Features:
- Smart timing (appears after 5 seconds)
- User preference persistence
- Dismissal cooldown (30 days)
- Install state detection
- Responsive design
- Animated slide-up effect

## Usage

### Generating Icons

To regenerate PWA icons from the source SVG:

```bash
cd apps/web
npm run generate:icons
```

This generates:
- All app icon sizes (72px to 512px)
- Apple touch icon (180px)
- Favicons (16px and 32px)

### Customizing the App Icon

1. Edit `apps/web/static/icons/icon.svg` with your design
2. Run `npm run generate:icons` to generate all sizes
3. Icons automatically update across all platforms

### Customizing App Metadata

Edit `apps/web/static/manifest.json`:

```json
{
  "name": "Your App Name",
  "short_name": "App",
  "description": "Your app description",
  "theme_color": "#your-color",
  "background_color": "#your-color"
}
```

### Testing PWA Features

1. **Development Mode**:
   - Service worker works in dev mode but may have issues
   - For full testing, use production build

2. **Production Build**:
   ```bash
   cd apps/web
   npm run build
   npm run preview
   ```

3. **Testing Installability**:
   - Open Chrome DevTools > Application > Manifest
   - Check for manifest errors
   - Test install prompt in supported browsers

4. **Testing Offline**:
   - Open Chrome DevTools > Network
   - Enable "Offline" mode
   - Navigate the app to test offline functionality

5. **Testing Service Worker**:
   - Open Chrome DevTools > Application > Service Workers
   - View service worker status and caches
   - Test update flow

## Browser Support

### Desktop
- ✅ Chrome/Edge (Windows, macOS, Linux)
- ✅ Firefox (limited PWA features)
- ⚠️ Safari (basic support, no install prompt)

### Mobile
- ✅ Chrome (Android)
- ✅ Samsung Internet (Android)
- ✅ Safari (iOS 16.4+)
- ✅ Edge (Android/iOS)

## Caching Strategy

### Static Assets (Cache-First)
- JavaScript bundles
- CSS stylesheets
- Images
- Fonts
- Icons

### Navigation (Network-First)
- HTML pages
- Routes
- Falls back to cache if offline

### API Calls (Network-Only)
- Always fetch from network
- Provide offline error message if network unavailable
- No caching of API responses (ensures data freshness)

## Best Practices

### 1. **Update Management**
- Service worker updates automatically in background
- Users notified when update available
- Manual update trigger provided
- No forced updates (user choice)

### 2. **Cache Management**
- Old caches automatically cleaned on update
- Versioned cache names
- No stale content served

### 3. **Offline Experience**
- Clear offline messaging
- Graceful degradation
- No broken functionality

### 4. **Performance**
- Minimal service worker overhead
- Efficient caching
- Fast app startup

## Troubleshooting

### Service Worker Not Registering

**Issue**: Service worker fails to register
**Solution**:
- Check browser console for errors
- Ensure HTTPS or localhost
- Verify service worker file path
- Check for TypeScript compilation errors

### App Not Installable

**Issue**: Install prompt doesn't appear
**Solution**:
- Verify manifest.json is valid
- Check all required icons exist
- Ensure HTTPS connection
- Verify manifest is linked in app.html
- Check browser DevTools > Application > Manifest

### Offline Mode Not Working

**Issue**: App doesn't work offline
**Solution**:
- Check service worker is activated
- Verify assets are cached (DevTools > Application > Cache Storage)
- Check fetch event handlers
- Ensure service worker scope is correct

### Update Not Detected

**Issue**: New version doesn't trigger update
**Solution**:
- Service worker updates every hour automatically
- Force update: DevTools > Application > Service Workers > Update
- Clear cache and reload
- Check service worker version number

## Future Enhancements

### Planned Features

1. **Push Notifications**
   - Real-time comment notifications
   - New content alerts
   - Report status updates

2. **Background Sync**
   - Offline comment creation
   - Queue for network recovery
   - Automatic retry logic

3. **Advanced Caching**
   - Selective API response caching
   - Cache-then-network strategy for lists
   - Image optimization and caching

4. **Share Target**
   - Share to Commentable from other apps
   - Content creation via share

5. **App Shortcuts**
   - Quick actions from home screen
   - Jump to specific sections
   - Context menu integration

### Implementation Checklist

- [ ] Push notification subscription
- [ ] Background sync for offline actions
- [ ] Share target API integration
- [ ] Advanced caching strategies
- [ ] App shortcuts implementation
- [ ] File handling API
- [ ] Web share API

## Security Considerations

1. **HTTPS Required**
   - Service workers only work over HTTPS
   - localhost exempt for development

2. **Content Security Policy**
   - Ensure CSP allows service worker
   - Verify script-src includes service worker

3. **Cache Security**
   - No sensitive data in cache
   - API responses not cached
   - User data not persisted offline

## Performance Metrics

### Lighthouse PWA Score: Target 100/100

Checklist:
- ✅ Registers a service worker
- ✅ Responds with 200 when offline
- ✅ Contains web app manifest
- ✅ Manifest includes name, icons, start_url
- ✅ Theme color set
- ✅ Content sized correctly for viewport
- ✅ Page load fast on mobile networks

## Resources

- [Web.dev PWA Guide](https://web.dev/progressive-web-apps/)
- [MDN Service Worker API](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
- [Web App Manifest Spec](https://www.w3.org/TR/appmanifest/)
- [Workbox (Google)](https://developers.google.com/web/tools/workbox)
- [SvelteKit PWA](https://kit.svelte.dev/docs/service-workers)

## Changelog

### Version 1.0.0 (Current)
- ✅ Initial PWA implementation
- ✅ Service worker with caching
- ✅ Web app manifest
- ✅ Install prompt component
- ✅ Update management
- ✅ Offline support
- ✅ Icon generation script
- ✅ Comprehensive documentation
