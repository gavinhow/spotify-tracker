# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Architecture

This is a full-stack Spotify listening statistics application with the following architecture:

### Backend (.NET)
- **Web API**: ASP.NET Core 8.0 GraphQL API using HotChocolate (`Services/Gavinhow.SpotifyStatistics.Web/`)
- **Database Layer**: Entity Framework Core with PostgreSQL (`Services/Gavinhow.SpotifyStatistics.Database/`)
- **Entities**: Data models and domain objects (`Services/Gavinhow.SpotifyStatistics.Database.Entity/`)
- **API Facade**: Spotify API integration layer (`Services/Gavinhow.SpotifyStatistics.Api/`)
- **Import Function**: Azure Functions for data import (`Services/Gavinhow.SpotifyStatistics.ImportFunction/`)
- **Importer Service**: Background worker service for continuous data import (`Services/Gavinhow.SpotifyStatistics.Importer/`)

### Frontend (Next.js)
- **Framework**: Next.js 15 with App Router (`frontend/`)
- **UI**: React components using Tailwind CSS and Radix UI
- **GraphQL**: Apollo Client with code generation from backend schema
- **Authentication**: JWT-based auth with Spotify OAuth integration

### Database
- PostgreSQL database with Entity Framework migrations
- Docker Compose setup for local development
- Seed data available in `Services/Gavinhow.SpotifyStatistics.Database/SeedData/`

## Development Commands

### Backend (.NET)
```bash
# Build all projects
dotnet build Services/SpotifyStatistics.sln

# Run the main web API (from Services/ directory)
dotnet run --project Gavinhow.SpotifyStatistics.Web

# Run database migrations
dotnet ef database update --project Gavinhow.SpotifyStatistics.Database --startup-project Gavinhow.SpotifyStatistics.Web

# Run the importer service
dotnet run --project Gavinhow.SpotifyStatistics.Importer
```

### Frontend (Next.js)
```bash
# Install dependencies (from frontend/ directory)
pnpm install

# Run development server
pnpm dev

# Build for production
pnpm build

# Run linting
pnpm lint

# Generate GraphQL types from backend schema
pnpm compile

# Watch and regenerate GraphQL types
pnpm watch
```

### Infrastructure
```bash
# Start local PostgreSQL database
docker-compose up -d

# Stop database
docker-compose down
```

## Key Architecture Decisions

- **GraphQL API**: Uses HotChocolate for type-safe GraphQL schema with authorization
- **Authentication**: Dual auth scheme supporting both JWT Bearer tokens and API keys
- **Database**: PostgreSQL with EF Core migrations, designed to handle large volumes of play history data
- **Import Strategy**: Separate services for Azure Functions (cloud) and Worker Service (local) data import
- **Frontend State**: Apollo Client for GraphQL state management with generated TypeScript types
- **UI Components**: Shadcn/ui component library built on Radix UI primitives

## Project Dependencies

- Backend targets .NET 8.0
- Frontend uses Node.js with pnpm package manager
- Database requires PostgreSQL
- Spotify API integration for OAuth and data fetching
- Azure services for cloud deployment (Functions, App Service)

## Development Environment Setup

1. Start PostgreSQL: `docker-compose up -d`
2. Run backend: `dotnet run --project Services/Gavinhow.SpotifyStatistics.Web`
3. Install frontend deps: `cd frontend && pnpm install`
4. Run frontend: `pnpm dev`
5. Generate GraphQL types: `pnpm compile`