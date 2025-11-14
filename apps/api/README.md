# CommentableAPI - C# .NET 9 REST API

Production-ready REST API built with C# .NET 9 implementing a comprehensive comments CRUD system with polymorphic relationships.

## Architecture

This API follows **Clean Architecture** principles with clear separation of concerns:

```
apps/api/
├── Domain/                    # Core business logic and entities
│   ├── Entities/             # Domain entities (User, Comment, Video, etc.)
│   ├── Enums/                # Shared enumerations
│   └── Interfaces/           # Domain interfaces
├── Application/               # Application business logic
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Services/             # Business logic services
│   ├── Interfaces/           # Service interfaces
│   └── Mappings/             # AutoMapper profiles
├── Infrastructure/            # External concerns
│   ├── Data/                 # EF Core DbContext and configurations
│   ├── Repositories/         # Repository implementations
│   └── Caching/              # Redis caching implementation
└── Presentation/              # API layer
    ├── Controllers/          # API controllers
    ├── Middleware/           # Custom middleware
    └── Filters/              # Action filters
```

## Tech Stack

- **.NET 9.0** - Latest LTS framework
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Primary database with Npgsql provider
- **Redis** - Distributed caching via StackExchangeRedis
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Request validation
- **Swagger/OpenAPI** - API documentation (Swashbuckle.AspNetCore 6.5.0)

## Design Patterns

### Repository Pattern
Abstraction layer over data access logic:
- `IUserRepository`, `ICommentRepository`, etc.
- Promotes testability and loose coupling
- See: `Domain/Interfaces/` and `Infrastructure/Repositories/`

### Unit of Work Pattern
Coordinates multiple repository operations in a single transaction:
- Ensures data consistency
- See: `Infrastructure/Data/UnitOfWork.cs`

### Dependency Injection
All services and repositories registered in `Program.cs`:
- Scoped lifetime for repositories and DbContext
- Singleton for caching services
- Transient for application services

### SOLID Principles

#### Single Responsibility Principle (SRP)
- Each entity has a focused purpose
- Controllers handle HTTP concerns only
- Services contain business logic
- Repositories handle data access

#### Open/Closed Principle (OCP)
- Polymorphic comments allow extending to new commentable types
- Use interfaces for extensibility

#### Liskov Substitution Principle (LSP)
- All commentable entities implement `ICommentable` interface
- Repository implementations are interchangeable

#### Interface Segregation Principle (ISP)
- Separate interfaces for different operations (Read, Write, Delete)
- Clients depend only on methods they use

#### Dependency Inversion Principle (DIP)
- Controllers depend on service interfaces, not implementations
- Services depend on repository interfaces

## Entities

### User
- Manages user accounts and authentication
- Supports roles: Guest, User, Moderator, Admin
- Soft delete capability

### Video
- Video content that can receive comments
- Tracks views and comment counts
- Polymorphic commentable entity

### Post
- Blog posts or social media posts
- Publishing workflow (draft/published)
- Polymorphic commentable entity

### Comment
- **Polymorphic commenting** via `CommentableType` and `CommentableId`
- **Nested comments** via `ParentCommentId`
- Denormalized counts for performance

### Reaction
- Multiple reaction types (Like, Dislike, Love, Clap, Laugh, Sad)
- Composite unique constraint prevents duplicate reactions

### Report
- User reports of inappropriate comments
- Moderation workflow: Pending → Reviewed → Resolved/Dismissed

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - Login
- `POST /api/v1/auth/logout` - Logout
- `POST /api/v1/auth/refresh` - Refresh token
- `GET /api/v1/auth/me` - Get current user

### Comments
- `GET /api/v1/comments` - List comments (with filtering/pagination)
- `GET /api/v1/comments/{id}` - Get comment by ID
- `POST /api/v1/comments` - Create comment
- `PUT /api/v1/comments/{id}` - Update comment
- `DELETE /api/v1/comments/{id}` - Delete comment (soft delete)
- `GET /api/v1/comments/{id}/replies` - Get comment replies

### Videos
- `GET /api/v1/videos` - List videos
- `GET /api/v1/videos/{id}` - Get video by ID
- `POST /api/v1/videos` - Create video
- `PUT /api/v1/videos/{id}` - Update video
- `DELETE /api/v1/videos/{id}` - Delete video
- `GET /api/v1/videos/{id}/comments` - Get video comments

