# Commentable Frontend Implementation Guide

## Overview

This is a comprehensive SvelteKit 5 frontend application with full authentication, CRUD operations for Videos and Posts, nested comments with replies, reactions with optimistic updates, and moderation features.

## Features Implemented

### ✅ Authentication System
- **Login Page** (`/login`) - Email/password authentication with validation
- **Register Page** (`/register`) - User registration with password confirmation
- **Auth Store** - Svelte 5 runes-based state management
- **JWT Token Management** - Automatic token refresh and storage
- **Protected Routes** - Dashboard requires authentication

### ✅ Dashboard
- **Main Dashboard** (`/dashboard`) - Welcome screen with stats and quick actions
- **Responsive Navigation** - Mobile-friendly menu with role-based items
- **User Profile Display** - Shows username and role badges (Admin/Moderator)
- **Quick Actions** - Direct links to create videos and posts

### ✅ Videos CRUD
- **List Page** (`/dashboard/videos`) - Grid view with search, pagination
- **Detail Page** (`/dashboard/videos/[id]`) - Video player, comments, reactions
- **Create Page** (`/dashboard/videos/new`) - Upload new video with metadata
- **Edit Page** (`/dashboard/videos/[id]/edit`) - Update video information
- **Delete Functionality** - Soft delete with confirmation

### ✅ Posts CRUD
- **List Page** (`/dashboard/posts`) - List view with search, pagination
- **Detail Page** (`/dashboard/posts/[id]`) - Full post with comments, reactions
- **Create Page** (`/dashboard/posts/new`) - Create post with tags
- **Edit Page** (`/dashboard/posts/[id]/edit`) - Update post content
- **Delete Functionality** - Soft delete with confirmation

### ✅ Comments System
- **Nested Comments Component** - Recursive component supporting replies
- **Create Comments** - Post comments on videos, posts, and other comments
- **Edit Comments** - Update your own comments
- **Delete Comments** - Remove comments with permission check
- **Load Replies** - Lazy-load nested replies (up to 5 levels deep)
- **Real-time Counts** - Display reply counts

### ✅ Reactions with Optimistic Updates
- **6 Reaction Types** - Like, Love, Laugh, Wow, Sad, Angry
- **Optimistic UI** - Instant feedback, rollback on error
- **Hover Picker** - Beautiful reaction selector
- **Reaction Counts** - Display top 3 reactions
- **User Reaction State** - Show user's current reaction

### ✅ Moderation
- **Reports Page** (`/dashboard/reports`) - View and filter user reports
- **Role-Based Access** - Moderator/Admin only
- **Status Filters** - Pending, Under Review, Resolved, Dismissed
- **Report Details** - View full report information

## Technology Stack

### Core
- **SvelteKit 5** - Latest version with runes
- **TypeScript** - Full type safety
- **Vite** - Fast build tool
- **TailwindCSS 3** - Utility-first styling

### UI Components
- **Custom Components** - Button, Input, Card, Label, Textarea
- **Shadcn-svelte** - Component foundation
- **Responsive Design** - Mobile-first approach

### State Management
- **Svelte 5 Runes** - $state, $derived, $bindable
- **Auth Store** - Global authentication state
- **Toast Store** - Notification system

### API Integration
- **API Client** - Centralized HTTP client with interceptors
- **JWT Handling** - Automatic token refresh
- **Error Handling** - Global error management
- **Type-Safe APIs** - Full TypeScript types

## Project Structure

