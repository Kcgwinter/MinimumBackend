# 03 — Hash sensitive tokens before persistence

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] RefreshToken.TokenHash is computed before write; Token column is no longer used for lookup
- [ ] User.PasswordResetToken and EmailConfirmationToken are hashed before write and compared by hash on validation
- [ ] Existing plaintext tokens are handled via migration or one-time rehash on login where possible
