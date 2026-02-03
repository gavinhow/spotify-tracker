# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Full-stack Spotify listening statistics application built with ASP.NET Core 8.0 backend (GraphQL via HotChocolate) and Next.js 15 frontend. The system tracks Spotify listening history and provides analytics and insights.

## Architecture

### Backend (.NET) - `Services/`

**Core Services:**
- **Web API** (`Gavinhow.SpotifyStatistics.Web/`) - GraphQL API entry point with authorization, authentication, health checks, and Prometheus metrics
- **Database** (`Gavinhow.SpotifyStatistics.Database/`) - EF Core DbContext and migrations for PostgreSQL
- **Database.Entity** (`Gavinhow.SpotifyStatistics.Database.Entity/`) - Domain entities and data models
- **Api** (`Gavinhow.SpotifyStatistics.Api/`) - Spotify API client integration and service abstraction
- **Importer** (`Gavinhow.SpotifyStatistics.Importer/`) - Background worker service for continuous data sync
- **ImportFunction** (`Gavinhow.SpotifyStatistics.ImportFunction/`) - Azure Functions for cloud-based data import

**Key Patterns:**
- GraphQL queries organized in `Services/Gavinhow.SpotifyStatistics.Web/Queries/` (GetTopSongsQuery, GetTopArtistsQuery, GetTopAlbumsQuery, GetPlaysQuery, etc.)
- GraphQL types in `Types/` folder with root Query type coordinating all queries
- Authorization via `Authorization/Handler` and `Authorization/Requirements` for JWT validation
- CQRS pattern with IQuery interface in `CQRS/` for query abstraction
- Entity Framework Core 6.0.4 (note: version mismatch with target framework net8.0, check for potential updates)
- HotChocolate 14.3.0 with Data and Authorization packages

### Frontend (Next.js) - `frontend/`

**Structure:**
- `app/` - Next.js App Router pages and layouts
- `components/` - React components (UI library using Radix UI + Tailwind CSS + shadcn/ui)
- `lib/` - Utilities including `client.ts` (Apollo Client setup)
- `__generated__/` - GraphQL code generation output (auto-generated from schema)
- `middleware.ts` - Authentication middleware

**Key Setup:**
- Apollo Client 3.12.4 with experimental Next.js App Router support
- GraphQL code generation via `@graphql-codegen/client-preset`
- TypeScript with strict type checking
- No test framework configured (Jest/Vitest not in dependencies)

### Database

- PostgreSQL with EF Core migrations
- Docker Compose for local development (`docker-compose.yml` and `docker-compose.prod.yml`)
- Seed data in `Services/Gavinhow.SpotifyStatistics.Database/SeedData/`

## Development Commands

### Backend (.NET)

From `Services/` directory or repository root:

```bash
# Build
dotnet build Services/SpotifyStatistics.sln

# Run Web API (listens on port 5000)
dotnet run --project Services/Gavinhow.SpotifyStatistics.Web

# Run Importer service
dotnet run --project Services/Gavinhow.SpotifyStatistics.Importer

# Create migration (from Services/ directory)
dotnet ef migrations add MigrationName --project Gavinhow.SpotifyStatistics.Database --startup-project Gavinhow.SpotifyStatistics.Web

# Apply migrations
dotnet ef database update --project Gavinhow.SpotifyStatistics.Database --startup-project Gavinhow.SpotifyStatistics.Web

# Watch for changes (requires dotnet watch tool)
dotnet watch run --project Services/Gavinhow.SpotifyStatistics.Web
```

### Frontend (Next.js)

From `frontend/` directory:

```bash
# Install dependencies (uses pnpm)
pnpm install

# Development server (listens on port 3000 with Turbopack)
pnpm dev

# Production build
pnpm build

# Start production server
pnpm start

# Linting
pnpm lint

# Generate TypeScript types from GraphQL schema
pnpm compile

# Watch and regenerate on schema changes
pnpm watch
```

### Infrastructure

```bash
# Start PostgreSQL and other services
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f
```

## Important Architecture Details

### GraphQL Schema
- **Root Query** defined in `Services/Gavinhow.SpotifyStatistics.Web/Types/Query.cs`
- **Query Handlers** in `Queries/` folder using CQRS pattern with IQuery interface
- Schema introspection available at `/graphql` when API runs
- Authorization via HotChocolate Authorize attribute on queries/types

### Authentication & Authorization
- JWT Bearer tokens generated and validated in Program.cs
- Dual auth support: JWT tokens and API keys
- Custom authorization handlers in `Authorization/Handler/`
- Frontend middleware in `frontend/src/middleware.ts` handles auth flow

### Data Flow
1. Frontend authenticates user via Spotify OAuth
2. Backend validates JWT tokens on each GraphQL request
3. CQRS queries fetch data from PostgreSQL via EF Core
4. Results sent to Apollo Client for caching and UI updates
5. Background importer syncs latest data from Spotify API

### Configuration
- Backend settings in `appsettings.json` and `appsettings.Development.json`
- Frontend environment via `.env` file (see `.env.example`)
- Spotify OAuth credentials required in both backend and frontend configs

## Development Tips

- **HotChocolate Analyzer**: The types analyzer package provides compile-time validation of GraphQL schema
- **PostgreSQL**: Default local connection string in docker-compose uses `user:password` credentials
- **GraphQL Code Generation**: Always run `pnpm compile` after backend schema changes to update frontend types
- **Apollo Client**: Configured with `@apollo/experimental-nextjs-app-support` for App Router integration
- **Turbopack**: Frontend uses Next.js Turbopack for faster dev rebuilds
- **pnpm**: Monorepo package manager (enforced via preinstall script)