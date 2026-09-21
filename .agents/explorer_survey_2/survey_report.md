# Deployment, Containerization & Environment Readiness Survey Report

**Date:** 2026-09-20  
**Author:** explorer_survey_2  
**Target Solution:** `Zentric.slnx` (.NET 10.0 / PostgreSQL 16)  
**Corpus / Working Directory:** `c:\Users\da-V7\Desktop\zentric-backend`

---

## 1. Executive Summary

This survey assesses the production-readiness, containerization requirements, deployment posture, and failure modes for `zentric-backend`. The project compiles cleanly under .NET 10 (`dotnet build` succeeds with 0 warnings, 0 errors; 223/223 unit and application tests pass). However, no Dockerfile, Docker Compose file, or deployment automation scripts exist in the repository.

Executing `docker-compose up -d` in a production-like setting requires addressing four critical gaps:
1. **Zero container definitions:** Complete absence of multi-stage `Dockerfile`, `docker-compose.yml`, `.dockerignore`, or entrypoint scripts.
2. **Migration gap:** EF Core migrations exist in `Zentric.Infrastructure/Migrations`, but `Program.cs` does not apply them automatically at startup, leaving a fresh containerized PostgreSQL database without tables.
3. **HTTP/HTTPS pipeline trap:** `app.UseHttpsRedirection()` in `Program.cs` will force HTTPS redirects inside HTTP container environments unless configured or conditionalized.
4. **Environment & OpenApi exposure:** `MapOpenApi()` is gated by `app.Environment.IsDevelopment()`. Running under `Production` will make API documentation unreachable unless explicitly enabled or configured.

---

## 2. Review of Existing Project Configuration

### 2.1 Configuration Files Analysis

| File | Key Configurations Observed | Findings & Deployment Impact |
|---|---|---|
| `Zentric.slnx` | Solution references 5 projects: `Zentric.Api`, `Zentric.Application`, `Zentric.Domain`, `Zentric.Infrastructure`, `Zentric.Tests`. | Modern XML-based .slnx format supported in .NET 10. |
| `Zentric.Api/appsettings.json` | `ConnectionStrings:DefaultConnection = "Host=localhost;Database=ZentricDb;Username=postgres;Password=postgres"` | Hardcoded local connection string. In container environments, must be overridden via environment variable `ConnectionStrings__DefaultConnection`. |
| `Zentric.Api/appsettings.Development.json` | Log levels only (`Default: Information`, `Microsoft.AspNetCore: Warning`). | Minimal dev overrides. |
| `Zentric.Api/Properties/launchSettings.json` | Profiles: `http` (port 5076), `https` (ports 7170, 5076). `ASPNETCORE_ENVIRONMENT=Development`. | Used only for local CLI/IDE launch; ignored inside Docker containers. |
| `Zentric.Api/Program.cs` | Line 20-22: `options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Zentric.Infrastructure"))` | EF Core Npgsql registered against `DefaultConnection`. No startup migration logic (`Database.Migrate()`) is invoked. |
| `Zentric.Api/Program.cs` | Line 37: `builder.Services.AddHostedService<CheckoutTimeoutService>();` | Background service starts immediately on host boot; creates scopes and queries repository. If tables do not exist, logs errors. |
| `Zentric.Api/Program.cs` | Line 52-55: `if (app.Environment.IsDevelopment()) { app.MapOpenApi(); }` | OpenApi JSON (`/openapi/v1.json`) is disabled in non-development environments. No Swagger UI or Scalar UI is registered. |
| `Zentric.Api/Program.cs` | Line 60: `app.UseHttpsRedirection();` | Redirects plain HTTP calls to HTTPS. In Docker behind reverse proxy or running purely on HTTP port 8080, this triggers redirect failures (ERR_CONNECTION_REFUSED) if TLS termination occurs upstream or if HTTPS port is unbound. |

### 2.2 EF Core Migrations State

The repository contains three historical migrations in `Zentric.Infrastructure/Migrations`:
1. `20260918192602_InitialCreate.cs`
2. `20260918193539_CompleteSchema.cs`
3. `20260920003544_AddBackgroundServicesAndUpdates.cs`

**Finding:** Migrations are up-to-date with the model snapshot (`ZentricDbContextModelSnapshot.cs`). However, there is no automatic migration mechanism in code or containers.

