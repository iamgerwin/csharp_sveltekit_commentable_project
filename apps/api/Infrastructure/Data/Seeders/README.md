# Database Seeder

Comprehensive database seeding system for development and testing.

## Features

- **Realistic Data**: Seeds realistic user profiles, videos, posts, comments, reactions, and reports
- **Relationship Management**: Maintains proper foreign key relationships
- **Denormalized Counts**: Pre-populates reaction counts, comment counts, etc.
- **Safe Execution**: Checks for existing data before seeding to prevent duplicates
- **Ordered Seeding**: Seeds entities in dependency order (users → videos/posts → comments → reactions/reports)

## What Gets Seeded

### Users (8 total)
- 1 Admin user
- 1 Moderator user
- 5 Regular users
- 1 Guest user

**Test Credentials:**
- Admin: `admin` / `Admin123!`
- Moderator: `moderator` / `Mod123!`
- Users: `john_doe`, `jane_smith`, etc. / `User123!`
- Guest: `guest` / `Guest123!`

### Videos (10)
- Programming tutorials and tech content
- Assigned to content creators
- Random view counts (100-50,000)
- Professional thumbnails via Picsum

### Posts (8)
- Tech blog posts about development topics
- SEO-friendly slugs
- Featured images
- Random view counts (500-10,000)

### Comments (80-120)
- Realistic comment templates
- Distributed across videos and posts
- Some with nested replies (up to 3 levels deep)
- Mix of positive, neutral, and questioning comments

### Reactions (50-100)
- All reaction types: Like, Dislike, Love, Clap, Laugh, Sad
- No duplicate reactions (one user can only react once per comment)
- Random distribution

### Reports (5)
- Mix of pending and reviewed reports
- Various report categories (Spam, Harassment, etc.)
- Some with moderator review notes

## Usage

### Method 1: API Endpoint (Recommended)

```bash
# Start the API
dotnet run

# Call the seed endpoint
curl -X POST http://localhost:5043/api/v1/seed

# Or use Swagger UI
open http://localhost:5043/api/swagger
# Find "POST /api/v1/seed" and click "Try it out"
```

### Method 2: Manual Seeding

```csharp
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var seeder = new DatabaseSeeder(dbContext);
await seeder.SeedAllAsync();
```

## Database Setup

1. **Install PostgreSQL** (if not already installed)
   ```bash
   # macOS
   brew install postgresql@16
   brew services start postgresql@16

   # Create database
   createdb commentable
   ```

2. **Configure Connection String**

   Edit `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=commentable;Username=postgres;Password=your_password"
     }
   }
   ```

3. **Run Migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Seed the Database**
   ```bash
   curl -X POST http://localhost:5043/api/v1/seed
   ```

## Seeded Data Summary

After seeding, you'll have:
- ✅ 8 users with different roles
- ✅ 10 videos with realistic content
- ✅ 8 blog posts
- ✅ 80-120 comments (including nested replies)
- ✅ 50-100 reactions
- ✅ 5 reports (mix of pending/reviewed)

## Customization

### Adding More Data

Edit `DatabaseSeeder.cs` and modify the seeding methods:

```csharp
// Add more users
private async Task<List<User>> SeedUsersAsync()
{
    var users = new List<User>
    {
        // Add your custom users here
    };
    // ...
}
```

### Changing Quantities

```csharp
// In SeedVideosAsync
for (int i = 0; i < 20; i++) // Change from 10 to 20
{
    // ...
}
```

### Custom Data Sources

Replace the hard-coded templates with your own:

```csharp
var commentTemplates = new[]
{
    "Your custom comment 1",
    "Your custom comment 2",
    // ...
};
```

## Safety Features

1. **Duplicate Prevention**: Checks if users already exist before seeding
2. **Transaction Support**: All seeding happens in a transaction (rollback on error)
3. **Foreign Key Validation**: Ensures all relationships are valid
4. **Unique Constraints**: Respects database constraints (unique usernames, emails, etc.)

## Troubleshooting

### "Database already contains data"
- This is normal - the seeder won't run if data exists
- To re-seed, drop and recreate the database:
  ```bash
  dotnet ef database drop
  dotnet ef database update
  curl -X POST http://localhost:5043/api/v1/seed
  ```

### Connection Errors
- Verify PostgreSQL is running: `pg_isready`
- Check connection string in `appsettings.Development.json`
- Ensure database exists: `psql -l | grep commentable`

### Password Hashing Errors
- Ensure BCrypt.Net-Next package is installed
- Version should be 4.0.3 or higher

## Production Use

⚠️ **DO NOT use the seeder in production!**

The seeding endpoint is only available in Development environment. For production:
- Remove the seed endpoint
- Use proper data migration scripts
- Never seed fake data in production

## Next Steps

After seeding:
1. Test authentication with seeded users
2. Explore the data in Swagger UI
3. Build API endpoints for CRUD operations
4. Connect the frontend to display seeded data

## Related Files

- `Infrastructure/Data/ApplicationDbContext.cs` - EF Core context
- `Domain/Entities/*.cs` - Entity models
- `Program.cs` - Database configuration and seeding endpoint
