# 16 — Unify all controllers under ApiControllerBase

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] AuthController, RBACTestController, and MainController all inherit ApiControllerBase
- [ ] Shared behavior (versioning, route prefix) works consistently