---

## 3. Containerization Requirements & Architectural Design

### 3.1 Target Runtimes & Base Images

- **Target SDK Image:** `mcr.microsoft.com/dotnet/sdk:10.0`
- **Target ASP.NET Runtime Image:** `mcr.microsoft.com/dotnet/aspnet:10.0`
- **Target Database Image:** `postgres:16-alpine` (lightweight, stable, matches Npgsql 10.0.3 requirements)
- **Container Architecture:** Linux amd64/arm64 multi-arch compatible.

### 3.2 Multi-Stage Dockerfile Architecture

To ensure fast builds, small attack surface, and security compliance:
1. **Layer Caching Optimization:**
   - Copy only the `.csproj` files for `Zentric.Api`, `Zentric.Application`, `Zentric.Domain`, and `Zentric.Infrastructure`.
   - Run `dotnet restore Zentric.Api/Zentric.Api.csproj`.
   - Only after restore is cached, copy the remaining source directories. This prevents cache bust on source code edits when dependencies have not changed.
2. **Non-Root Security:**
   - In .NET 8+, the runtime image includes a built-in user `app` (`UID 1654`).
   - Use `USER $APP_UID`.
   - Listen on port `8080` (default non-root port in modern ASP.NET Core).
3. **Artifact Size Optimization:**
   - Exclude `Zentric.Tests` from the production runtime image.
   - Use `dotnet publish -c Release -o /app/publish /p:UseAppHost=false --no-restore`.

### 3.3 Proposed Multi-Stage Dockerfile Blueprint

```dockerfile
# Stage 1: Base Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Stage 2: Build SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files for dependency caching
COPY ["Zentric.Domain/Zentric.Domain.csproj", "Zentric.Domain/"]
COPY ["Zentric.Application/Zentric.Application.csproj", "Zentric.Application/"]
COPY ["Zentric.Infrastructure/Zentric.Infrastructure.csproj", "Zentric.Infrastructure/"]
COPY ["Zentric.Api/Zentric.Api.csproj", "Zentric.Api/"]

RUN dotnet restore "Zentric.Api/Zentric.Api.csproj"

# Copy source code and build
COPY Zentric.Domain/ Zentric.Domain/
COPY Zentric.Application/ Zentric.Application/
COPY Zentric.Infrastructure/ Zentric.Infrastructure/
COPY Zentric.Api/ Zentric.Api/

WORKDIR /src/Zentric.Api
RUN dotnet build "Zentric.Api.csproj" -c Release -o /app/build --no-restore

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "Zentric.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# Stage 4: Final Production Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zentric.Api.dll"]
```

### 3.4 Proposed `.dockerignore` Blueprint

```ignore
**/.git
**/.github
**/.vs
**/.vscode
**/.agents
**/bin
**/obj
**/TestResults
**/*.user
**/*.suo
README.md
LICENSE
```

### 3.5 Proposed `docker-compose.yml` Blueprint

```yaml
services:
  zentric-db:
    image: postgres:16-alpine
    container_name: zentric-postgres
    restart: unless-stopped
    environment:
      POSTGRES_DB: ${POSTGRES_DB:-zentricdb}
      POSTGRES_USER: ${POSTGRES_USER:-zentric}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-zentric_secret}
    ports:
      - "${POSTGRES_PORT:-5432}:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-zentric} -d ${POSTGRES_DB:-zentricdb}"]
      interval: 5s
      timeout: 5s
      retries: 5
      start_period: 10s
    networks:
      - zentric-network

  zentric-api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: zentric-api
    restart: unless-stopped
    depends_on:
      zentric-db:
        condition: service_healthy
    environment:
      ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Development}
      ASPNETCORE_HTTP_PORTS: "8080"
      ConnectionStrings__DefaultConnection: "Host=zentric-db;Port=5432;Database=${POSTGRES_DB:-zentricdb};Username=${POSTGRES_USER:-zentric};Password=${POSTGRES_PASSWORD:-zentric_secret}"
    ports:
      - "${API_PORT:-5076}:8080"
    networks:
      - zentric-network

volumes:
  postgres_data:
    driver: local

networks:
  zentric-network:
    driver: bridge
```

---