```
apps/web/src/
├── lib/
│   ├── api/                      # API clients
│   │   ├── client.ts            # Base HTTP client with JWT
│   │   ├── auth.api.ts          # Authentication endpoints
│   │   ├── videos.api.ts        # Videos CRUD
│   │   ├── posts.api.ts         # Posts CRUD
│   │   ├── comments.api.ts      # Comments CRUD
│   │   ├── reactions.api.ts     # Reactions endpoints
│   │   └── reports.api.ts       # Reports/moderation
│   │
│   ├── components/              # Reusable components
│   │   ├── ui/                  # Base UI components
│   │   │   ├── button.svelte
│   │   │   ├── input.svelte
│   │   │   ├── card.svelte
│   │   │   ├── label.svelte
│   │   │   ├── textarea.svelte
│   │   │   └── toast.svelte
│   │   ├── reactions.svelte     # Reaction component
│   │   └── comment-item.svelte  # Nested comment component
│   │
│   ├── stores/                  # State management
│   │   ├── auth.store.ts       # Authentication state
│   │   └── toast.store.ts      # Toast notifications
│   │
│   ├── types/                   # TypeScript types
│   │   ├── auth.types.ts       # Auth interfaces
│   │   └── entities.types.ts   # Entity interfaces
│   │
│   └── utils.ts                 # Utility functions
│
├── routes/                      # Pages and routing
│   ├── +layout.svelte          # Root layout
│   ├── +page.svelte            # Home page
│   ├── login/
│   │   └── +page.svelte        # Login page
│   ├── register/
│   │   └── +page.svelte        # Register page
│   └── dashboard/
│       ├── +layout.svelte      # Dashboard layout
│       ├── +page.svelte        # Dashboard home
│       ├── videos/
│       │   ├── +page.svelte    # Videos list
│       │   ├── new/
│       │   │   └── +page.svelte # Create video
│       │   └── [id]/
│       │       ├── +page.svelte # Video detail
│       │       └── edit/
│       │           └── +page.svelte # Edit video
│       ├── posts/
│       │   ├── +page.svelte    # Posts list
│       │   ├── new/
│       │   │   └── +page.svelte # Create post
│       │   └── [id]/
│       │       ├── +page.svelte # Post detail
│       │       └── edit/
│       │           └── +page.svelte # Edit post
│       └── reports/
│           └── +page.svelte    # Reports page
│
└── app.css                      # Global styles
```

## API Endpoints Used

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login with credentials
- `POST /api/auth/logout` - Logout current user
- `POST /api/auth/refresh` - Refresh access token
- `GET /api/auth/me` - Get current user

### Videos
- `GET /api/videos` - List videos (with filters)
- `GET /api/videos/{id}` - Get video by ID
- `POST /api/videos` - Create video
- `PUT /api/videos/{id}` - Update video
- `DELETE /api/videos/{id}` - Delete video
- `GET /api/videos/{id}/comments` - Get video comments

### Posts
- `GET /api/posts` - List posts (with filters)
- `GET /api/posts/{id}` - Get post by ID
- `POST /api/posts` - Create post
- `PUT /api/posts/{id}` - Update post
- `DELETE /api/posts/{id}` - Delete post
- `GET /api/posts/{id}/comments` - Get post comments

### Comments
- `GET /api/comments` - List comments (with filters)
- `GET /api/comments/{id}` - Get comment by ID
- `POST /api/comments` - Create comment
- `PUT /api/comments/{id}` - Update comment
- `DELETE /api/comments/{id}` - Delete comment
- `GET /api/comments/{id}/replies` - Get comment replies

### Reactions
- `GET /api/reactions` - List reactions
- `POST /api/reactions/upsert` - Create/update reaction
- `DELETE /api/reactions/{id}` - Delete reaction

### Reports
- `GET /api/reports` - List reports (moderators only)
- `GET /api/reports/{id}` - Get report by ID
- `POST /api/reports` - Create report
- `PUT /api/reports/{id}/review` - Review report (moderators only)

## Key Features Explained

### Optimistic Updates (Reactions)

The reactions component implements optimistic updates for better UX:

1. **Immediate Feedback** - UI updates instantly when user clicks
2. **Background API Call** - Request sent to server
3. **Rollback on Error** - Reverts to previous state if API fails
4. **State Synchronization** - Parent components notified of changes

```typescript
// Optimistic update flow
function optimisticUpdate(newReactionType: ReactionType | null) {
    const oldState = saveCurrentState();
    updateUIImmediately(newReactionType);

    try {
        await api.upsertReaction(newReactionType);
        // Success - keep new state
    } catch (error) {
        restoreOldState(oldState);
        showError();
    }
}
```

