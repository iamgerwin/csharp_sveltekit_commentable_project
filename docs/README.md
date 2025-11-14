# Project Documentation

Complete documentation for the C# SvelteKit Commentable MonoRepo project.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Getting Started](#getting-started)
3. [Architecture](#architecture)
4. [Development Workflow](#development-workflow)
5. [Documentation Index](#documentation-index)

## Project Overview

This is a production-ready MonoRepo implementing a comprehensive comments CRUD system with:

- **Backend**: C# .NET 9 REST API with Clean Architecture
- **Frontend**: SvelteKit 2.48.4 with TypeScript
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis for distributed caching
- **MonoRepo**: Turborepo for efficient builds

### Key Features

✅ **Polymorphic Comments System**: Comments can be attached to Videos, Posts, or any commentable entity
✅ **User Management**: Role-based access control (Guest, User, Moderator, Admin)
✅ **Reactions System**: Multiple reaction types (Like, Dislike, Love, Clap, Laugh, Sad)
✅ **Report System**: Flag inappropriate comments with moderation workflow
✅ **Nested Comments**: Threaded discussions with reply support
✅ **Real-time Updates**: Optimistic UI updates with SvelteKit stores
✅ **Performance Optimized**: Caching, pagination, N+1 prevention, denormalized counts
✅ **Type Safety**: Shared enums and types between frontend and backend

## Getting Started

### Prerequisites

- Node.js >= 20.0.0
- .NET SDK 9.0
- PostgreSQL 16+
- Redis 7+

### Quick Start

```bash
# Clone the repository
git clone <repository-url>
cd csharp_sveltekit_commentable_project

# Make run script executable
chmod +x ./run.sh

# Check prerequisites
./run.sh check

# Install all dependencies
./run.sh install

# Configure environment variables
cp apps/api/.env.example apps/api/.env
cp apps/web/.env.example apps/web/.env
# Edit .env files with your configuration

# Run database migrations
./run.sh migrate

# Start all services
./run.sh dev
```

### Run Script Commands

The `./run.sh` script simplifies running the MonoRepo:

- `./run.sh check` - Check prerequisites (Node, .NET, etc.)
- `./run.sh install` - Install all dependencies (npm + .NET packages)
- `./run.sh migrate` - Run database migrations
- `./run.sh dev` - Start all services (API + Web)
- `./run.sh api` - Run API server only
- `./run.sh web` - Run Web server only
- `./run.sh build` - Build all projects
- `./run.sh test` - Run all tests
- `./run.sh clean` - Clean all build artifacts

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     SvelteKit Frontend                       │
│  (TypeScript, TailwindCSS, Svelte Stores)                   │
└────────────────────┬────────────────────────────────────────┘
                     │ HTTP/REST
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   C# .NET 9 REST API                         │
│  Controllers → Services → Repositories → DbContext          │
└────────────────────┬────────────────────────────────────────┘
                     │
        ┌────────────┴──────────────┐
        ▼                           ▼
┌──────────────────┐       ┌──────────────────┐
│   PostgreSQL      │       │      Redis       │
│  (Primary DB)     │       │    (Caching)     │
└──────────────────┘       └──────────────────┘
```

### Project Structure

```
csharp_nextjs_commentable_project/
├── apps/
│   ├── api/              # C# .NET 9 REST API
│   │   ├── Domain/       # Entities, Enums, Interfaces
│   │   ├── Application/  # Business Logic, DTOs, Services
│   │   ├── Infrastructure/ # Data Access, Repositories, Caching
│   │   └── Presentation/ # Controllers, Middleware, Filters
│   └── web/              # SvelteKit Frontend
│       ├── src/
│       │   ├── lib/      # Reusable components
│       │   ├── routes/   # SvelteKit routes
│       │   └── stores/   # State management
│       └── static/       # Static assets
├── packages/
│   ├── shared-enums/     # Shared enums (TS)
│   ├── shared-types/     # Shared types (TS)
│   └── shared-constants/ # Shared constants (TS)
├── docs/                 # Documentation
│   ├── erd.md           # Entity Relationship Diagram
│   └── README.md        # This file
├── run.sh               # MonoRepo runner script
├── turbo.json           # Turborepo configuration
└── package.json         # Root package.json
```

## Development Workflow

### Creating a New Feature

1. **Plan the Feature**
   - Review existing architecture
   - Identify affected entities and services
   - Create/update database migrations if needed

2. **Backend Development**
   ```bash
   cd apps/api

   # Create entity
   # Add to Domain/Entities/

   # Create repository interface
   # Add to Domain/Interfaces/

   # Implement repository
   # Add to Infrastructure/Repositories/

   # Create DTOs
   # Add to Application/DTOs/

   # Create service
   # Add to Application/Services/

   # Create controller
   # Add to Presentation/Controllers/

   # Run migrations
   dotnet ef migrations add FeatureName
   dotnet ef database update
   ```

3. **Frontend Development**
   ```bash
   cd apps/web

   # Create API client
   # Add to src/lib/api/

   # Create components
   # Add to src/lib/components/

   # Create routes
   # Add to src/routes/

   # Create stores if needed
   # Add to src/stores/
   ```

4. **Testing**
   ```bash
   # Test backend
   cd apps/api
   dotnet test

   # Test frontend
   cd apps/web
   npm test
   ```

5. **Commit and Push**
   Follow conventional commits format

### Database Migrations

```bash
# Create migration
cd apps/api
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Rollback migration
dotnet ef migrations remove
```

### Adding Shared Types

When adding types that need to be shared between frontend and backend:

1. **Add TypeScript enum/type**
   ```bash
   # For enums
   cd packages/shared-enums/src
   # Create EnumName.ts

   # For types
   cd packages/shared-types/src
   # Create TypeName.ts
   ```

2. **Create equivalent C# enum/class**
   ```bash
   cd apps/api/Domain
   # Create corresponding C# file
   ```

3. **Build shared packages**
   ```bash
   npm run build --workspace=@commentable/shared-enums
   npm run build --workspace=@commentable/shared-types
   ```

## Documentation Index

- [Entity Relationship Diagram](./erd.md) - Complete database schema with Mermaid diagrams
- [API Documentation](../apps/api/README.md) - C# .NET API architecture and endpoints
- [Web Documentation](../apps/web/README.md) - SvelteKit frontend structure
- [PWA Documentation](./PWA.md) - Progressive Web App features and implementation
- [Root README](../README.md) - Project overview and quick start

### Mermaid Diagrams

- [ERD Diagram](./erd.md) - Entity relationships and database design
- Database indexes and optimization strategies
- Caching architecture
- N+1 query prevention strategies

## Best Practices

### Code Organization

✅ **Separation of Concerns**: Clear boundaries between layers
✅ **Single Responsibility**: Each class has one reason to change
✅ **DRY Principle**: No code duplication
✅ **Naming Conventions**: Consistent naming across project
✅ **Comments**: Document complex business logic

### Performance

✅ **Caching**: Multi-level caching (Redis + Memory)
✅ **Pagination**: All list endpoints support pagination
✅ **Eager Loading**: Prevent N+1 queries
✅ **Denormalization**: Store computed values (counts)
✅ **Indexes**: Proper database indexing

### Security

✅ **Authentication**: JWT-based authentication
✅ **Authorization**: Role-based access control
✅ **Validation**: Input validation on all endpoints
✅ **SQL Injection**: Parameterized queries via EF Core
✅ **XSS Prevention**: Content sanitization

### Testing

✅ **Unit Tests**: Test business logic in isolation
✅ **Integration Tests**: Test API endpoints
✅ **E2E Tests**: Test critical user flows
✅ **Code Coverage**: Aim for >80% coverage

## Troubleshooting

### Common Issues

**PostgreSQL Connection Error**
```bash
# Check if PostgreSQL is running
pg_isready

# Check connection string in appsettings.json
```

**Redis Connection Error**
```bash
# Check if Redis is running
redis-cli ping

# Should return PONG
```

**EF Core Migration Error**
```bash
# Clear migrations and start fresh
rm -rf Migrations/
dotnet ef database drop
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Node Modules Error**
```bash
# Clean install
rm -rf node_modules package-lock.json
npm install
```

## Contributing

1. Fork the repository
2. Create a feature branch (`feat/feature-name`)
3. Follow conventional commits
4. Write tests for new features
5. Update documentation
6. Submit a pull request

## License

MIT License

## Support

For issues and questions:
- Create an issue on GitHub
- Check existing documentation
- Review code examples in the project
