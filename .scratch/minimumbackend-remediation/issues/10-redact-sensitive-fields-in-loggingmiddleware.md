# 10 — Redact sensitive fields in LoggingMiddleware

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] Authorization header is masked or omitted
- [ ] Password, token, and PII fields in request/response bodies are redacted before logging
- [ ] Structured logging placeholders still work for non-sensitive fields
