# 05 — Fix stale refresh token in loginWithRefresh flow

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] LoginWithRefreshHandler returns the newly persisted refresh token
- [ ] Old token is invalidated atomically with new token creation
