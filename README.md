# C# SvelteKit Commentable Project

A production-ready MonoRepo project with C# .NET 9 backend and SvelteKit frontend implementing a comprehensive comments CRUD system with polymorphic relationships.

## Project Architecture

```
csharp_nextjs_commentable_project/
├── apps/
│   ├── api/          # C# .NET 9 REST API with Swagger/OpenAPI
│   └── web/          # SvelteKit 2.48.4 Frontend
├── packages/
│   ├── shared-enums/      # Shared TypeScript/C# enums
│   ├── shared-types/      # Shared TypeScript types
│   └── shared-constants/  # Shared constants
└── docs/             # Project documentation & diagrams
```

## Tech Stack

### Backend (API)
- **Runtime**: .NET 9 (C# 13)
- **Database**: PostgreSQL with Entity Framework Core 9.0
- **Caching**: Redis (Distributed Cache)
- **Documentation**: Swagger/OpenAPI (Swashbuckle 6.5.0)
- **Patterns**: Repository Pattern, SOLID Principles, Unit of Work
- **Architecture**: Clean Architecture, Dependency Injection

### Frontend (Web)
- **Framework**: SvelteKit 2.48.4
- **Language**: TypeScript
- **Styling**: TailwindCSS
- **State Management**: Svelte Stores

### Infrastructure
- **MonoRepo**: Turborepo
- **Package Manager**: npm workspaces
- **Documentation**: Mermaid diagrams

## Features

### Core Features
- **Polymorphic Comments System**: Comments can be attached to Videos, Posts, or other entities
- **User Management**: User authentication and profile management
- **CRUD Operations**: Full Create, Read, Update, Delete for all entities

### Advanced Features
- **Reactions System**: Multiple reaction types (Like, Dislike, Love, Clap, etc.)
- **Report Comments**: Flag inappropriate comments with categories
- **Pagination**: Efficient pagination to prevent N+1 queries
- **Caching**: Multi-level caching strategy (Redis, Memory)
- **Queue System**: Background job processing for heavy operations

## Getting Started

### Prerequisites
- Node.js >= 20.0.0
- .NET SDK 9.0 or 10.0
- PostgreSQL 16+
- Redis 7+

### Quick Links
- **API Documentation (Swagger)**: `http://localhost:5043/api/swagger`
- **Web App**: `http://localhost:5173` (or 5174)
- **Health Check**: `http://localhost:5043/api/v1/health`
- **Example Endpoints**: `http://localhost:5043/api/v1/Example/reaction-types`

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd csharp_nextjs_commentable_project
```

2. **Install dependencies**
```bash
npm install
```

3. **Set up environment variables**
```bash
# Copy example env files
cp apps/api/.env.example apps/api/.env
cp apps/web/.env.example apps/web/.env
```

4. **Run database migrations**
```bash
cd apps/api
dotnet ef database update
```

5. **Start development servers**
```bash
# Start all services
npm run dev

# Or run individually
npm run api:dev  # API only
npm run web:dev  # Web only
```

## Development

### Available Scripts

- `npm run dev` - Start all services in development mode
- `npm run build` - Build all applications
- `npm run test` - Run all tests
- `npm run lint` - Lint all code
- `npm run format` - Format code with Prettier
- `npm run clean` - Clean all build artifacts

### Project Structure Details

See individual README files in each directory:
- [API Documentation](./apps/api/README.md)
- [Web Documentation](./apps/web/README.md)
- [Architecture Documentation](./docs/README.md)

## Database Schema

See [Entity Relationship Diagram](./docs/erd.md) for detailed database structure.

## Best Practices Implemented

- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Design Patterns**: Repository, Unit of Work, Factory, Strategy, Dependency Injection
- **Performance**: Caching layers, query optimization, N+1 prevention, pagination
- **Code Quality**: No code smells, proper separation of concerns, comprehensive documentation
- **Type Safety**: Shared enums and types between frontend and backend
- **Scalability**: Queue system for async operations, distributed caching

## Contributing

Follow conventional commits format for all commit messages.

## License

MIT License
