# Spotify Statistics Tracker

A full-stack application for tracking and analyzing your Spotify listening habits. View your top songs, artists, albums, and detailed play history with beautiful visualizations and insights.

## Features

- 🎵 **Top Tracks Analysis** - See your most played songs with play counts and trends
- 🎤 **Artist Statistics** - Discover your favorite artists and their top albums
- 💿 **Album Insights** - Track your album listening patterns
- 📊 **Play History** - Detailed timeline of your listening activity
- 👥 **Friends Integration** - Compare listening habits with friends
- 📱 **Responsive Design** - Works seamlessly on desktop and mobile
- 🔐 **Spotify OAuth** - Secure authentication with your Spotify account

## Technology Stack

### Backend
- **ASP.NET Core 8.0** - Web API framework
- **HotChocolate** - GraphQL server implementation
- **Entity Framework Core** - Database ORM
- **PostgreSQL** - Primary database
- **Azure Functions** - Serverless data import
- **JWT Authentication** - Secure API access

### Frontend
- **Next.js 15** - React framework with App Router
- **TypeScript** - Type-safe development
- **Apollo Client** - GraphQL client with caching
- **Tailwind CSS** - Utility-first styling
- **Radix UI** - Accessible component primitives
- **Recharts** - Data visualization

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and [pnpm](https://pnpm.io/)
- [Docker](https://www.docker.com/) (for PostgreSQL)
- [Spotify Developer Account](https://developer.spotify.com/)

### Environment Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SpotifyStatistics
   ```

2. **Start PostgreSQL database**
   ```bash
   docker-compose up -d
   ```

3. **Configure Spotify API**
   - Create a Spotify app at [developer.spotify.com](https://developer.spotify.com/dashboard)
   - Add redirect URI: `http://localhost:3000/api/authenticate`
   - Update `Services/Gavinhow.SpotifyStatistics.Web/appsettings.Development.json`:
   ```json
   {
     "Spotify": {
       "ClientId": "your-spotify-client-id",
       "ClientSecret": "your-spotify-client-secret",
       "CallbackUri": "http://localhost:3000/api/authenticate",
       "ServerUri": "http://localhost:5000"
     },
     "ConnectionStrings": {
       "Sql": "Host=localhost;Database=spotify;Username=user;Password=password"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   cd Services
   dotnet ef database update --project Gavinhow.SpotifyStatistics.Database --startup-project Gavinhow.SpotifyStatistics.Web
   ```

5. **Start the backend API**
   ```bash
   dotnet run --project Gavinhow.SpotifyStatistics.Web
   ```
   API will be available at `http://localhost:5000`

6. **Install frontend dependencies**
   ```bash
   cd frontend
   pnpm install
   ```

7. **Generate GraphQL types**
   ```bash
   pnpm compile
   ```

8. **Start the frontend**
   ```bash
   pnpm dev
   ```
   Frontend will be available at `http://localhost:3000`

## Development

### Backend Commands
```bash
# Build all projects
dotnet build Services/SpotifyStatistics.sln

# Run the web API
dotnet run --project Services/Gavinhow.SpotifyStatistics.Web

# Run the importer service
dotnet run --project Services/Gavinhow.SpotifyStatistics.Importer

# Create new migration
dotnet ef migrations add MigrationName --project Services/Gavinhow.SpotifyStatistics.Database --startup-project Services/Gavinhow.SpotifyStatistics.Web
```

### Frontend Commands
```bash
# Development server
pnpm dev

# Production build
pnpm build

# Lint code
pnpm lint

# Generate GraphQL types
pnpm compile

# Watch for GraphQL schema changes
pnpm watch
```

## Architecture

### Data Flow
1. **Authentication**: Users authenticate via Spotify OAuth
2. **Data Import**: Background services import listening history from Spotify API
3. **API Layer**: GraphQL API serves data with authorization
4. **Frontend**: Next.js app consumes GraphQL API and displays insights

### Key Components
- **GraphQL Schema**: Type-safe API with filtering and projections
- **Authorization**: JWT tokens with user-specific data access
- **Data Import**: Incremental import of Spotify play history
- **Caching**: Memory caching for frequently accessed data
- **Migrations**: Entity Framework for database schema management

## Deployment

### Docker (Recommended)

The application is designed for easy self-hosting using Docker Compose.

#### Production Deployment with Docker

1. **Clone and configure**
   ```bash
   git clone <repository-url>
   cd SpotifyStatistics
   cp .env.example .env
   # Edit .env with your configuration
   ```

2. **Deploy with Docker Compose**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d
   ```

3. **Run database migrations**
   ```bash
   docker-compose -f docker-compose.prod.yml exec web-api dotnet ef database update
   ```

The production setup includes:
- PostgreSQL database with automated backups
- ASP.NET Core Web API with health checks
- Next.js frontend with optimized builds
- Background importer service
- Nginx reverse proxy with SSL support
- Comprehensive logging and monitoring

For detailed setup instructions, see [docs/docker-setup.md](docs/docker-setup.md).

For database backup procedures, see [docs/database-backup.md](docs/database-backup.md).

#### Traditional Cloud Deployment
- Azure App Service for the Web API
- Azure Functions for data import
- Azure Database for PostgreSQL
- CI/CD via Azure Pipelines

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For questions or issues, please open a GitHub issue or contact the maintainers.