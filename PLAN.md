## Plan: Unified .NET 10 EmailClient Modernization

Build a single, ASP.NET Core-friendly package surface that supports SMTP, Mailgun, SendGrid, and Office365 with explicit provider selection by config type, while modernizing to .NET 10 (preferably multi-target net8.0 + net10.0), addressing current security and architecture gaps, and expanding test coverage before release.

**Steps**
1. Phase 1 - Baseline and migration guardrails
1.1 Confirm target framework strategy: multi-target net8.0;net10.0 for all shipping projects, fallback to net10.0-only if dependency/tooling constraints emerge.
1.2 Update solution/project metadata and CI pipeline to build/test selected target(s).
1.3 Fix current TLS security risk in Mailgun HTTP adapter (remove unconditional certificate acceptance) before provider expansion.

2. Phase 2 - Introduce unified abstractions (depends on 1)
2.1 Create shared provider-agnostic contracts for email sending, message, recipient, and provider identification.
2.2 Add a single facade interface for application use (for DI consumers) and keep provider-specific clients as implementation details or adapters.
2.3 Normalize config validation behavior across providers (Mailgun and Office365 currently diverge).

3. Phase 3 - Provider model and runtime selection (depends on 2)
3.1 Implement explicit provider selection by configuration object type (SmtpConfig, SendGridConfig, MailgunConfig, Office365Config), per agreed requirement.
3.2 Keep provider priority ordering (SMTP > Mailgun > SendGrid > Office365) as documentation/default registration precedence only, not automatic failover.
3.3 Define extensibility point for optional failover policy in future without changing current public contract.

4. Phase 4 - SMTP and SendGrid implementation (depends on 2, parallelizable)
4.1 Add first-class generic SMTP provider (host/port/security/auth configurable; Office365 no longer the only SMTP path).
4.2 Add SendGrid provider using official API patterns and robust request/response/error handling.
4.3 Refactor Office365 provider to fit unified abstractions while retaining compatibility.
4.4 Align Mailgun provider with shared abstractions and remove provider-specific behavior leaks from the main app contract.

5. Phase 5 - ASP.NET Core integration surface (depends on 3,4)
5.1 Add DI extension methods on IServiceCollection for easy integration, including single entrypoint AddEmailSupport(configObject).
5.2 Add provider-specific overloads/options where useful, but keep one clear default path for MVC/API projects.
5.3 Add options validation and startup-time diagnostics for invalid configuration.

6. Phase 6 - Architecture optimization and cleanup (parallel with 4/5 where safe)
6.1 Eliminate duplicated recipient/message concepts across provider projects by reusing shared model/contracts.
6.2 Reduce SRP/DRY violations (notably Office365 composition methods and mixed auth/sending responsibilities).
6.3 Preserve backward compatibility where practical via adapters/obsolete attributes and release notes.

7. Phase 7 - Tests and quality gates (depends on 4,5,6)
7.1 Expand unit tests to cover SMTP, SendGrid, Office365, Mailgun request composition, validation, cancellation, and failure paths.
7.2 Add integration-style tests with mocked HTTP/SMTP seams to verify end-to-end provider behavior.
7.3 Add DI registration tests for AddEmailSupport(configObject) and provider selection behavior.
7.4 Establish minimum coverage targets per provider and for shared abstraction layer.

8. Phase 8 - Packaging, docs, and agent guidance (depends on 7)
8.1 Update README from Mailgun-only to multi-provider usage and ASP.NET Core examples.
8.2 Add AGENT.md at repo root with architecture, coding/testing conventions, and contributor workflow.
8.3 Finalize NuGet packaging metadata for unified package story and versioning notes.

**Relevant files**
- DdotM.EmailClient.sln - solution-level project layout updates and potential new shared/aggregate project inclusion.
- .pipelines/azure-pipelines.yml - SDK version and multi-target build/test updates.
- DdotM.EmailClient.Mailgun/DdotM.EmailClient.Mailgun.csproj - target frameworks and package metadata adjustments.
- DdotM.EmailClient.Office365/DdotM.EmailClient.Office365.csproj - target frameworks and dependency/version checks.
- DdotM.EmailClient.Mailgun/HttpClientAdapter.cs - TLS/certificate validation hardening.
- DdotM.EmailClient.Mailgun/MailgunClient.cs - alignment with unified sender abstractions.
- DdotM.EmailClient.Mailgun/MailgunClientConfig.cs - validation consistency with unified options model.
- DdotM.EmailClient.Office365/Office365EmailClient.cs - abstraction alignment and SMTP responsibility cleanup.
- DdotM.EmailClient.Office365/Office365ClientConfig.cs - add/align validation and options patterns.
- DdotM.EmailClient.Office365/EmailComposer.cs - DRY cleanup for recipient composition.
- DdotM.EmailClient.Mailgun.Tests/DdotM.EmailClient.Mailgun.Tests.csproj - test target updates and shared test infra references.
- DdotM.EmailClient.Mailgun.Tests/MailgunClientTests.cs - expanded provider-agnostic and Mailgun-specific coverage baseline.
- DdotM.EmailClient.Mailgun.Tests/MailgunClientConfigTests.cs - config validation baseline to expand across providers.
- README.md - package positioning and integration docs.

**Verification**
1. Build all projects for each target framework and ensure clean compile.
2. Run full test suite with coverage; verify new provider and DI tests pass.
3. Validate startup-time options validation catches invalid config for each provider type.
4. Execute smoke scenarios for MVC/API style registration using AddEmailSupport(configObject) and confirm selected provider dispatch.
5. Confirm no insecure certificate bypass remains in HTTP sending path.
6. Validate package artifacts and README examples are consistent with shipped API.

**Decisions**
- Package shape: single unified package is the target surface.
- Runtime provider behavior: explicit provider selection by config type; no mandatory automatic failover in this phase.
- Target framework: prefer net8.0 + net10.0 multi-target; net10.0-only is acceptable fallback if constraints are discovered.
- Priority ordering is treated as implementation/default registration precedence and documentation guidance.

**Further Considerations**
1. Backward compatibility strategy: keep existing provider-specific interfaces public for one deprecation cycle, or hard break to unified interface now.
2. SMTP implementation detail: reuse MailKit for generic SMTP transport or adopt System.Net.Mail-compatible path for reduced dependencies.
3. Test strategy depth: include optional live-provider integration tests behind environment flags vs. fully mocked CI-only tests.


DIMA'S NOTES:`
SMTP > Mailgun > SendGrid > Office365 - that is the preferred implementation order, nothing else. There will be no failover whatsoever. Take failover provisions out of the spec.

Convert .sln to .slnx 

I stupidly named the nuget DdotM.EmailClient.Mailgun. This will have to be rebranded and published under a different name. That will affect proj metadata, build yml etc.