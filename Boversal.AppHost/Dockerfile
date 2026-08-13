FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["Boversal.AppHost/Boversal.AppHost.csproj", "Boversal.AppHost/"]
COPY ["Boversal.ServiceDefaults/Boversal.ServiceDefaults.csproj", "Boversal.ServiceDefaults/"]
COPY ["ProjectManagementService.API/ProjectManagementService.API.csproj", "ProjectManagementService.API/"]
COPY ["ProjectManagementService.Application/ProjectManagementService.Application.csproj", "ProjectManagementService.Application/"]
COPY ["ProjectManagementService.Domain/ProjectManagementService.Domain.csproj", "ProjectManagementService.Domain/"]
COPY ["ProjectManagementService.Infrastructure/ProjectManagementService.Infrastructure.csproj", "ProjectManagementService.Infrastructure/"]
COPY ["UtilityService/UtilityService.csproj", "UtilityService/"]
COPY ["Boversal.Gateway/Boversal.Gateway.csproj", "Boversal.Gateway/"]

# Restore dependencies
RUN dotnet restore "Boversal.AppHost/Boversal.AppHost.csproj"

# Copy all source code
COPY . .

# Build AppHost
WORKDIR "/src/Boversal.AppHost"
RUN dotnet build "Boversal.AppHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Boversal.AppHost.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPIRE_ALLOW_UNSECURED_TRANSPORT=true

ENTRYPOINT ["dotnet", "Boversal.AppHost.dll"]
