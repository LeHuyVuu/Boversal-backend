FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files
COPY ["UtilityService/UtilityService.csproj", "UtilityService/"]
COPY ["Boversal.ServiceDefaults/Boversal.ServiceDefaults.csproj", "Boversal.ServiceDefaults/"]

RUN dotnet restore "UtilityService/UtilityService.csproj"

# Copy source code
COPY . .

WORKDIR "/src/UtilityService"
RUN dotnet build "UtilityService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UtilityService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UtilityService.dll"]
