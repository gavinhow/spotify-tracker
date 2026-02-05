# Repository Guidelines

## Project Structure & Module Organization
- `Services/`: ASP.NET Core 8 backend (GraphQL via HotChocolate), EF Core, importer services, and Azure Functions.
- `frontend/`: Next.js 15 app (App Router), Apollo Client, Tailwind, and generated GraphQL types in `frontend/__generated__/`.
- `scripts/`: operational scripts (e.g., data anonymization, dump/restore).
- `docs/`, `infrastructure/`, `docker-compose*.yml`: documentation and deployment/dev infra.
- `AzureComponents/`: Azure-related assets and configuration.

## Build, Test, and Development Commands
- `dotnet build Services/SpotifyStatistics.sln`: build all backend services.
- `dotnet run --project Services/Gavinhow.SpotifyStatistics.Web`: run GraphQL API (default `http://localhost:5000`).
- `dotnet run --project Services/Gavinhow.SpotifyStatistics.Importer`: run background importer.
- `docker-compose up -d`: start PostgreSQL and supporting services.
- `pnpm install`: install frontend dependencies (pnpm is enforced).
- `pnpm dev`: run Next.js dev server with Turbopack.
- `pnpm build` / `pnpm start`: production build and start.
- `pnpm compile`: regenerate GraphQL TypeScript types from the backend schema.
- `pnpm lint`: run ESLint (Next.js config).

## Coding Style & Naming Conventions
- C#: use standard .NET conventions (PascalCase for types/methods, camelCase for locals, `I` prefix for interfaces).
- TypeScript/React: components in PascalCase, hooks/utilities in camelCase; keep file names aligned to component names.
- Prefer existing patterns in `Services/Gavinhow.SpotifyStatistics.Web/Queries/` and `frontend/components/`.
- Formatting is enforced by tooling (`dotnet` defaults, Next.js ESLint). Run `pnpm lint` before PRs.

## Testing Guidelines
- No dedicated test projects are currently configured.
- If you add tests, place .NET tests in a `Services/*.Tests` project and frontend tests under `frontend/` (e.g., `__tests__/`).
- Keep test names descriptive and aligned to feature names or query handlers.

## Commit & Pull Request Guidelines
- Commit messages follow Conventional Commits: `feat:`, `chore:`, `refactor:`, `ci:` (short, imperative subject).
- PRs should include: summary of changes, linked issue (if any), and screenshots for UI updates.
- Call out schema changes and remember to run `pnpm compile` when GraphQL types change.

## Configuration & Secrets
- Backend settings live in `appsettings.json` / `appsettings.Development.json`.
- Frontend uses `.env` (see `.env.example`). Never commit real secrets.
