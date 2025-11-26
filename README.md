# 🚀 Boversal Backend - Clean Architecture

Backend system for Boversal project management platform built with .NET 8.0, following Clean Architecture principles.

## 📋 Project Structure

```
boversal-backend/
├── ProjectManagementService/          # Main service
│   ├── ProjectManagementService.API/           # API Layer
│   ├── ProjectManagementService.Application/   # Business Logic (CQRS)
│   ├── ProjectManagementService.Domain/        # Domain Entities
│   └── ProjectManagementService.Infrastructure/# Data Access & External Services
├── UtilityService/                    # Utility microservice (Email, File Upload)
└── docker-compose.yml                 # Docker orchestration
```

## 🏗️ Architecture

- **Clean Architecture** with CQRS pattern
- **MediatR** for command/query handling
- **Entity Framework Core** with MySQL
- **JWT Authentication** with cookie-based auth
- **AutoMapper** for object mapping
- **FluentValidation** for request validation

## ⚙️ Tech Stack

- **.NET 8.0**
- **MySQL** (Aiven Cloud)
- **Docker** & Docker Compose
- **EF Core** for ORM
- **MediatR** for CQRS
- **AutoMapper**
- **JWT** Authentication

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK
- Docker & Docker Compose
- MySQL (or use provided Aiven connection)

### 1. Clone Repository

```bash
git clone https://github.com/LeHuyVuu/Boversal-backend.git
cd Boversal-backend
```

### 2. Environment Variables

Create `.env` file in root directory:

```env
# Database
DB_SERVER=your-mysql-server
DB_PORT=3306
DB_NAME=bnote
DB_USER=your-username
DB_PASSWORD=your-password

# JWT
JWT_KEY=your-super-secret-key-min-32-characters-long
JWT_ISSUER=ProjectManagementAPI
JWT_AUDIENCE=ProjectManagementClient
JWT_EXPIRATION_HOURS=24

# API Ports
PROJECT_MANAGEMENT_PORT=5268
UTILITY_SERVICE_PORT=5269
```

### 3. Update appsettings.json

**ProjectManagementService.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_SERVER};Port=${DB_PORT};Database=${DB_NAME};User=${DB_USER};Password=${DB_PASSWORD};SslMode=Required"
  },
  "Jwt": {
    "Key": "${JWT_KEY}",
    "Issuer": "${JWT_ISSUER}",
    "Audience": "${JWT_AUDIENCE}",
    "ExpirationHours": "${JWT_EXPIRATION_HOURS}"
  }
}
```

### 4. Run with Docker Compose

```bash
docker-compose up --build
```

### 5. Run Locally (Development)

```bash
# Restore packages
dotnet restore

# Run ProjectManagementService
cd ProjectManagementService.API
dotnet run

# Run UtilityService (in another terminal)
cd UtilityService
dotnet run
```

## 📡 API Endpoints

### ProjectManagementService (Port 5268)

#### Authentication
- `POST /api/Auth/register` - Register new user
- `POST /api/Auth/login` - Login
- `GET /api/Auth/me` - Get current user

#### Projects
- `GET /api/Project` - Get all projects (paginated, filtered by current user)
- `GET /api/Project/{id}` - Get project by ID
- `POST /api/Project` - Create new project
- `PUT /api/Project/{id}` - Update project
- `DELETE /api/Project/{id}` - Delete project

#### Tasks
- `GET /api/Task/my-tasks` - Get current user's tasks
- `GET /api/Task/my-tasks/project/{projectId}` - Get user's tasks by project
- `GET /api/Task/project/{projectId}` - Get all tasks in project
- `GET /api/Task/{id}` - Get task by ID
- `POST /api/Task` - Create new task
- `PUT /api/Task/{id}` - Update task
- `PATCH /api/Task/{id}` - Partial update task
- `DELETE /api/Task/{id}` - Delete task

#### Dashboard
- `GET /api/Dashboard` - Get dashboard data (stats, recent tasks, active projects)

#### Users
- `GET /api/User/{id}` - Get user by ID
- `GET /api/User/search?query={query}` - Search users
- `PUT /api/User/profile` - Update user profile
- `POST /api/User/change-password` - Change password

### UtilityService (Port 5269)
- Email service
- File upload service

## 📚 Documentation

See detailed API documentation:
- [Create Project API Guide](./CREATE_PROJECT_API_GUIDE.md)
- [Update Task & User Profile Guide](./UPDATE_TASK_AND_USER_PROFILE_GUIDE.md)
- [Dashboard API Guide](./DASHBOARD_API_GUIDE.md)

## 🐳 Docker Deployment

### Build Images

```bash
# Build ProjectManagementService
docker build -f ProjectManagementService.API/Dockerfile -t boversal-project-service:latest .

# Build UtilityService
docker build -f UtilityService/Dockerfile -t boversal-utility-service:latest .
```

### Push to Docker Hub

```bash
docker tag boversal-project-service:latest yourusername/boversal-project-service:latest
docker push yourusername/boversal-project-service:latest

docker tag boversal-utility-service:latest yourusername/boversal-utility-service:latest
docker push yourusername/boversal-utility-service:latest
```

## 🚀 Production Deployment

### Deploy to Azure/AWS/GCP

1. Set environment variables in cloud platform
2. Use provided Dockerfiles
3. Configure load balancer and SSL
4. Set up database connection (already using Aiven MySQL)

### Deploy to Railway/Render/Fly.io

```bash
# Example for Railway
railway up

# Example for Render
# Connect GitHub repo and set environment variables in dashboard
```

## 🔒 Security Notes

- ⚠️ **NEVER commit secrets to GitHub**
- Use environment variables for sensitive data
- JWT secret key must be at least 32 characters
- Enable HTTPS in production
- Use strong database passwords
- Rotate secrets regularly

## 🧪 Testing

```bash
# Run tests (when added)
dotnet test
```

## 📝 Database Migrations

```bash
cd ProjectManagementService.Infrastructure

# Add migration
dotnet ef migrations add MigrationName --startup-project ../ProjectManagementService.API

# Update database
dotnet ef database update --startup-project ../ProjectManagementService.API
```

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📄 License

This project is private and proprietary.

## 👥 Team

- **LeHuyVuu** - Owner & Developer

## 📞 Contact

Project Link: [https://github.com/LeHuyVuu/Boversal-backend](https://github.com/LeHuyVuu/Boversal-backend)
