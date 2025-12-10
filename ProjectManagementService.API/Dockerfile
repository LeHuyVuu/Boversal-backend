FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files
COPY ["ProjectManagementService.API/ProjectManagementService.API.csproj", "ProjectManagementService.API/"]
COPY ["ProjectManagementService.Application/ProjectManagementService.Application.csproj", "ProjectManagementService.Application/"]
COPY ["ProjectManagementService.Domain/ProjectManagementService.Domain.csproj", "ProjectManagementService.Domain/"]
COPY ["ProjectManagementService.Infrastructure/ProjectManagementService.Infrastructure.csproj", "ProjectManagementService.Infrastructure/"]
COPY ["Boversal.ServiceDefaults/Boversal.ServiceDefaults.csproj", "Boversal.ServiceDefaults/"]

RUN dotnet restore "ProjectManagementService.API/ProjectManagementService.API.csproj"

# Copy source code
COPY . .

WORKDIR "/src/ProjectManagementService.API"
RUN dotnet build "ProjectManagementService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectManagementService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectManagementService.API.dll"]
