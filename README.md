# AuthService 🔐

AuthService is a **reusable authentication and authorization service** built with **.NET 8**, **ASP.NET Core Identity**, and **SQL Server**.  
It is designed following **Clean Architecture** principles to ensure **maintainability, testability, and reusability** across multiple projects.

---

## 🚀 Features

- User registration and login with ASP.NET Core Identity
- JWT-based authentication
- Extensible `ApplicationUser` entity
- Structured in Clean Architecture layers:
  - **Domain**: Core entities (`ApplicationUser`, `RefreshToken`)
  - **Application**: Business logic and services
  - **Infrastructure**: Database (SQL Server), Identity, EF Core
  - **API**: REST endpoints
- Ready for **CI/CD**, **Docker**, and **Azure deployment**
- Includes unit and integration testing projects
- Frontend example with **React + Vite**

---

## 📂 Project Structure

AuthService/
├── src/
│ ├── AuthService.Api/ # API layer (controllers, config)
│ ├── AuthService.Application/ # Business logic
│ ├── AuthService.Domain/ # Domain entities
│ ├── AuthService.Infrastructure/# EF Core, Identity, persistence
├── frontend/ # React frontend
├── tests/ # Unit and integration tests
├── docs/ # Documentation (ADR, system design, requirements)
├── .github/workflows/ # CI/CD pipelines

---

## 🛠️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Setup
 
 It is not created yet, this is a *TEMPLATE*.

 ---

## 📚 Documentation
[System Design](/docs/system-design.md)
[Requirements](/docs/requirements.md)
[Architecture Decision Records](/docs/adr/)
