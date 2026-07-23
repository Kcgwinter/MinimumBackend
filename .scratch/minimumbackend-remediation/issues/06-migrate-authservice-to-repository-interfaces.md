# 06 — Migrate AuthService to repository interfaces

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** 01, 03, 04

**Status:** ready-for-agent

- [ ] AuthService depends on IUserRepository and IRefreshTokenRepository, not AppDbContext
- [ ] Application.csproj no longer references Infrastructure.csproj
- [ ] DI registrations for repositories live in Api/Program.cs
