# 01 — Add repository interfaces for User and RefreshToken

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] IUserRepository interface exists in Application layer with async methods for user CRUD and lookup by username/email
- [ ] IRefreshTokenRepository interface exists in Application layer with async methods for token CRUD and lookup by token hash
- [ ] Both interfaces are registered in DI and can be mocked in tests