### Posts
- `GET /api/v1/posts` - List posts
- `GET /api/v1/posts/{id}` - Get post by ID
- `POST /api/v1/posts` - Create post
- `PUT /api/v1/posts/{id}` - Update post
- `DELETE /api/v1/posts/{id}` - Delete post
- `GET /api/v1/posts/{id}/comments` - Get post comments

### Reactions
- `GET /api/v1/reactions` - List reactions
- `POST /api/v1/reactions/upsert` - Create or update reaction
- `DELETE /api/v1/reactions/{id}` - Delete reaction

### Reports
- `GET /api/v1/reports` - List reports (moderators only)
- `GET /api/v1/reports/{id}` - Get report by ID
- `POST /api/v1/reports` - Create report
- `PUT /api/v1/reports/{id}/review` - Review report (moderators only)

## Database Migrations

```bash
# Create new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Rollback migration
dotnet ef migrations remove
```

## Environment Variables

Create `appsettings.Development.json`:

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
    "SecretKey": "your-secret-key-min-32-characters",
    "Issuer": "CommentableAPI",
    "Audience": "CommentableClient",
    "ExpirationMinutes": 15
  }
}
```

## Running the API

### Development
```bash
# From project root
./run.sh api

# Or from apps/api/
dotnet run
```

### Production
```bash
dotnet build -c Release
dotnet publish -c Release -o ./publish
cd publish
dotnet CommentableAPI.dll
```

## Performance Optimization

### Caching Strategy
- **User profiles**: 5 minutes TTL
- **Comments**: 1 minute TTL
- **Reaction summaries**: 30 seconds TTL
- Invalidation on create/update/delete

### N+1 Query Prevention
- Denormalized counts in entities
- Eager loading with `Include()` for related data
- Projection to DTOs to limit data transfer

### Pagination
- Default page size: 20
- Maximum page size: 100
- Cursor-based pagination for infinite scroll

### Indexing
Critical indexes for performance:
- `Comments(CommentableType, CommentableId)`
- `Comments(ParentCommentId)`
- `Reactions(UserId, CommentId)` - Unique
- `Reports(Status)`

## Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## API Documentation

### Swagger/OpenAPI UI

Interactive API documentation available at:
- **Development**: `http://localhost:5043/api/swagger` (or port from launchSettings.json)
- **Production**: `https://api.example.com/api/swagger`

### OpenAPI Specification

Raw OpenAPI 3.0.1 JSON schema available at:
- **Development**: `http://localhost:5043/api/swagger/v1/swagger.json`
- **Production**: `https://api.example.com/api/swagger/v1/swagger.json`

### Features

The API includes comprehensive Swagger annotations:
- **Operation summaries** with detailed descriptions
- **Request/response examples** for all endpoints
- **Schema documentation** with XML comments
- **JWT Bearer authentication** support in Swagger UI
- **Try it out** functionality for testing endpoints
- **Deep linking** to specific operations
- **Filter/search** capability across all endpoints

### Example Endpoints

The API includes example endpoints demonstrating best practices:
- `GET /api/v1/Example/reaction-types` - Lists all available reaction types with emojis
- `GET /api/v1/Example/user-roles` - Shows user roles and their permissions
- `GET /api/v1/Example/entity-statuses` - Displays entity status values for moderation

### Using Swagger UI

1. Start the API: `dotnet run` or `./run.sh api`
2. Open browser to: `http://localhost:5043/api/swagger`
3. Click "Authorize" to add JWT Bearer token (when auth is implemented)
4. Expand any endpoint and click "Try it out" to test
5. View request/response schemas and examples

## Security

- **Authentication**: JWT Bearer tokens
- **Authorization**: Role-based access control
- **Validation**: FluentValidation for all requests
- **SQL Injection**: Protection via parameterized queries (EF Core)
- **XSS**: Content sanitization in DTOs
- **Rate Limiting**: Implemented via middleware

## Code Quality

- **No code smells**: Regular refactoring
- **SOLID principles**: Throughout architecture
- **DRY**: Shared logic in base classes and services
- **Proper error handling**: Global exception middleware
- **Logging**: Structured logging with Serilog (optional)

## Contributing

1. Follow conventional commits
2. Write unit tests for new features
3. Update API documentation
4. Run linting before committing

## License

MIT License