## 4. Failure Modes & Concrete Solutions Matrix

| # | Failure Mode | Root Cause | Impact | Concrete Solution |
|---|---|---|---|---|
| **F1** | **Database Connectivity Race Condition** | API boots up before PostgreSQL is accepting connections. | API crashes on start or throws 500 on first query. | 1. Implement Docker Compose healthcheck on `zentric-db` (`pg_isready`).<br>2. Set `depends_on: zentric-db: condition: service_healthy`.<br>3. In code, configure Npgsql connection resilience (`options.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null)`). |
| **F2** | **Unapplied Migrations / Missing Tables** | Containerized PostgreSQL starts with clean data volume; no code currently runs `Database.Migrate()`. | Any query fails with `42P01: relation ... does not exist`. Hosted service logs repeated SQL errors. | **Solution:** Add an automated startup migration handler or environment flag in `Program.cs`:<br>```csharp<br>if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", true))<br>{<br>    using var scope = app.Services.CreateScope();<br>    var db = scope.ServiceProvider.GetRequiredService<ZentricDbContext>();<br>    await db.Database.MigrateAsync();<br>}<br>``` |
| **F3** | **HTTPS Redirection Loop / Port Failures** | Line 60 of `Program.cs`: `app.UseHttpsRedirection()` is executed unconditionally. | In HTTP container environments (e.g. port 8080), browser receives 307/308 redirect to unreachable HTTPS port. | Wrap redirection in check: `if (!app.Environment.IsProduction() || builder.Configuration.GetValue<bool>("EnableHttpsRedirection", false))` or only if HTTPS port is configured. |
| **F4** | **Host Port Conflict (5432 or 5076)** | Local developer machine may already run a native PostgreSQL service on port 5432 or another service on 5076. | `docker-compose up -d` fails with `port is already allocated`. | Externalize ports in `docker-compose.yml` via variables with safe defaults: `${POSTGRES_PORT:-5432}:5432` and `${API_PORT:-5076}:8080`, plus an example `.env.example` file. |
| **F5** | **Hardcoded Connection String Ignored** | ASP.NET Core loads `appsettings.json` first. | Developers expect docker to connect to DB container. | Utilize the standard ASP.NET Core environment variable format `ConnectionStrings__DefaultConnection` in `docker-compose.yml`, which overrides `ConnectionStrings:DefaultConnection`. |
| **F6** | **OpenApi / Swagger Inaccessible in Production** | `if (app.Environment.IsDevelopment()) { app.MapOpenApi(); }` in `Program.cs`. | In production containers (`ASPNETCORE_ENVIRONMENT=Production`), `/openapi/v1.json` returns 404. | Allow documentation through configuration flag: `if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableOpenApi", false)) { app.MapOpenApi(); }`. Also recommend registering Swagger UI or Scalar UI package for interactive exploration. |
| **F7** | **Host Docker Daemon Inactivity** | Survey test revealed Docker Desktop CLI is present (v29.7.2) but engine daemon was not started (`pipe/dockerDesktopLinuxEngine` not found). | `docker-compose up` will immediately exit with daemon connection error. | Provide clear operational run instructions and pre-flight validation command in `README.md` / deployment scripts. |

---

## 5. Deployment Readiness Recommendations

1. **Implement Docker Assets:**
   - Create root `Dockerfile` using the 4-stage build pattern.
   - Create root `docker-compose.yml` with health checks, persistent volume, and network isolation.
   - Create `.dockerignore` to keep image builds lean.
   - Create `.env.example` documenting all customizable ports and credentials.
2. **Add Connection Resilience & Startup Migration in `Program.cs`:**
   - Add `options.UseNpgsql(..., o => o.EnableRetryOnFailure(5))` in `Program.cs`.
   - Add safe database migration runner on startup guarded by a configuration switch (`ApplyMigrationsOnStartup`).
3. **Guard `app.UseHttpsRedirection()`:**
   - Make HTTPS redirection optional in containerized environments.
4. **Health Check Endpoint:**
   - Register `builder.Services.AddHealthChecks().AddDbContextCheck<ZentricDbContext>();` and map `app.MapHealthChecks("/health");` so orchestration tools (Docker Compose, Kubernetes, AWS ECS) can verify container health.