### Nested Comments

Recursive component supporting unlimited nesting (limited to 5 levels for UX):

1. **Self-Referencing Component** - `<svelte:self>` for recursion
2. **Lazy Loading** - Replies loaded on demand
3. **Visual Indentation** - Margins increase with depth
4. **Reply Functionality** - Reply to any comment
5. **Edit/Delete** - Permission-based actions

### JWT Token Management

Automatic token refresh with retry logic:

1. **Token Storage** - LocalStorage for persistence
2. **Request Interceptor** - Auto-attach JWT to requests
3. **401 Detection** - Catch expired tokens
4. **Token Refresh** - Automatic renewal with refresh token
5. **Retry Logic** - Retry failed request with new token
6. **Logout on Failure** - Redirect to login if refresh fails

### Form Validation

Client-side validation on all forms:

1. **Required Fields** - Prevent empty submissions
2. **Email Format** - Regex validation
3. **Password Strength** - Minimum length checks
4. **Password Confirmation** - Match validation
5. **Real-time Feedback** - Error messages below fields

## Environment Configuration

Create `.env` file in `apps/web/`:

```env
VITE_API_BASE_URL=http://localhost:5043/api
```

## Running the Application

### Development
```bash
cd apps/web
npm install
npm run dev
```

Access at: `http://localhost:5173`

### Build for Production
```bash
npm run build
npm run preview
```

## User Roles

### Guest (0)
- View public content
- Cannot create, edit, or react

### User (1)
- All guest permissions
- Create videos and posts
- Comment and reply
- React to content
- Edit own content
- Delete own content

### Moderator (2)
- All user permissions
- View reports
- Edit any content
- Delete any content
- Review reports

### Admin (3)
- All moderator permissions
- Full system access

## Best Practices Implemented

### Code Organization
- ✅ Modular component structure
- ✅ Separation of concerns (UI, logic, state)
- ✅ Type-safe API clients
- ✅ Reusable components

### Performance
- ✅ Lazy loading for comments
- ✅ Pagination for lists
- ✅ Optimistic updates
- ✅ Debounced search

### UX
- ✅ Loading states
- ✅ Error handling
- ✅ Toast notifications
- ✅ Responsive design
- ✅ Keyboard navigation
- ✅ Accessibility

### Security
- ✅ JWT authentication
- ✅ Protected routes
- ✅ Permission checks
- ✅ Input validation
- ✅ XSS prevention

## Next Steps

To complete the implementation:

1. **Backend API** - Implement C# .NET API endpoints
2. **Database** - Set up PostgreSQL with Entity Framework
3. **File Upload** - Implement video/image upload
4. **Search** - Add full-text search
5. **Real-time** - Add WebSocket for notifications
6. **Testing** - Add unit and E2E tests

## Available Routes

### Public
- `/` - Home page
- `/login` - Login page
- `/register` - Registration page

### Protected (Requires Authentication)
- `/dashboard` - Main dashboard
- `/dashboard/videos` - Videos list
- `/dashboard/videos/new` - Create video
- `/dashboard/videos/[id]` - Video detail
- `/dashboard/videos/[id]/edit` - Edit video
- `/dashboard/posts` - Posts list
- `/dashboard/posts/new` - Create post
- `/dashboard/posts/[id]` - Post detail
- `/dashboard/posts/[id]/edit` - Edit post
- `/dashboard/reports` - Reports (Moderators only)

## Troubleshooting

### API Connection Issues
- Check `VITE_API_BASE_URL` in `.env`
- Ensure backend API is running
- Verify CORS settings

### Authentication Issues
- Clear localStorage
- Check JWT token expiration
- Verify refresh token flow

### Build Issues
- Clear `.svelte-kit` directory
- Run `npm install` again
- Check Node.js version (18+)

## Support

For issues or questions, refer to:
- SvelteKit docs: https://kit.svelte.dev
- Svelte 5 docs: https://svelte.dev
- TailwindCSS docs: https://tailwindcss.com
