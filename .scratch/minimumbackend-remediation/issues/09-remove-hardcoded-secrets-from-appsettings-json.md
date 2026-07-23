# 09 — Remove hardcoded secrets from appsettings.json

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] Jwt:Key is removed from appsettings.json and appsettings.Development.json
- [ ] SmtpSettings credentials are removed from appsettings files
- [ ] Secrets are loaded from user-secrets or environment variables via code
