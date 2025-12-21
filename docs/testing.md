# Testing Strategy

## Objective

This document describes the testing strategy for the AuthService.

The goal is to:
- Validate core authentication flows.
- Ensure the proper functioning of validation, token generation and user creation.
- Provide confidence for future refactoring

---

## Types of Tests

### Unit Tests

Unit tests validate individual components in isolation, without involving HTTP resquest, database or the ASP.NET pipeline.

#### Covered Components
- Login Validator
- Request Validator
- TokenService
- AuthService
- AuthController

#### Project
- AuthService.UnitTests



### Integration Tests

Integration tests validate the full request flow:
HTTP → Controller → Application → Infrastructure → Database

#### Covered Scenarios
- User registration
- User login
- Validation errors
- Authentication failures

#### Project
- AuthService.IntegrationTests

---

## Test Architecture

- In-memory SQLite database is used
- Database is migrated and seeded automatically
- Tests share the same database instance per test collection
- Real DTOs and routes are used

---

## ▶️ Running Tests

```bash
dotnet test
```