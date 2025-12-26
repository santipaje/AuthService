# AuthService 🔐

AuthService is a **reusable authentication and authorization service** built with **.NET 8**, **ASP.NET Core Identity**, and **SQL Server**.  
It is designed following **Clean Architecture** principles to ensure **maintainability, testability, and reusability** across multiple projects.

---

## 🚀 Features

- User registration and login with ASP.NET Core Identity
- JWT-based authentication
- Structured in Clean Architecture layers:
  - **Domain**: Core entities 
  - **Application**: Business logic and services
  - **Infrastructure**: Database, Identity, EF Core
  - **API**: REST endpoints
- Ready for **CI/CD**, **Docker**, and **Azure deployment**
- Includes unit and integration testing projects

---

## 📂 Project Structure

AuthService/</br>
├── src/ (source code) </br>
├── frontend/ (ux/ui, user interactions, etc) </br>
├── tests/ (unit and integration tests) </br>
├── docs/ (documentation) </br>
├── .github/workflows/ (pipelines) </br>

---

## 🛠️ Getting Started

### Prerequisites
- Install [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Install [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Install EntityFramework tool → `dotnet tool install --global dotnet-ef`
- Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Setup

**Clone the repository:**
```bash
git clone https://github.com/santipaje/AuthService.git
cd AuthService
```

### Setup Locally

1. **Apply migrations**
```bash
dotnet ef database update --project src/AuthService.Infrastructure
```

2. **Run Api:**
```bash
dotnet run --project src/AuthService.Api
```

### Setup with Docker

1. **Config environment**
Clone the example environment file and fill in the values:
```bash
cp .env.example .env
```

2. **Launch Containers**
Build and start the services in detached mode:
```bash
docker-compose up -d --build
```

3. **Verify Status**
Check if the containers are running and healthy:
```bash
docker ps
```

The AuthService API will be reachable at `http://localhost:5000` and the SQL Server at `localhost:1433`.

---

## Branching Strategy

This repository follows a Git Flow inspired strategy:

- feature/* → development work
- docs/* → documentation work
- bugfix/* → bug fixing
- develop → integration branch
- main → stable production-ready code

---

## CI/CD

This project uses **GitHub Actions** to ensure code quality.

The pipeline does the following steps:
- Builds the solution
- Runs unit tests
- Runs integration tests with SQL Server
- Builds the Docker image
- Pushes the image in this [DockerHub repository](https://hub.docker.com/repositories/santipaje).

All checks must pass before merging into `develop` and `main`, except for the docker image build and push step, which is only required when pushing to `main`.

 ---

## 📚 Documentation
[System Design](/docs/system-design.md)</br>
[Requirements](/docs/requirements.md)</br>
[Testing](/docs/testing.md)</br>
[Architecture Decision Records](/docs/adr/)</br>
