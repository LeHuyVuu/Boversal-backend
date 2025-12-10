FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj và restore dependencies
COPY ["Boversal.Gateway/Boversal.Gateway.csproj", "Boversal.Gateway/"]
COPY ["Boversal.ServiceDefaults/Boversal.ServiceDefaults.csproj", "Boversal.ServiceDefaults/"]
RUN dotnet restore "Boversal.Gateway/Boversal.Gateway.csproj"

# Copy toàn bộ source code
COPY . .

# Build
WORKDIR "/src/Boversal.Gateway"
RUN dotnet build "Boversal.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Boversal.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Boversal.Gateway.dll"]
