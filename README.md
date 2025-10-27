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
- Install EntityFramework tool
```bash
dotnet tool install --global dotnet-ef
```

## Setup
 
1. Clone the repository:
```bash
git clone https://github.com/santipaje/AuthService.git
cd AuthService
```

2. Apply migrations
```bash
dotnet ef database update --project src/AuthService.Infrastructure
```

3. Run Api:
```bash
dotnet run --project src/AuthService.Api
```

 ---

## 📚 Documentation
[System Design](/docs/system-design.md)
[Requirements](/docs/requirements.md)
[Architecture Decision Records](/docs/adr/)
