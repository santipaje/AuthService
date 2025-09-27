# ADR 001: Use Clean Architecture

---

## Context  
When designing the AuthService project, I needed to decide on the architectural style. The key requirements were:  
- Reusability across future projects, because it is supposed to be used in other projetcs.  
- Testability.
- Clear separation of concerns between layers (domain, application, infrastructure, API).  
- Support for modern DevOps practices such as CI/CD, Docker, cloud deployment in Azure.  
- Long-term maintainability, since authentication/authorization will likely evolve (refresh tokens, OpenID Connect, integration with third-party IdPs).

> **Note:** I did some research to learn about the advantages and disadvantages of different architectures, and I used AI as an advisor to assess more efficiently and learn as I went along.


We considered several options:

1. **Layered Architecture** 
   - Pros: Simple, well-known, quick to implement.  
   - Cons: Coupling between layers tends to grow.

2. **Hexagonal Architecture (Ports and Adapters)**  
   - Pros: Very flexible, isolates domain logic with ports/adapters.  
   - Cons: Similar to Clean Architecture but less standardized in .NET ecosystem.  

3. **Microservices from the start**  
   - Pros: High scalability, fits well with cloud-native deployments.  
   - Cons: Overengineering at this stage. Not justified for a single auth service project.  

4. **Clean Architecture** (Chosen)  
   - Pros:  
     - Aligns well with .NET community best practices.  
     - Explicit separation of **Domain, Application, Infrastructure, API** projects.  
     - Business logic is independent of frameworks (e.g., EF Core, Identity, Azure).  
     - High testability: application and domain layers can be tested in isolation.  
     - Supports incremental complexity: can start small and evolve into an Identity Provider.  
   - Cons:  
     - More initial effort (multiple projects, abstractions).

---

## Reasons why Clean Architecture is the best option here  

1. **Future-proofing**  
   Authentication and authorization evolve quickly (e.g., refresh tokens, OpenID Connect, integration with Azure AD). Clean Architecture allows adding these without rewriting the core business logic.  

2. **Testability**  
   Business rules and token logic can be tested without relying on EF Core, Identity, or the API layer. This guarantees fast and reliable test coverage.  

3. **Clear boundaries for maintainability**  
   - `Domain` → core entities. 
   - `Application` → use cases and services.
   - `Infrastructure` → persistence, Identity integration, external services.  
   - `Api` → only delivery concerns (controllers, request/response).  
   This separation reduces the risk of business logic leaking into controllers or repositories.  

4. **Alignment with industry and .NET best practices**  
   The .NET community widely embraces Clean Architecture for professional solutions, making the project recognizable and easier for contributors to understand.  

5. **Balance between simplicity and scalability**  
   Unlike microservices, this approach avoids unnecessary complexity, but still allows splitting components into services later if the system grows.  

---

## Decision  
We will adopt **Clean Architecture** as the foundation of the AuthService project. It best balances testability, maintainability, and professional demonstration of software engineering practices in .NET.  

---

## Consequences  
- Project will be organized in four main layers: `Domain`, `Application`, `Infrastructure`, and `Api`.  
- Clear boundaries between layers enforced through interfaces and dependency inversion.
- Future additions can be introduced without disrupting core domain logic.  
