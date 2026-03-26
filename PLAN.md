## Plan: Unified .NET 10 EmailClient Modernization

Build a rebranded, ASP.NET Core-friendly package surface that supports SMTP, Mailgun, SendGrid, and Office365, implemented strictly in that order, with explicit provider selection by config type (no failover), while modernizing to .NET 10 (preferably multi-target net8.0 + net10.0), enforcing SRP/SoC/DRY, and expanding test coverage before release.

**Steps**
1. Phase 1 - Baseline, migration, and repository shape guardrails
1.1 Confirm target framework strategy: multi-target net8.0;net10.0 for all shipping projects, fallback to net10.0-only if dependency/tooling constraints emerge.
1.2 Update solution/project metadata and CI pipeline to build/test selected target(s).
1.3 Convert solution from .sln to .slnx and ensure local/devops workflows are updated.
1.4 Rebrand package identity from DdotM.EmailClient.Mailgun to the new package name across project metadata, CI, and documentation.
1.5 Fix current TLS security risk in Mailgun HTTP adapter (remove unconditional certificate acceptance) before provider expansion.

2. Phase 2 - Introduce unified abstractions and project boundaries (depends on 1)
2.1 Create new DdotM.EmailClient.Infrastructure project to host shared provider-agnostic contracts for email sending, message, recipient, provider identification, options binding, and DI extension surface.
2.2 Add a single facade contract for application use (for DI consumers); provider projects are implementation details and must not leak provider-specific types in the main contract.
2.3 Normalize config validation behavior across providers (Mailgun and Office365 currently diverge).

3. Phase 3 - Provider model and runtime selection (depends on 2)
3.1 Implement explicit provider selection by configuration object type in DI (SmtpConfig, MailgunConfig, SendGridConfig, Office365Config), per agreed requirement.
3.2 Preserve provider implementation order strictly as: SMTP, then Mailgun, then SendGrid, then Office365.
3.3 Do not introduce failover behavior or failover extension points.

4. Phase 4 - SMTP and SendGrid implementation (depends on 2, parallelizable)
4.1 Add first-class generic SMTP provider (host/port/security/auth configurable; Office365 no longer the only SMTP path).
4.2 Add SendGrid provider using official API patterns and robust request/response/error handling.
4.3 Refactor Office365 provider to fit unified abstractions while retaining compatibility.
4.4 Align Mailgun provider with shared abstractions and remove provider-specific behavior leaks from the main app contract.
4.5 Ensure each provider has its own project and depends on DdotM.EmailClient.Infrastructure only.

5. Phase 5 - ASP.NET Core integration surface (depends on 3,4)
5.1 Add DI extension methods on IServiceCollection for easy integration, including single entrypoint AddEmailSupport(configObject).
5.2 Add provider-specific overloads/options where useful, but keep one clear default path for MVC/API projects.
5.3 Add options validation and startup-time diagnostics for invalid configuration.

6. Phase 6 - Architecture optimization and cleanup (parallel with 4/5 where safe)
6.1 Eliminate duplicated recipient/message concepts across provider projects by reusing shared model/contracts.
6.2 Reduce SRP/DRY violations (notably Office365 composition methods and mixed auth/sending responsibilities).
6.3 Keep public classes thin and orchestration-focused; move behavior behind interface-based internal/private services that are independently testable.
6.4 Do not add backward compatibility adapters or obsoletes; old package remains unsupported as-is.

7. Phase 7 - Tests and quality gates (depends on 4,5,6)
7.1 Expand unit tests to cover SMTP, SendGrid, Office365, Mailgun request composition, validation, cancellation, and failure paths.
7.2 Add integration-style tests with mocked HTTP/SMTP seams to verify end-to-end provider behavior.
7.3 Add DI registration tests for AddEmailSupport(configObject) and provider selection behavior.
7.4 Establish minimum coverage targets per provider and for shared abstraction layer.
7.5 Standardize test stack on xUnit + NSubstitute + FluentAssertions and adapt existing tests accordingly.

8. Phase 8 - Packaging, docs, and agent guidance (depends on 7)
8.1 Update README from Mailgun-only to multi-provider usage and ASP.NET Core examples.
8.2 Add CONTRIBUTING.md with coding conventions, local setup, testing guidelines, and contribution workflow.
8.3 Add AGENT.md at repo root with architecture overview, design decisions, and agent guidance for future contributions.
8.4 Finalize rebranded NuGet packaging metadata for unified package story and versioning notes.
8.5 Validate APIs and best practices against current official docs during implementation rather than relying on historical assumptions.

