# Project Management Service Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081
# Install curl for health checks  
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (better caching)
COPY ["ProjectManagementService.API.csproj", "./"]
RUN dotnet restore "ProjectManagementService.API.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "ProjectManagementService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectManagementService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8081/health || exit 1

ENTRYPOINT ["dotnet", "ProjectManagementService.API.dll"]