# 19 — Write tests for middleware and authorization handlers

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** 07, 08, 10

**Status:** ready-for-agent

- [ ] ExceptionMiddleware tests assert status code and JSON body
- [ ] CsrfTokenMiddleware tests assert token validation without hardcoded secrets
- [ ] LoggingMiddleware tests assert sensitive-field redaction
- [ ] PermissionHandler and PermissionPolicyProvider tests verify policy resolution
