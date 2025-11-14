# Getting Started with C# SvelteKit Commentable Project

Welcome! This guide will help you get started with this MonoRepo project.

## What's Been Built

This is a **production-ready starter MonoRepo** with:

### ✅ Complete Project Structure
- Turborepo configuration for efficient builds
- Clean directory structure following best practices
- Properly configured .gitignore and prettier

### ✅ Shared Packages
- **@commentable/shared-enums**: TypeScript enums (CommentableType, ReactionType, ReportCategory, UserRole, EntityStatus)
- **@commentable/shared-types**: Complete type definitions for all entities and DTOs
- **@commentable/shared-constants**: Validation rules, cache TTL, rate limits, etc.

### ✅ C# .NET 9 API Foundation
- Clean Architecture structure (Domain, Application, Infrastructure, Presentation)
- All enums created and ready to use
- NuGet packages installed:
  - Entity Framework Core 9.0
  - PostgreSQL provider (Npgsql)
  - Redis caching (StackExchangeRedis)
  - AutoMapper
  - FluentValidation
- Directory structure for entities, repositories, services, controllers

### ✅ Documentation
- Complete ERD with Mermaid diagrams
- API README with architecture details
- Comprehensive project documentation
- Database schema design
- Caching strategies
- Best practices guide

### ✅ Development Tools
- **./run.sh** - Powerful script to manage the entire MonoRepo
- Supports: install, dev, build, test, clean, migrate

## Next Steps to Complete the Project

### 1. Create Database Entities (apps/api/Domain/Entities/)

You need to create the C# entity classes:

```csharp
// Example: User.cs
public class User : BaseEntity
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public EntityStatus Status { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<Video> Videos { get; set; }
    public ICollection<Post> Posts { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
```

Create similar entities for:
- BaseEntity (with Id, CreatedAt, UpdatedAt, DeletedAt)
- Video
- Post
- Comment (with polymorphic relationship support)
- Reaction
- Report

### 2. Create DbContext (apps/api/Infrastructure/Data/ApplicationDbContext.cs)

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships, indexes, constraints
        // See docs/erd.md for detailed schema
    }
}
```

### 3. Create Repositories (apps/api/Infrastructure/Repositories/)

Implement repository pattern for each entity:

```csharp
public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comment> GetByIdAsync(Guid id)
    {
        return await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // Implement other methods...
}
```

### 4. Create Application Services (apps/api/Application/Services/)

Business logic layer:

```csharp
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICacheService _cacheService;

    public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentDto dto)
    {
        // Validate
        // Create entity
        // Save to database
        // Invalidate cache
        // Return DTO
    }
}
```

### 5. Create Controllers (apps/api/Presentation/Controllers/)

API endpoints:

```csharp
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> Create([FromBody] CreateCommentDto dto)
    {
        var result = await _commentService.CreateCommentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Implement other endpoints...
}
```

### 6. Configure Program.cs

Register all services:

```csharp
// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Redis caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
// ... other repositories

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
// ... other services

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

### 7. Create Database Migrations

```bash
cd apps/api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 8. Set Up SvelteKit Frontend (apps/web/)

```bash
cd apps/web
npm create svelte@latest .
# Choose: Skeleton project, TypeScript, ESLint, Prettier

# Install dependencies
npm install
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# Install shared packages
npm install ../../packages/shared-enums
npm install ../../packages/shared-types
npm install ../../packages/shared-constants
```

### 9. Create Frontend Structure

```
apps/web/src/
├── lib/
│   ├── api/           # API client functions
│   ├── components/    # Reusable components
│   │   ├── Comment.svelte
│   │   ├── CommentList.svelte
│   │   ├── ReactionButton.svelte
│   │   └── ReportModal.svelte
│   ├── stores/        # Svelte stores
│   │   ├── auth.ts
│   │   ├── comments.ts
│   │   └── reactions.ts
│   └── utils/         # Utility functions
└── routes/
    ├── +page.svelte   # Home page
    ├── videos/
    │   └── [id]/
    │       └── +page.svelte
    └── posts/
        └── [id]/
            └── +page.svelte
```

### 10. Environment Configuration

Create environment files:

**apps/api/appsettings.Development.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=commentable;Username=postgres;Password=your_password"
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "commentable:"
  },
  "JWT": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "CommentableAPI",
    "Audience": "CommentableClient",
    "ExpirationMinutes": 15
  }
}
```

**apps/web/.env**:
```env
PUBLIC_API_URL=http://localhost:5000/api
```

## Quick Start Commands

```bash
# 1. Install PostgreSQL and Redis (if not installed)
# macOS:
brew install postgresql redis
brew services start postgresql
brew services start redis

# 2. Install project dependencies
./run.sh install

# 3. Configure environment variables
# Edit apps/api/appsettings.Development.json
# Edit apps/web/.env

# 4. Create and run migrations (after completing step 1-2 above)
cd apps/api
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ../..

# 5. Start development servers
./run.sh dev
```

## Project Status

### ✅ Completed
- [x] MonoRepo structure with Turborepo
- [x] Shared packages (enums, types, constants)
- [x] C# .NET 9 API project structure
- [x] Database schema design and ERD
- [x] Comprehensive documentation
- [x] Run script for easy development
- [x] All enums defined (both TS and C#)
- [x] Complete type definitions
- [x] Architecture patterns defined

### 🔨 To Do (Next Steps)
- [ ] Create C# entity classes
- [ ] Implement DbContext and configurations
- [ ] Create repository implementations
- [ ] Implement application services
- [ ] Create API controllers
- [ ] Set up authentication/authorization
- [ ] Initialize SvelteKit project
- [ ] Create frontend components
- [ ] Implement API client
- [ ] Add caching layer
- [ ] Write tests

## Learning Resources

### C# / .NET
- [Entity Framework Core Docs](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### SvelteKit
- [SvelteKit Docs](https://kit.svelte.dev/docs)
- [Svelte Tutorial](https://svelte.dev/tutorial)
- [SvelteKit Best Practices](https://kit.svelte.dev/docs/best-practices)

### Database
- [PostgreSQL Docs](https://www.postgresql.org/docs/)
- [Redis Docs](https://redis.io/documentation)

## Estimated Time to Complete

Based on the current foundation:

- **Basic CRUD**: 8-12 hours
  - Entities, DbContext, Repositories: 3-4 hours
  - Services and Controllers: 3-4 hours
  - Frontend components: 2-4 hours

- **Full Features**: 20-30 hours
  - Above + Authentication: +4-6 hours
  - + Reactions & Reports: +3-4 hours
  - + Caching & Optimization: +3-4 hours
  - + Testing: +4-6 hours

- **Production Ready**: 40-60 hours
  - Above + Error handling: +4-6 hours
  - + Comprehensive tests: +6-8 hours
  - + Documentation: +4-6 hours
  - + Deployment setup: +6-10 hours

## Support

If you have questions:
1. Check the docs in `/docs/` directory
2. Review the ERD diagram for database structure
3. Check the API README for architecture patterns
4. Look at the shared types for DTOs and interfaces

## Tips for Success

1. **Start Simple**: Implement basic CRUD for one entity (e.g., Comments) first
2. **Test Early**: Write tests as you build features
3. **Follow the Pattern**: Use existing structure as a template
4. **Check Types**: Use shared types to ensure consistency
5. **Document**: Add comments for complex business logic
6. **Use run.sh**: Leverage the run script for common tasks

Happy coding! 🚀
