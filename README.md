# ShopBridge

**ShopBridge** is an e-commerce order management system built with .NET 8, following Clean Architecture principles.

The project focuses on maintainable architecture, separation of concerns, testability, and reliable backend development practices.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- MediatR (CQRS)
- FluentValidation
- Docker & Docker Compose
- xUnit
- Moq

## Architecture

ShopBridge follows Clean Architecture principles, separating business logic, application logic, infrastructure, and presentation concerns.

The solution consists of the following projects:

| Project | Responsibility |
|---|---|
| `ShopBridge.Domain` | Core business entities and domain rules |
| `ShopBridge.Application` | Application use cases, CQRS handlers, and abstractions |
| `ShopBridge.Infra` | Database access and infrastructure implementations |
| `ShopBridge.Api` | REST API endpoints and application entry point |
| `ShopBridge.Tests` | Unit and integration tests |

## Features

- Product management API
- CQRS implementation using MediatR
- Generic Repository Pattern
- Unit of Work Pattern
- Input validation using FluentValidation
- PostgreSQL integration with Entity Framework Core
- Dependency Injection
- Dockerized API and PostgreSQL database
- Unit and integration testing

## Testing

Run all tests from the solution root:

```bash
dotnet test ShopBridge.sln
```

The test project includes tests for application handlers, validators, domain logic, and API integration.

## Running with Docker

ShopBridge uses Docker Compose to run the ASP.NET Core API and PostgreSQL database together.

### Prerequisites

- Docker Desktop
- Docker Compose

### Configuration

Create a `.env` file in the solution root, next to `docker-compose.yml`:

```env
SHOPBRIDGE_DB_PASSWORD=your_secure_password
```

Add `.env` to `.gitignore` to prevent committing database credentials:

```gitignore
.env
```

**Important:** Never commit database credentials or environment-specific secrets to Git.

### Start the Application

Run the following command from the solution root:

```bash
docker compose up -d --build
```

This command builds the API image and starts the API and PostgreSQL containers.

### Verify Containers

Check the running containers:

```bash
docker compose ps
```

Check application logs:

```bash
docker compose logs -f
```

### Access the API

Once the containers are running:

- **Swagger UI:** http://localhost:8080/swagger
- **API Base URL:** http://localhost:8080

### Stop the Application

Stop and remove the containers:

```bash
docker compose down
```

To remove containers and the PostgreSQL volume:

```bash
docker compose down -v
```

**Warning:** The `-v` option permanently deletes the PostgreSQL data stored in the Docker volume.

## Docker Services

| Service | Description | Host Port | Container Port |
|---|---|---:|---:|
| `shopbridge.api` | ASP.NET Core Web API | 8080 | 8080 |
| `shopbridge-db` | PostgreSQL 16 | 5433 | 5432 |

The API connects to PostgreSQL through the internal Docker network using:

- **Host:** `shopbridge-db`
- **Port:** `5432`
- **Database:** `shopbridge_db`

## Development

To build the solution:

```bash
dotnet build ShopBridge.sln
```

To run the API directly on your machine:

```bash
dotnet run --project ShopBridge.Api
```

To apply Entity Framework Core migrations:

```bash
dotnet ef database update --project ShopBridge.Infra --startup-project ShopBridge.Api
```

## Project Status

ShopBridge is an evolving backend portfolio project. New features and infrastructure components will be introduced incrementally as development progresses.