**Relevant files**
- DdotM.EmailClient.sln - source solution to replace.
- DdotM.EmailClient.slnx - new primary solution format and project orchestration.
- .pipelines/azure-pipelines.yml - SDK version and multi-target build/test updates.
- DdotM.EmailClient.Infrastructure/DdotM.EmailClient.Infrastructure.csproj - new shared abstractions and DI contract surface.
- DdotM.EmailClient.Mailgun/DdotM.EmailClient.Mailgun.csproj - target frameworks, provider implementation alignment, and package metadata updates.
- DdotM.EmailClient.SendGrid/DdotM.EmailClient.SendGrid.csproj - new provider project.
- DdotM.EmailClient.Smtp/DdotM.EmailClient.Smtp.csproj - new provider project.
- DdotM.EmailClient.Office365/DdotM.EmailClient.Office365.csproj - target frameworks and provider implementation alignment.
- DdotM.EmailClient.Mailgun/HttpClientAdapter.cs - TLS/certificate validation hardening.
- DdotM.EmailClient.Mailgun/MailgunClient.cs - provider implementation alignment with shared abstractions.
- DdotM.EmailClient.Mailgun/MailgunClientConfig.cs - validation consistency with unified options model.
- DdotM.EmailClient.Office365/Office365EmailClient.cs - abstraction alignment and SMTP responsibility cleanup.
- DdotM.EmailClient.Office365/Office365ClientConfig.cs - validation and options alignment.
- DdotM.EmailClient.Office365/EmailComposer.cs - DRY cleanup for recipient composition.
- DdotM.EmailClient.Mailgun.Tests/DdotM.EmailClient.Mailgun.Tests.csproj - test target updates and test stack standardization.
- DdotM.EmailClient.Mailgun.Tests/MailgunClientTests.cs - provider-agnostic and Mailgun-specific coverage baseline.
- DdotM.EmailClient.Mailgun.Tests/MailgunClientConfigTests.cs - config validation baseline to expand across providers.
- README.md - package positioning and integration docs.
- CONTRIBUTING.md - contributor workflow, coding conventions, local setup, and testing guidance.
- AGENT.md - architecture and agent-focused contributor guidance.

**Verification**
1. Build all projects for each target framework and ensure clean compile.
2. Run full test suite with coverage; verify new provider and DI tests pass.
3. Validate startup-time options validation catches invalid config for each provider type.
4. Execute smoke scenarios for MVC/API style registration using AddEmailSupport(configObject) and confirm selected provider dispatch.
5. Confirm no insecure certificate bypass remains in HTTP sending path.
6. Validate package artifacts, renamed package identity, and README examples are consistent with shipped API.
7. Confirm .slnx is the canonical solution entry and CI uses it.
8. Confirm no failover logic exists in code paths or contracts.
9. Confirm provider implementations are complete in required order: SMTP, Mailgun, SendGrid, Office365.

**Decisions**
- Package shape: single rebranded package is the target public surface.
- Runtime provider behavior: explicit provider selection by config type; no failover support.
- Target framework: prefer net8.0 + net10.0 multi-target; net10.0-only is acceptable fallback if constraints are discovered.
- Provider implementation order is fixed: SMTP > Mailgun > SendGrid > Office365.
- Project boundaries: one provider per project, shared contracts in DdotM.EmailClient.Infrastructure.
- Backward compatibility with old package is not required.

**Further Considerations**
1. SMTP implementation detail: reuse MailKit for generic SMTP transport or adopt System.Net.Mail-compatible path for reduced dependencies.
2. Test strategy depth: include optional live-provider integration tests behind environment flags vs. fully mocked CI-only tests.
3. Rebrand details: decide final package ID, assembly names, namespaces, and migration messaging in docs.


