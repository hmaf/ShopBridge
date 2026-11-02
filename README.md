# ShopBridge

E-commerce order management system built with .NET 8 and Clean Architecture.

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

The solution follows Clean Architecture principles:

- **ShopBridge.Domain** – Business entities and domain rules.
- **ShopBridge.Application** – Application logic, CQRS handlers, and abstractions.
- **ShopBridge.Infra** – Persistence and infrastructure implementations.
- **ShopBridge.Api** – REST API endpoints and dependency injection.
- **ShopBridge.Tests** – Unit and integration tests.

## Features

- Product management API
- CQRS implementation using MediatR
- Generic Repository Pattern
- Unit of Work Pattern
- Input validation using FluentValidation
- PostgreSQL integration with EF Core
- Dockerized API and database
- Unit and integration testing

## 🐳 Running with Docker

ShopBridge uses Docker Compose to run the ASP.NET Core API and PostgreSQL database together.

### Prerequisites

- Docker Desktop
- Docker Compose

### Configuration

Create a `.env` file in the solution root (next to `docker-compose.yml`):

```env
SHOPBRIDGE_DB_PASSWORD=your_secure_password
```

**Important:** Never commit your `.env` file or database credentials to Git.

Add `.env` to `.gitignore`:

```gitignore
.env
```

### Start the application

Run the following command from the solution root:

```bash
docker compose up -d --build
```

This command builds the API image and starts the API and PostgreSQL containers.

### Verify containers

```bash
docker compose ps
```

Check the container logs:

```bash
docker compose logs -f
```

### Access the API

Once the containers are running:

- **Swagger UI:** http://localhost:8080/swagger
- **API:** http://localhost:8080

### Stop the application

```bash
docker compose down
```

To remove containers and the PostgreSQL volume (this permanently deletes the database data):

```bash
docker compose down -v
```

### Services

| Service | Description | Port |
|---|---|---|
| `shopbridge.api` | ASP.NET Core Web API | 8080 |
| `shopbridge-db` | PostgreSQL 16 | 5433 (host) |

PostgreSQL is accessible from the API container through the internal Docker network using the hostname `shopbridge-db` and port `5432`.

### Architecture

ShopBridge follows Clean Architecture principles and separates responsibilities into:

- **ShopBridge.Domain** – Core business entities and rules.
- **ShopBridge.Application** – Use cases, CQRS handlers, and application abstractions.
- **ShopBridge.Infra** – Database access and infrastructure implementations.
- **ShopBridge.Api** – HTTP endpoints and application entry point.
- **ShopBridge.Tests** – Unit and integration tests.
