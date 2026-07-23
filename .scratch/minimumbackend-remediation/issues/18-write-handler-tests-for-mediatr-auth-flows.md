# 18 — Write handler tests for MediatR auth flows

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** 03, 04, 05, 06

**Status:** ready-for-agent

- [ ] Tests cover register, login, loginWithRefresh, forgot/reset password, confirm email, and revoke refresh token
- [ ] Tests verify new token-hashing and deadlock-free behavior
- [ ] Tests use the repository-backed AuthService
