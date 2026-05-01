# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build
dotnet build --configuration Release

# Run all tests
dotnet test

# Run a specific test project
dotnet test Tests/Unit/Unit.csproj
dotnet test Tests/Integration/Integration.csproj
dotnet test Tests/Application/Application.csproj

# Run a single test by name
dotnet test Tests/Unit/Unit.csproj --filter "FullyQualifiedName~GlobalExceptionHandlerTests"

# Run the API
dotnet run --project Src/Api/Api.csproj

# Run the Agent (background worker)
dotnet run --project Src/Agent/Agent.csproj

# First-time setup (installs commitlint hook, restores packages)
./setup.sh
```

## Architecture

This is a **Clean Architecture** solution using **Vertical Slice Architecture** targeting **.NET 10** with five layers and two executables.

### Layer dependency order (innermost → outermost)

```
Domain ← Application ← Infrastructure ← Api
                                       ← Agent (separate executable)
```

- **Domain** (`Src/Domain/`) — Business entities, domain exceptions, pure .NET with no external dependencies.
- **Application** (`Src/Application/`) — Use cases, CQRS handlers, validators. Currently empty; intended for MediatR commands/queries when added.
- **Infrastructure** (`Src/Infrastructure/`) — Data access, external service clients. Currently empty; EF Core + repositories are a planned addition.
- **Api** (`Src/Api/`) — ASP.NET Core 10 minimal API entry point. Hosts REST endpoints, OpenAPI, versioning, Serilog, and global exception handling.
- **Agent** (`Src/Agent/`) — Separate Worker Service executable for background jobs and scheduled tasks. Shares no startup code with Api.

### Exception handling pipeline

All domain exceptions in `Src/Domain/Exceptions/` map to RFC 9457 ProblemDetails via `GlobalExceptionHandler` (implements `IExceptionHandler`):

| Exception | HTTP status |
|---|---|
| `NotFoundException` | 404 |
| `ValidationException` | 422 (includes field-level errors) |
| `ConflictException` | 409 |
| Unhandled | 500 (detail redacted outside Development) |

### API versioning

URL-segment versioning (`/api/v{version}/[controller]`). New API versions get their own OpenAPI document via `ApiVersionDocumentTransformer`. Base controller in `Src/Api/Controllers/Controller.cs` carries the route template; all controllers inherit from it.

### Logging

Both Api and Agent use **Serilog** configured from `appsettings.json`. Enriched with machine name, environment name, and HTTP request context. Do not use `ILogger` from Microsoft.Extensions.Logging directly — wire through Serilog enrichment.

### Service registration pattern

Extension methods on `IHostApplicationBuilder` keep `Program.cs` clean:
- `AddApiServices()` — controllers, versioning, OpenAPI, ProblemDetails, global exception handler
- `AddSerilogLogging()` — Serilog from config
- `AddAgentServices()` — registers `Worker` as `IHostedService`

## Tests

Three test projects with distinct responsibilities:

| Project | Purpose | Key infrastructure |
|---|---|---|
| `Tests/Unit/` | Isolated logic with Moq mocks | xUnit + Moq + FluentAssertions |
| `Tests/Integration/` | HTTP-level API tests | `WebApplicationFactory`, scaffolded, no tests yet |
| `Tests/Application/` | Full in-memory app, real middleware | `ApplicationFixture` (collection fixture), `ApplicationTestBase` base class |

`ApplicationFixture` boots the API once per xUnit collection. Tests that need `HttpClient` or `IServiceProvider` inherit from `ApplicationTestBase`.

## Code conventions

Enforced at build time via `.editorconfig` + `StyleCop.Analyzers` + `EnforceCodeStyleInBuild=true`:

- File-scoped namespaces
- Allman braces
- 4-space indentation, 120-char line limit
- No `this.` qualifier
- Private fields: `_camelCase`; constants: `SCREAMING_SNAKE_CASE`; interfaces: `IPascalCase`; type params: `TPascalCase`
- Suppress analyzer warnings via `.editorconfig` (`dotnet_diagnostic.<ID>.severity = none`), never with `#pragma warning disable`

## Commit conventions

Conventional Commits are enforced by a commitlint Git hook installed via `setup.sh`. Release-please uses commit history to auto-generate CHANGELOG and version bumps on merge to `main`.

## Roadmap (Docs/TODO.md highlights)

Key items not yet implemented that new features will likely need:
- EF Core DbContext + migrations
- MediatR (CQRS) + FluentValidation pipeline
- Repository pattern (`IRepository<T>`, `IUnitOfWork`)
- Health check endpoints (`/health`, `/health/ready`)
- CORS configuration
- OpenTelemetry traces + metrics
- Scalar/Swagger UI (OpenAPI docs already generated)
