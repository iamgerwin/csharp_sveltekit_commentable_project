# Database Seeding Setup Guide

## ✅ What's Been Created

### Entity Models
- ✅ `BaseEntity.cs` - Base class for all entities
- ✅ `User.cs` - User authentication and profiles
- ✅ `Video.cs` - Video content
- ✅ `Post.cs` - Blog posts
- ✅ `Comment.cs` - Polymorphic comments system
- ✅ `Reaction.cs` - User reactions (Like, Dislike, Love, etc.)
- ✅ `Report.cs` - Content moderation reports

### Database Infrastructure
- ✅ `ApplicationDbContext.cs` - EF Core database context with full configuration
- ✅ `DatabaseSeeder.cs` - Comprehensive seeding system with realistic data
- ✅ `Program.cs` - Auto-migration and seed endpoint configured
- ✅ `appsettings.Development.json` - Database connection configured

### Seeding Features
- ✅ **8 Users**: Admin, Moderator, 5 regular users, 1 guest
- ✅ **10 Videos**: Programming tutorials with views and metadata
- ✅ **8 Posts**: Tech blog posts with SEO slugs
- ✅ **80-120 Comments**: Including nested replies
- ✅ **50-100 Reactions**: All reaction types distributed
- ✅ **5 Reports**: Mix of pending and reviewed

### Packages Installed
- ✅ BCrypt.Net-Next 4.0.3 - Password hashing
- ✅ Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0 - PostgreSQL provider
- ✅ Microsoft.EntityFrameworkCore 9.0.0 - ORM
- ✅ Microsoft.EntityFrameworkCore.Design 9.0.0 - Migration tools

## 🚀 Setup Instructions

### Step 1: Install PostgreSQL

```bash
# macOS (Homebrew)
brew install postgresql@16
brew services start postgresql@16

# Or use Docker
docker run --name postgres-commentable \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16
```

### Step 2: Create Database

```bash
# Using psql
psql postgres
CREATE DATABASE commentable;
\q

# Or using createdb
createdb commentable
```

### Step 3: Update Connection String (if needed)

Edit `apps/api/appsettings.Development.json` if your PostgreSQL credentials are different:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=commentable;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### Step 4: Create and Run Migrations

```bash
cd apps/api

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update
```

**Note**: If you encounter an error with .NET 10 SDK, you can use runtime migrations (already configured in Program.cs).

### Step 5: Run the API

```bash
dotnet run
```

The application will:
1. ✓ Auto-apply pending migrations
2. ✓ Start on http://localhost:5043
3. ✓ Enable Swagger UI at http://localhost:5043/api/swagger

### Step 6: Seed the Database

**Option A: Using cURL**
```bash
curl -X POST http://localhost:5043/api/v1/seed
```

**Option B: Using Swagger UI**
1. Open http://localhost:5043/api/swagger
2. Find `POST /api/v1/seed` under "Database" tag
3. Click "Try it out" → "Execute"

**Option C: Using your shell script**
```bash
./run.sh api  # Start the API
# In another terminal:
curl -X POST http://localhost:5043/api/v1/seed
```

## 📊 Seeded Data

### Test User Credentials

| Username | Email | Password | Role |
|----------|-------|----------|------|
| admin | admin@commentable.com | Admin123! | Admin |
| moderator | moderator@commentable.com | Mod123! | Moderator |
| john_doe | john@example.com | User123! | User |
| jane_smith | jane@example.com | User123! | User |
| mike_wilson | mike@example.com | User123! | User |
| emily_chen | emily@example.com | User123! | User |
| alex_kumar | alex@example.com | User123! | User |
| guest | guest@example.com | Guest123! | Guest |

### Data Relationships

```
Users (8)
├── Videos (10) - Created by content creators
│   └── Comments (30-50)
│       ├── Reactions (10-30 per video)
│       └── Reports (1-2)
├── Posts (8) - Created by authors
│   └── Comments (50-70)
│       ├── Reactions (20-40 per post)
│       └── Reports (2-3)
└── Direct Comments (0) - Can be added
```

### Sample Queries

```sql
-- Check seeded data
SELECT COUNT(*) FROM "Users";        -- Should be 8
SELECT COUNT(*) FROM "Videos";       -- Should be 10
SELECT COUNT(*) FROM "Posts";        -- Should be 8
SELECT COUNT(*) FROM "Comments";     -- Should be 80-120
SELECT COUNT(*) FROM "Reactions";    -- Should be 50-100
SELECT COUNT(*) FROM "Reports";      -- Should be 5

-- View users by role
SELECT "Username", "Email", "Role", "Status" FROM "Users";

-- Comments with reactions
SELECT c."Content", COUNT(r."Id") as reaction_count
FROM "Comments" c
LEFT JOIN "Reactions" r ON r."CommentId" = c."Id"
GROUP BY c."Id", c."Content"
HAVING COUNT(r."Id") > 0;
```

## 🔧 Troubleshooting

### Migration Error: "Could not load file or assembly 'System.Runtime'"

This is a known issue with EF Core tools and .NET 10 SDK. Solutions:

**Option 1: Use Runtime Migrations** (Already configured)
- Migrations run automatically on startup in Development
- No manual `dotnet ef` commands needed

**Option 2: Downgrade EF Core Tools**
```bash
dotnet tool update --global dotnet-ef --version 9.0.0
```

**Option 3: Use .NET 9 SDK for migrations**
```bash
# Temporarily switch to .NET 9 for migrations
dotnet new globaljson --sdk-version 9.0.100
dotnet ef migrations add InitialCreate
dotnet ef database update
rm global.json  # Remove after migrations
```

### "Database already contains data"

The seeder won't run if data exists. To reseed:

```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update

# Or manually
dropdb commentable
createdb commentable
dotnet ef database update

# Then seed
curl -X POST http://localhost:5043/api/v1/seed
```

### PostgreSQL Connection Failed

```bash
# Check if PostgreSQL is running
pg_isready

# Check connection
psql postgres -c "SELECT version();"

# Verify database exists
psql -l | grep commentable
```

## 📝 Next Steps

After seeding:

1. ✅ **Test Authentication**
   - Use Swagger UI to test login with seeded users
   - Implement JWT authentication endpoints

2. ✅ **Build API Endpoints**
   - Create controllers for Users, Videos, Posts, Comments
   - Implement CRUD operations
   - Add pagination and filtering

3. ✅ **Connect Frontend**
   - Fetch seeded data from SvelteKit app
   - Display videos and posts
   - Show comments with reactions

4. ✅ **Add Authorization**
   - Role-based access control
   - Moderator review workflow
   - User permissions

## 📚 Documentation

- Entity models: `Domain/Entities/`
- Database context: `Infrastructure/Data/ApplicationDbContext.cs`
- Seeder logic: `Infrastructure/Data/Seeders/DatabaseSeeder.cs`
- Seeder README: `Infrastructure/Data/Seeders/README.md`

## 🎯 Quick Commands Reference

```bash
# Setup
createdb commentable
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run & Seed
dotnet run
curl -X POST http://localhost:5043/api/v1/seed

# Reset
dotnet ef database drop --force
dotnet ef database update
curl -X POST http://localhost:5043/api/v1/seed

# Verify
psql commentable -c "SELECT COUNT(*) FROM \"Users\";"
```

Happy coding! 🚀
