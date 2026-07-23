# Spec: MinimumBackend Codebase Remediation

## Problem Statement

The MinimumBackend API is a .NET 10 Clean Architecture project that implements authentication, authorization, and RBAC. While the structural layering is conceptually correct, the codebase contains critical security vulnerabilities, architectural violations, logic bugs, and dead code that make it unsafe for any non-development deployment. Secrets are hardcoded in appsettings files, sensitive tokens are stored in plaintext, the Application layer directly references Infrastructure, async methods use synchronous blocking patterns that can deadlock, and the MediatR refresh-token flow returns stale tokens. Additionally, tests are sparse and do not cover the majority of handlers, middleware, or edge cases.

## Solution

Remediate the codebase through a phased effort: first eliminate critical security and correctness bugs (P0), then restore Clean Architecture boundaries (P1), then harden logging, error handling, and missing tests (P2), and finally clean up dead code and inconsistencies (P3). Each fix is scoped to existing seams (Application handlers, Infrastructure repositories, Api middleware) so that behavior can be verified with tests at the highest practical layer without introducing new cross-cutting seams.

## User Stories

1. As a developer, I want secrets removed from appsettings.json so that the repository can be committed safely without leaking credentials.
2. As a security engineer, I want the CSRF middleware to use ASP.NET Core's built-in antiforgery service instead of a hardcoded token, so that CSRF protection actually works.
3. As a security engineer, I want refresh tokens hashed before persistence so that a database leak does not immediately compromise user sessions.
4. As a security engineer, I want password reset and email confirmation tokens hashed so that token leakage does not allow unauthorized account access.
5. As a developer, I want the Application layer to depend on repository interfaces rather than concrete DbContext types, so that Clean Architecture boundaries are enforced.
6. As a developer, I want all async EF Core calls to be awaited rather than blocked with `.Result`, so that the API does not deadlock under load.
7. As a user, I want the refresh-token rotation flow to return the newly issued refresh token, so that my session continues seamlessly after rotation.
8. As a user, I want the ExceptionMiddleware to return the correct HTTP status code, so that clients can programmatically detect failures.
9. As a user, I want sensitive request and response bodies excluded from logs, so that my passwords, tokens, and PII are not written to disk.
10. As a developer, I want CORS restricted to actual frontend origins rather than allowing all origins, so that random sites cannot make authenticated requests.
11. As a developer, I want database mutations batched into a single transaction per unit of work, so that failures do not leave the database in an inconsistent state.
12. As a developer, I want soft-delete to also clear the `DeletedAt` timestamp, so that audit trails are complete.
13. As a developer, I want consistent namespace casing across handlers so that the project structure is predictable.
14. As a developer, I want all controllers to inherit from the same base class so that shared behavior is reliable.
15. As a QA engineer, I want tests for every MediatR handler so that auth flows are verified end-to-end.
16. As a QA engineer, I want tests for every FluentValidation validator so that invalid inputs are rejected consistently.
17. As a QA engineer, I want tests for LoginWithRefreshHandler that verify the returned refresh token matches the newly persisted one.
18. As a QA engineer, I want tests for ExceptionMiddleware that assert the response status code is set correctly.
19. As a developer, I want `throw;` used instead of `throw ex;` so that stack traces are preserved.
20. As a developer, I want `ILogger<T>` injected into all services and handlers so that logging follows the project's Serilog requirement.
21. As a developer, I want domain-specific exceptions instead of raw `ApplicationException` so that error handling is explicit and typed.
22. As a developer, I want unused files and empty feature folders removed so that the codebase stays navigable.

## Implementation Decisions

- Boundaries will be restored so that the Application layer depends on interfaces, not Infrastructure. A repository abstraction will be introduced at the Application layer interface level and implemented in Infrastructure.
- Refresh tokens, password reset tokens, and email confirmation tokens will be hashed before being written to the database. Lookups will compare hashes rather than raw values.
- All blocking `.Result` and `.Wait()` calls on async EF Core operations will be refactored to `await`.
- The refresh-token rotation flow in `LoginWithRefreshHandler` will be changed to return the persisted new token rather than the request's stale token.
- Database mutations within a single handler or service call will be grouped so that a single `SaveChangesAsync` closes the transaction, respecting EF Core's implicit unit-of-work behavior.
- The ExceptionMiddleware will set `context.Response.StatusCode` from the exception payload before writing the body.
- The LoggingMiddleware will redact sensitive headers and request/response fields (e.g., passwords, tokens, Authorization header) before writing to logs.
- CORS will be changed from `AllowAnyOrigin` to a named policy backed by explicit frontend origins from configuration.
- Soft-delete will set `DeletedAt` whenever `IsDeleted` is toggled to true inside `SaveChangesAsync`.
- Handler namespaces will be consolidated to one consistent casing scheme.
- All controllers will inherit from the shared `ApiControllerBase`.
- `throw ex;` will be replaced with `throw;` everywhere it appears.
- Domain-specific exception types will be introduced for auth failures (e.g., `InvalidCredentialsException`, `EmailNotConfirmedException`, `InvalidTokenException`).
- `ILogger<T>` will be injected into all handlers and services. `Console.WriteLine` will be removed.
- Dead code (empty `Features/Todo/`, unused validators, placeholder tests) will be removed or implemented.
- Tests will be added at the existing Application, Core, and Api test seams.

## Testing Decisions

- Tests will verify external behavior only: given an input, did the output/state change as specified? Internal implementation details (e.g., exact number of `SaveChangesAsync` calls) will be validated only when they are part of a behavioral contract (transaction atomicity).
- The highest practical seams will be used:
  - Application-layer tests for commands, handlers, and services using EF Core in-memory or SQLite in-memory, with mocked or real `ILogger` and repository abstractions.
  - Api-layer tests for middleware and authorization handlers using `DefaultHttpContext` and Moq.
  - Core-layer tests for validators using FluentValidation's `TestValidate` extensions.
- Prior art: `Tests/Application/AuthServiceTests.cs` (in-memory DbContext + hand-rolled FakeMapper), `Tests/Api/ExceptionMiddlewareTests.cs` (middleware via `DefaultHttpContext`), and `Tests/Core/UserTests.cs` will be extended and modernized.

## Out of Scope

- Multi-tenancy, billing, and production DevOps features listed in the roadmap are out of scope.
- The `Features/Todo/` module remains out of scope until explicitly scoped.
- Database provider expansion beyond SQLite remains out of scope.
- Frontend integration and CORS origin configuration are out of scope beyond changing the default policy.
- MediatR pipeline behavior for validation automation is out of scope for this remediation pass.

## Further Notes

- No ADRs currently exist for this codebase; decisions above should be captured if they become controversial.
- No external issue tracker is configured in `.kilo/`; this spec is stored as a markdown file under `specs/` and should be published to the project's chosen tracker once one is configured.
