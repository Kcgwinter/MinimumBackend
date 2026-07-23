# 12 — Batch SaveChangesAsync for atomic unit-of-work

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] Multiple entity mutations in one handler/service call hit a single SaveChangesAsync
- [ ] Transaction atomicity is preserved across the batch
