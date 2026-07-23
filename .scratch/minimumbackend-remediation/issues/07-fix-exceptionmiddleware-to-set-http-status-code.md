# 07 — Fix ExceptionMiddleware to set HTTP status code

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] context.Response.StatusCode is set from the exception or default before writing the body
- [ ] Existing ExceptionMiddlewareTests pass with the corrected behavior
