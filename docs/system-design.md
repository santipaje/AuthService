# AuthService – System Design

## Overview
AuthService is a reusable authentication and authorization service built with **.NET 8**, **ASP.NET Identity**, and **SQL Server**, following the *Microsoft* stack.  
It is designed to provide a clean separation of concerns, strong testability, and flexibility for future extensions.

---

## Architecture

We follow a **Clean Architecture** approach, organizing the solution into four main layers:

- **AuthService.Api (Presentation Layer)**  
  - Exposes the REST API endpoints.  
  - Configures middleware: authentication, authorization, logging, validation, Swagger.  
  - Depends only on the Application layer (not on Infrastructure directly).

- **AuthService.Application (Application Layer)**  
  - Contains **use cases** and business logic: services, DTOs, validators.  
  - Defines interfaces for persistence and token generation (abstractions).  
  - Does not depend on Infrastructure — making it easy to test with mocks.  

- **AuthService.Domain (Domain Layer)**  
  - Core business entities  
  - Enums.  
  - Contains **pure domain logic** with no dependencies.  

- **AuthService.Infrastructure (Infrastructure Layer)**  
  - Implementation details: EF Core DbContext, ASP.NET Identity configuration, migrations.  
  - Responsible for persistence, database access, and integration with Identity.  
  - Depends on Application abstractions to implement repositories/services.  

---

## Why Clean Architecture?

Check the adr 001 => ``docs/adr/001-use-clean-architecture.md``
