# 15 — Inject ILogger<T> and replace Console.WriteLine

**What to build:** the end-to-end behaviour this ticket makes work, from the user's perspective — not a layer-by-layer implementation list.

**Blocked by:** None — can start immediately

**Status:** ready-for-agent

- [ ] AuthService and EmailService accept ILogger<T> via constructor
- [ ] Console.WriteLine is removed from production code
- [ ] Logs use Serilog-backed structured logging
