# --- BUILD STAGE ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AuthService.sln .
COPY src/AuthService.Api/AuthService.Api.csproj src/AuthService.Api/
COPY src/AuthService.Application/AuthService.Application.csproj src/AuthService.Application/
COPY src/AuthService.Domain/AuthService.Domain.csproj src/AuthService.Domain/
COPY src/AuthService.Infrastructure/AuthService.Infrastructure.csproj src/AuthService.Infrastructure/
COPY tests/AuthService.IntegrationTests/AuthService.IntegrationTests.csproj tests/AuthService.IntegrationTests/
COPY tests/AuthService.UnitTests/AuthService.UnitTests.csproj tests/AuthService.UnitTests/

# Restore dependencies
RUN dotnet restore

# --- PUBLISH STAGE ---
COPY src/ src/
COPY tests/ tests/

# Publish API
RUN dotnet publish src/AuthService.Api/AuthService.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# --- RUN STAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]