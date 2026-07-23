# 02 — Introduce typed domain exceptions for auth failures

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [x] InvalidCredentialsException replaces generic auth-failure messages
- [x] EmailNotConfirmedException replaces email-not-confirmed message
- [x] InvalidTokenException replaces invalid/expired token messages
- [x] All existing catch sites and tests are updated
