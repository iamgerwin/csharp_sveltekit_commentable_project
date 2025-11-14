# Getting Started with Commentable API

## Quick Start

### 1. Run the API

```bash
cd apps/api
dotnet run
```

The API will:
- Automatically create the SQLite database (`commentable.db`)
- Create all required tables with proper indexes and relationships
- Start listening on `http://localhost:5043`

### 2. Seed the Database

```bash
curl -X POST http://localhost:5043/api/v1/seed
```

Or use Swagger UI:
1. Open `http://localhost:5043/api/swagger`
2. Find `POST /api/v1/seed` under "Database" tag
3. Click "Try it out" → "Execute"

### 3. Explore the API

- **Swagger UI**: http://localhost:5043/api/swagger
- **Health Check**: http://localhost:5043/api/v1/health

## Test User Credentials

Use these credentials to test authentication and authorization features:

| Username | Email | Password | Role | Description |
|----------|-------|----------|------|-------------|
| `admin` | admin@commentable.com | `Admin123!` | Admin | Full system access |
| `moderator` | moderator@commentable.com | `Mod123!` | Moderator | Can review and moderate content |
| `john_doe` | john@example.com | `User123!` | User | Regular user account |
| `jane_smith` | jane@example.com | `User123!` | User | Regular user account |
| `mike_wilson` | mike@example.com | `User123!` | User | Regular user account |
| `emily_chen` | emily@example.com | `User123!` | User | Regular user account |
| `alex_kumar` | alex@example.com | `User123!` | User | Regular user account |
| `guest` | guest@example.com | `Guest123!` | Guest | Limited guest access |

## Seeded Data Summary

After running the seed endpoint, your database will contain:

- **8 Users** - Different roles (Admin, Moderator, User, Guest)
- **10 Videos** - Programming tutorials with titles, descriptions, thumbnails
- **8 Blog Posts** - Tech articles with SEO-friendly slugs
- **~120 Comments** - Including nested replies (up to 3 levels deep)
- **~150 Reactions** - Likes, loves, laughs, etc. on comments
- **5 Reports** - Sample moderation reports (pending and reviewed)

## Database Information

### Development Database
- **Type**: SQLite
- **Location**: `apps/api/commentable.db`
- **Auto-created**: Yes (on first run)
- **Gitignored**: Yes (*.db files are excluded)

### Production Database
- **Type**: PostgreSQL
- **Configuration**: Set in `appsettings.Production.json`
- **Connection String**: Configure via environment variables

## API Endpoints

### Health & Database
- `GET /api/v1/health` - Health check endpoint
- `POST /api/v1/seed` - Seed database with test data (Development only)

### Swagger Documentation
- `GET /api/swagger` - Interactive API documentation

## Database Schema

### Entity Relationships

```
Users (8)
├── Videos (10) - One-to-many
│   └── Comments - Polymorphic relationship
│       ├── Reactions - One-to-many
│       └── Reports - One-to-many
├── Posts (8) - One-to-many
│   └── Comments - Polymorphic relationship
│       ├── Reactions - One-to-many
│       └── Reports - One-to-many
└── Comments - Direct comments (can be on any entity)
    ├── Nested Replies - Self-referencing relationship
    ├── Reactions - One-to-many (unique per user)
    └── Reports - One-to-many
```

### Key Features

1. **Polymorphic Comments**: Comments can be attached to Videos, Posts, or any future entity
2. **Nested Comments**: Comments can have replies (self-referencing with `ParentCommentId`)
3. **Unique Reactions**: Each user can only react once per comment
4. **Soft Deletes**: Users have `DeletedAt` for soft deletion
5. **Denormalized Counts**: `CommentCount`, `ReactionCount`, `ViewCount` for performance
6. **Moderation System**: Reports with status tracking and reviewer notes

## Resetting the Database

To start fresh with a clean database:

```bash
# Stop the API if running
pkill -f "dotnet run"

# Delete the database
rm commentable.db*

# Run the API (will recreate the database)
dotnet run

# Re-seed the data
curl -X POST http://localhost:5043/api/v1/seed
```

## Verifying Seeded Data

### Using SQLite CLI

```bash
# Install SQLite (if not already installed)
brew install sqlite  # macOS
# or: apt install sqlite3  # Linux

# Query the database
sqlite3 commentable.db

# Sample queries
SELECT COUNT(*) FROM Users;
SELECT Username, Role FROM Users;
SELECT Title FROM Videos;
SELECT Title, Slug FROM Posts;
```

### Using Swagger UI

1. Open http://localhost:5043/api/swagger
2. Test the endpoints with the test user credentials
3. Explore the seeded data through the API

## Next Steps

1. **Build Controllers**: Create API endpoints for CRUD operations
   - UsersController
   - VideosController
   - PostsController
   - CommentsController
   - ReactionsController

2. **Add Authentication**: Implement JWT-based authentication
   - Login endpoint
   - Token generation
   - Protected routes

3. **Add Authorization**: Role-based access control
   - Admin-only endpoints
   - Moderator actions
   - User permissions

4. **Connect Frontend**: Integrate with the SvelteKit app
   - Fetch videos and posts
   - Display comments
   - Handle reactions

## Documentation

- **Entity Models**: `Domain/Entities/`
- **Database Context**: `Infrastructure/Data/ApplicationDbContext.cs`
- **Seeder Logic**: `Infrastructure/Data/Seeders/DatabaseSeeder.cs`
- **Seeder README**: `Infrastructure/Data/Seeders/README.md`
- **Setup Guide**: `SEEDING_SETUP.md`

## Troubleshooting

### Database Not Created
- Ensure you're in the `apps/api` directory
- Check console output for error messages
- Verify write permissions in the directory

### Seeding Fails
- Database might already contain data (seeder checks for existing users)
- Reset the database using the steps above
- Check the error message in the response

### Port Already in Use
- Default port is 5043
- Kill existing processes: `pkill -f "dotnet run"`
- Or change port in `Properties/launchSettings.json`

## Support

For issues or questions:
- Check the documentation files in this repository
- Review the Swagger UI for API endpoint details
- Examine the console output for detailed error messages

Happy coding!