## DIMA'S NOTES
- SMTP > Mailgun > SendGrid > Office365 - that is the preferred implementation order, nothing else. There will be no failover whatsoever. Take failover provisions out of the spec.
- Convert .sln to .slnx 
- I stupidly named the nuget DdotM.EmailClient.Mailgun. It will have to be rebranded and published under a different name. That will affect proj metadata, build yml etc.
- Each provider should have its own project. The shared abstractions will be in DdotM.EmailClient.Infrastracture project (new).
- The public contract for the package will be the shared abstractions + DI extensions. Each provider project will be an implementation detail and not expose provider-specific types in the main contract.
- Closely adhere to Single Responsibility, Separation of Concerns, and DRY Principles across the codebase.
- Docs that must be updated or added
  - README.md
  - CONTRIBUTING.md (add coding conventions, local setup, testing guidelines, and contribution workflow)
  - AGENT.md (new - architecture overview, design decisions, and agent guidance for future contributions)
- Testing tech stack: xUnit, NSubstitute for mocking, FluentAssertions for assertions. Existing tests must be adapted to this.
- As much as possible, public classes must be very thin. Interface based private classes should be fully testable.
- Backward compatability is not a concern at all. This nuget will be published under a new name. Old one will just exist out there as-is and be unsupported. No need for adapters or obsoletes.
- Don't rely on your training data alone. Whenever possible, read web documentation to know the latest versions' public API and best practices.

- # ADR 0001: Provider Strategy and Selection Model

- Status: Accepted
- Date: 2026-03-25

## Context
The package must support multiple email providers while remaining simple for ASP.NET Core applications. The required provider priority is:
1. SMTP
2. Mailgun
3. SendGrid
4. Office365

There was ambiguity in the draft plan about whether provider priority implied runtime failover behavior.

## Decision
1. Provider implementation and delivery order is fixed to SMTP, then Mailgun, then SendGrid, then Office365.
2. Runtime behavior is explicit provider selection by configuration type (for example: SmtpConfig, MailgunConfig, SendGridConfig, Office365Config).
3. No failover behavior will be implemented.
4. No failover extension points will be included in the public contract for this release.

## Consequences
1. Runtime behavior remains deterministic and easy to reason about.
2. Failure handling is the responsibility of consuming applications and calling workflows.
3. The package avoids hidden delivery retries or cross-provider side effects.
4. The architecture stays open to future failover ADRs without adding accidental complexity now.

# ADR 0002: Solution Structure, Packaging, and Public Contract

- Status: Accepted
- Date: 2026-03-25

## Context
The repository started as a Mailgun-focused package and now needs to become a rebranded multi-provider package. Existing structure and naming do not match the desired long-term architecture.

## Decision
1. The solution format is migrated from .sln to .slnx as the canonical solution entry.
2. The package is rebranded from DdotM.EmailClient.Mailgun to a new package identity.
3. Shared abstractions and DI contract live in a dedicated project: DdotM.EmailClient.Infrastructure.
4. Each provider has its own project and depends on the infrastructure abstractions.
5. The only public package contract is shared abstractions plus DI extension methods.
6. Provider-specific implementation types are not exposed through the main consumer contract.
7. Backward compatibility with the existing Mailgun package is explicitly out of scope.

## Consequences
1. Consumers get a clean, provider-agnostic integration surface suitable for ASP.NET Core MVC and API projects.
2. Provider implementations can evolve independently with reduced coupling.
3. Rebranding affects project metadata, CI configuration, package publishing, and documentation.
4. No adapter or obsolete layers are required, reducing maintenance burden.

# ADR 0003: Engineering Principles, Test Stack, and Documentation Baseline

- Status: Accepted
- Date: 2026-03-25

## Context
The modernization effort requires consistent architectural discipline and quality standards across a growing multi-project solution.

## Decision
1. Code changes must adhere to Single Responsibility, Separation of Concerns, and DRY.
2. Public classes should be thin orchestration surfaces wherever possible.
3. Behavior is pushed behind interface-based internal/private services that are independently testable.
4. Test stack is standardized to xUnit, NSubstitute, and FluentAssertions; existing tests are adapted to this standard.
5. Required documentation set includes:
   - README.md updates
   - CONTRIBUTING.md (coding conventions, local setup, testing guidelines, contribution workflow)
   - AGENT.md (architecture overview, design decisions, agent guidance)
6. Implementation choices should be validated against current official documentation when possible, rather than relying only on historical assumptions.

## Consequences
1. The codebase remains maintainable as providers and integrations grow.
2. Tests align with architecture boundaries and support safer refactoring.
3. Contributor onboarding and agent-assisted work become more reliable through explicit documentation.
4. The team accepts up-front documentation and design effort to reduce long-term delivery risk.
