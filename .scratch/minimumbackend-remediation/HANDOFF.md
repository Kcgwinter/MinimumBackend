# Handoff — MinimumBackend Codebase Remediation

Date: 2026-07-22
Source: Code review + grill + to-spec + ticket breakdown session

## What This Is

A bridge document for the next session to pick up implementation without re-solving the design. The next session should read this, then use `/implement` against the tickets in `.scratch/minimumbackend-remediation/issues/`.

## Project Context

- **Repo:** `F:\Dev\MinimumBackend`
- **Stack:** .NET 10, Clean Architecture (Core → Application → Infrastructure → Api), EF Core 10 (SQLite), MediatR 14, FluentValidation, AutoMapper, Serilog
- **Domain:** Authentication, authorization, RBAC. Entities: User, Role, Permission, RefreshToken. BaseEntity with audit fields (CreatedAt, UpdatedAt, IsDeleted).
- **Key constraint from AGENTS.md:** Serilog for all logging, ILogger<T> injection, xUnit tests for every change, Clean Architecture boundaries enforced.

## Where We Left Off

1. Completed a full codebase review.
2. Produced a remediation spec at `specs/2026-07-22-codebase-remediation.md`.
3. Broke the spec into tracer-bullet tickets in dependency order.

## Critical Findings (from code review)

| Priority | Issue | Location | Fix direction |
|----------|-------|----------|---------------|
| P0 | Hardcoded secrets in appsettings.json | appsettings.json, appsettings.Development.json | Move to user-secrets/env vars |
| P0 | CSRF middleware hardcoded token | Api/Middleware/CsrfTokenMiddleware.cs | Replace with ASP.NET Core IAntiforgery |
| P0 | Application → Infrastructure dependency | Application/Services/AuthService.cs, Application.csproj | Introduce repo interfaces; DI in Api layer |
| P0 | Sync-over-async .Result deadlock risk | AuthService.cs (RevokeRefreshTokenAsync, ResetPasswordAsync, ConfirmEmailAsync, RequestEmailConfirmationAsync) | Replace all .Result/.Wait() with await |
| P0 | Refresh tokens stored in plaintext | Core/Entities/RefreshToken.cs, RefreshTokenHash unused | Hash before persist; use RefreshTokenHash for lookups |
| P1 | LoginWithRefresh returns stale token | Application/Handler/LoginWithRefreshHandler.cs line 24 | Return newToken.refreshToken |
| P1 | ExceptionMiddleware missing status code | Api/Middleware/ExceptionMiddleware.cs | Set context.Response.StatusCode |
| P1 | LoggingMiddleware logs raw request/response bodies including secrets | Api/Middleware/LoggingMiddleware.cs | Redact sensitive fields |
| P1 | CORS AllowAnyOrigin | Api/Program.cs lines 103-111 | Restrict to configured origins |
| P1 | Multiple SaveChangesAsync per op | AuthService.cs | Batch into single transaction |
| P1 | Soft-delete never sets DeletedAt | Infrastructure/Data/AppDbContext.cs SaveChangesAsync | Set DeletedAt when IsDeleted toggled |
| P2 | throw ex; resets stack trace | LoggingMiddleware.cs line 34 | Use throw; |
| P2 | Console.WriteLine + no Serilog in handlers/services | AuthService.cs, EmailService.cs | Inject ILogger<T> |
| P2 | Mixed handler namespaces | Application/Handler vs Application/Handlers | Consolidate |
| P2 | Controllers inconsistent base class | MainController inherits ControllerBase | All inherit ApiControllerBase |
| P3 | Empty Features/Todo/, unused validators, placeholder tests | Various | Remove or implement |
| P3 | FakeMapper boilerplate in tests | Tests/Application/AuthServiceTests.cs | Use real AutoMapper config |

## Approved Ticket Breakdown

Tickets are written under `.scratch/minimumbackend-remediation/issues/` and numbered in dependency order. The frontier (all unblocked) is: 01, 02, 03, 04, 05, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 20.

| # | Title | Blocked by | Status |
|---|-------|------------|--------|
| 01 | Add repository interfaces for User and RefreshToken | None | ready-for-agent |
| 02 | Introduce typed domain exceptions for auth failures | None | ready-for-agent |
| 03 | Hash sensitive tokens before persistence | None | ready-for-agent |
| 04 | Eliminate sync-over-async blocking in AuthService | None | ready-for-agent |
| 05 | Fix stale refresh token in loginWithRefresh flow | None | ready-for-agent |
| 06 | Migrate AuthService to repository interfaces | 01, 03, 04 | ready-for-agent |
| 07 | Fix ExceptionMiddleware to set HTTP status code | None | ready-for-agent |
| 08 | Replace hardcoded CSRF token with antiforgery | None | ready-for-agent |
| 09 | Remove hardcoded secrets from appsettings.json | None | ready-for-agent |
| 10 | Redact sensitive fields in LoggingMiddleware | None | ready-for-agent |
| 11 | Restrict CORS from AllowAnyOrigin to named policy | None | ready-for-agent |
| 12 | Batch SaveChangesAsync for atomic unit-of-work | None | ready-for-agent |
| 13 | Fix soft-delete to populate DeletedAt | None | ready-for-agent |
| 14 | Consolidate handler namespace casing | None | ready-for-agent |
| 15 | Inject ILogger<T> and replace Console.WriteLine | None | ready-for-agent |
| 16 | Unify all controllers under ApiControllerBase | None | ready-for-agent |
| 17 | Replace throw ex; with throw; | None | ready-for-agent |
| 18 | Write handler tests for MediatR auth flows | 03, 04, 05, 06 | ready-for-agent |
| 19 | Write tests for middleware and authorization handlers | 07, 08, 10 | ready-for-agent |
| 20 | Write tests for FluentValidation validators | None | ready-for-agent |
| 21 | Remove dead code, unused files, empty feature folders | 14, 16 | ready-for-agent |

## How the Next Session Should Start

1. Read this handoff file.
2. Read the ticket list: `ls .scratch/minimumbackend-remediation/issues/`
3. Pick an unblocked ticket from the frontier.
4. Invoke `/implement` and reference the specific ticket file as the input.

## Key Seams for Testing

- Application handlers/services (highest seam for auth business logic). Prior art: `Tests/Application/AuthServiceTests.cs`.
- Api middleware using `DefaultHttpContext` + Moq. Prior art: `Tests/Api/ExceptionMiddlewareTests.cs`.
- Core validators via FluentValidation `TestValidate`. Current tests are trivial; modernize in ticket 20.

## Constraints / Non-negotiables

- No secrets in committed files.
- Clean Architecture must be respected (Core zero external deps beyond FluentValidation; Application depends on abstractions, not Infrastructure).
- All async EF Core must be awaited; no `.Result` or `.Wait()`.
- Serilog + ILogger<T> everywhere; no Console.WriteLine in production code.
- Every behavioral change needs an xUnit test per AGENTS.md.

## Out of Scope

- Multi-tenancy, billing, production DevOps.
- Features/Todo module (empty, not part of spec).
- Database provider expansion beyond SQLite.
- MediatR validation pipeline behavior (deferred to later pass).
