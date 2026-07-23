# 13 — Fix soft-delete to populate DeletedAt

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] SaveChangesAsync override sets DeletedAt = DateTime.UtcNow when IsDeleted is toggled to true
- [ ] Existing soft-delete tests still pass
