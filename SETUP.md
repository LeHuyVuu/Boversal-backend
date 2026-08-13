# Boversal Backend - Setup Guide

## Architecture

```
┌─────────────┐
│   Gateway   │  :80
│  (YARP)     │
└──────┬──────┘
       │
       ├──────────────────┬──────────────────┐
       │                  │                  │
┌──────▼──────┐  ┌────────▼────────┐  ┌─────▼──────┐
│ Project Mgmt│  │  Utility Service│  │   Aspire   │
│  Service    │  │  (Email/File)   │  │  Dashboard │
│   :8080     │  │     :8080       │  │   :18888   │
└──────┬──────┘  └────────┬────────┘  └────────────┘
       │                  │
       └─────────┬────────┘
                 │
         ┌───────▼───────┐
         │  MySQL Aiven  │
         │     Cloud     │
         └───────────────┘
```

## Quick Start

### 1. Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (for local development)
- MySQL Aiven Cloud account

### 2. Configuration

Copy `.env.example` to `.env` and update values:

```bash
cp .env.example .env
```

Required environment variables:
- `DATABASE_URL`: MySQL connection string
- `JWT_KEY`: Secret key for JWT (min 32 characters)
- `KAFKA_BOOTSTRAP_SERVERS`: Kafka server address
- `EMAIL_SMTP_*`: SMTP configuration for email sending
- `AWS_*`: AWS S3 configuration for file storage

### 3. Start Services

```bash
docker compose down
docker compose build
docker compose up -d
```

### 4. Access Services

| Service | URL | Description |
|---------|-----|-------------|
| Gateway | http://localhost | API Gateway |
| Project Management API | http://localhost/project-management-service/swagger | Swagger UI |
| Utility Service API | http://localhost/utility-service/swagger | Swagger UI |
| Aspire Dashboard | http://localhost:18888 | Monitoring |

## API Usage

### Via Swagger UI

1. Open: http://localhost/project-management-service/swagger
2. Test endpoints directly in Swagger UI
3. All requests automatically routed through Gateway

### Via curl

```bash
# Register user
curl -X POST http://localhost/project-management-service/api/Auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","email":"test@example.com","password":"Test@123","fullName":"Test User"}'

# Login
curl -X POST http://localhost/project-management-service/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test@123"}'

# Get Projects
curl -X GET "http://localhost/project-management-service/api/Project?pageNumber=1&pageSize=10" \
  -H "accept: application/json"
```

## Troubleshooting

```bash
# View logs
docker compose logs -f

# View specific service logs
docker compose logs -f projectmanagement

# Check service status
docker compose ps

# Restart specific service
docker compose restart gateway

# Rebuild and restart
docker compose down && docker compose build && docker compose up -d
```

## Database

Database schema is automatically created on first startup using EF Core's `EnsureCreated()`.

Tables created:
- user
- project
- project_member
- project_status
- task
- task_assignee
- comment_thread
- comment_message
- meeting
- reminders
- activity_log
- file_asset
- attachment

## Development

### Running locally (without Docker)

1. Start each service:
```bash
cd ProjectManagementService.API
dotnet run

cd UtilityService
dotnet run

cd Boversal.Gateway
dotnet run
```

2. Or use Aspire AppHost:
```bash
cd Boversal.AppHost
dotnet run
```

## Routing

Gateway handles all routing:

| Client Request | Backend Receives |
|---------------|------------------|
| `/project-management-service/swagger/*` | `/swagger/*` |
| `/project-management-service/api/*` | `/api/*` |
| `/utility-service/swagger/*` | `/swagger/*` |
| `/utility-service/api/*` | `/api/*` |

## Security

- JWT authentication for Project Management API
- CORS configured for development (AllowAll)
- Cookie-based JWT token support
- X-Forwarded headers support for proxies
