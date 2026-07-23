# 04 — Eliminate sync-over-async blocking in AuthService

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] RevokeRefreshTokenAsync uses await instead of .Result
- [ ] ResetPasswordAsync uses await instead of .Result
- [ ] ConfirmEmailAsync uses await instead of .Result
- [ ] RequestEmailConfirmationAsync uses await instead of .Result
