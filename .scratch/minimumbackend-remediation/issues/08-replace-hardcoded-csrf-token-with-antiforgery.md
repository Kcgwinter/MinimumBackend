# 08 — Replace hardcoded CSRF token with antiforgery

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] CsrfTokenMiddleware uses IAntiforgery to validate tokens
- [ ] No hardcoded secrets remain in the middleware
- [ ] Endpoint accepts/validates real antiforgery tokens
