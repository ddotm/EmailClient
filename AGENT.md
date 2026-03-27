# AGENT.md

## C# and Code Style Notes

- Public API models must include XML documentation on all public properties.
- In collection property initialization, use `[]` and do not use `new()`.
- Prefer thin public classes and push behavior behind interface-based collaborators where possible.
- Follow SRP, SoC, and DRY principles consistently.

## Current Architectural Direction

1. Provider implementation order is fixed: SMTP, Mailgun, SendGrid, Office365.
2. No failover behavior is allowed.
3. Shared abstractions and DI extensions form the public contract.
4. Provider projects are implementation details and should not leak provider-specific types into the main contract.
