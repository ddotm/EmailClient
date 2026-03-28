## Plan: Unified .NET 10 EmailClient Modernization

Build a rebranded, ASP.NET Core-friendly package surface that supports SMTP, Mailgun, SendGrid, and Office365, implemented strictly in that order, with explicit provider selection by config type (no failover), while modernizing to .NET 10 (preferably multi-target net8.0 + net10.0), enforcing SRP/SoC/DRY, and expanding test coverage before release.

**Steps**
1. [ ] Phase 1 - Baseline, migration, and repository shape guardrails
1.1 [x] Confirm target framework strategy: multi-target net8.0;net10.0 for all shipping projects, fallback to net10.0-only if dependency/tooling constraints emerge.
1.2 [x] Update solution/project metadata and CI pipeline to build/test selected target(s).
1.3 [x] Convert solution from .sln to .slnx and ensure local/devops workflows are updated.
1.4 [x] Rebrand package identity from DdotM.EmailClient.Mailgun to IdempotentCookie.Email across project metadata, CI, and documentation.
1.5 [x] Fix current TLS security risk in Mailgun HTTP adapter (remove unconditional certificate acceptance) before provider expansion.

2. [x] Phase 2 - Introduce unified abstractions and project boundaries (depends on 1)
2.1 [x] Create new IdempotentCookie.Email (Infrastructure) project to host shared provider-agnostic contracts for email sending, message, recipient, provider identification, options binding, and DI extension surface.
2.2 [x] Add a single facade contract for application use (for DI consumers); provider projects are implementation details and must not leak provider-specific types in the main contract.
2.3 [x] Normalize config validation behavior across providers (Mailgun and Office365 currently diverge).

3. [x] Phase 3 - Provider model and runtime selection (depends on 2)
3.1 [x] Implement explicit provider selection by configuration object type in DI using services.AddEmailSending().UseMailgun(config) pattern.
3.2 [x] Preserve provider implementation order strictly as: SMTP, then Mailgun, then SendGrid, then Office365.
3.3 [x] Do not introduce failover behavior or failover extension points.

4. [x] Phase 4 - SMTP and SendGrid implementation (depends on 2, parallelizable)
4.1 [x] Add first-class generic SMTP provider (host/port/security/auth configurable; Office365 no longer the only SMTP path).
4.2 [x] Add SendGrid provider using official API patterns and robust request/response/error handling.
4.3 [x] Refactor Office365 provider to fit unified abstractions while retaining compatibility.
4.4 [x] Align Mailgun provider with shared abstractions and remove provider-specific behavior leaks from the main app contract.
4.5 [x] Ensure each provider has its own project and depends on IdempotentCookie.Email (Infrastructure) only.

5. [x] Phase 5 - ASP.NET Core integration surface (depends on 3,4)
5.1 [x] Add DI extension methods on IServiceCollection using the decided entry point: services.AddEmailSending().Use<Provider>(...).
5.2 [x] One active provider per application in v1; no chaining or fallback. DI is first-class but optional — the package must be usable without DI.
5.3 [x] Add options validation and startup-time diagnostics for invalid configuration.

6. [x] Phase 6 - Architecture optimization and cleanup (parallel with 4/5 where safe)
6.1 [x] Eliminate duplicated recipient/message concepts across provider projects by reusing shared model/contracts.
6.2 [x] Reduce SRP/DRY violations (notably Office365 composition methods and mixed auth/sending responsibilities).
6.3 [x] Keep public classes thin and orchestration-focused; move behavior behind interface-based internal/private services that are independently testable.
6.4 [x] Do not add backward compatibility adapters or obsoletes; old package remains unsupported as-is.

7. [x] Phase 7 - Tests and quality gates (depends on 4,5,6)
7.1 [x] Expand unit tests to cover SMTP, SendGrid, Office365, Mailgun request composition, validation, cancellation, and failure paths.
7.2 [x] Add integration-style tests with mocked HTTP/SMTP seams to verify end-to-end provider behavior.
7.3 [x] Add DI registration tests for AddEmailSupport(configObject) and provider selection behavior.
7.4 [x] Establish minimum coverage targets per provider and for shared abstraction layer.
7.5 [x] Standardize test stack on xunit.v3 + NSubstitute + AwesomeAssertions and adapt existing tests accordingly.

8. [ ] Phase 8 - Packaging, docs, and agent guidance (depends on 7)
8.1 [ ] Update README from Mailgun-only to multi-provider usage and ASP.NET Core examples.
8.2 [ ] Add CONTRIBUTING.md with coding conventions, local setup, testing guidelines, and contribution workflow.
8.3 [ ] Add AGENT.md at repo root with architecture overview, design decisions, and agent guidance for future contributions.
8.4 [ ] Finalize rebranded NuGet packaging metadata for unified package story and versioning notes.
8.5 [ ] Validate APIs and best practices against current official docs during implementation rather than relying on historical assumptions.

**Relevant files**
- IdempotentCookie.Email.slnx - primary solution format and project orchestration.
- .pipelines/azure-pipelines.yml - SDK version and multi-target build/test updates.
- IdempotentCookie.Email.Infrastructure/IdempotentCookie.Email.Infrastructure.csproj - shared abstractions, DI contract surface, public package (IdempotentCookie.Email).
- IdempotentCookie.Email.Mailgun/IdempotentCookie.Email.Mailgun.csproj - Mailgun provider implementation.
- IdempotentCookie.Email.SendGrid/IdempotentCookie.Email.SendGrid.csproj - new provider project (Phase 4).
- IdempotentCookie.Email.Smtp/IdempotentCookie.Email.Smtp.csproj - new provider project (Phase 4).
- IdempotentCookie.Email.Office365/IdempotentCookie.Email.Office365.csproj - Office365 provider implementation.
- IdempotentCookie.Email.Mailgun/HttpClientAdapter.cs - TLS/certificate validation hardening.
- IdempotentCookie.Email.Mailgun/MailgunClient.cs - Mailgun-specific send implementation.
- IdempotentCookie.Email.Mailgun/MailgunEmailClient.cs - IEmailClient adapter for DI.
- IdempotentCookie.Email.Mailgun/MailgunEmailSendingBuilderExtensions.cs - UseMailgun DI extension.
- IdempotentCookie.Email.Mailgun/MailgunClientConfig.cs - validation consistency with unified options model.
- IdempotentCookie.Email.Office365/Office365EmailClient.cs - abstraction alignment and SMTP responsibility cleanup.
- IdempotentCookie.Email.Office365/Office365ClientConfig.cs - validation and options alignment.
- IdempotentCookie.Email.Office365/EmailComposer.cs - DRY cleanup for recipient composition.
- IdempotentCookie.Email.Mailgun.Tests/IdempotentCookie.Email.Mailgun.Tests.csproj - test target updates and test stack standardization.
- IdempotentCookie.Email.Mailgun.Tests/MailgunClientTests.cs - Mailgun-specific coverage baseline.
- IdempotentCookie.Email.Mailgun.Tests/MailgunClientConfigTests.cs - config validation baseline to expand across providers.
- README.md - package positioning and integration docs.
- CONTRIBUTING.md - contributor workflow, coding conventions, local setup, and testing guidance.
- AGENT.md - architecture and agent-focused contributor guidance.

**Verification**
1. Build all projects for each target framework and ensure clean compile.
2. Run full test suite with coverage; verify new provider and DI tests pass.
3. Validate startup-time options validation catches invalid config for each provider type.
4. Execute smoke scenarios for MVC/API style registration using services.AddEmailSending().Use<Provider>(...) and confirm selected provider dispatch.
5. Confirm no insecure certificate bypass remains in HTTP sending path.
6. Validate package artifacts, renamed package identity, and README examples are consistent with shipped API.
7. Confirm .slnx is the canonical solution entry and CI uses it.
8. Confirm no failover logic exists in code paths or contracts.
9. Confirm provider implementations are complete in required order: SMTP, Mailgun, SendGrid, Office365.

**Decisions**
- Package name: IdempotentCookie.Email
- Namespaces: IdempotentCookie.Email.DependencyInjection, IdempotentCookie.Email.Smtp, IdempotentCookie.Email.Mailgun, IdempotentCookie.Email.SendGrid, IdempotentCookie.Email.Office365
- DI entry point: services.AddEmailSending().Use<Provider>(...) — no variants.
- DI scope: one active provider per application in v1; no multi-provider chaining or fallback.
- DI optionality: DI support is first-class but optional; the package must be usable without DI.
- Package shape: single rebranded package is the target public surface.
- Runtime provider behavior: explicit provider selection by config type; no failover support.
- Target framework: prefer net8.0 + net10.0 multi-target; net10.0-only is acceptable fallback if constraints are discovered.
- Provider implementation order is fixed: SMTP > Mailgun > SendGrid > Office365.
- Project boundaries: one provider per project, shared contracts in DdotM.EmailClient.Infrastructure.
- Backward compatibility with old package is not required.

**Further Considerations**
1. SMTP implementation detail: reuse MailKit for generic SMTP transport or adopt System.Net.Mail-compatible path for reduced dependencies.
2. Test strategy depth: include optional live-provider integration tests behind environment flags vs. fully mocked CI-only tests.


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
- Testing tech stack: xunit.v3, NSubstitute for mocking, AwesomeAssertions for assertions. Existing tests must be adapted to this.
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
2. The package is rebranded from DdotM.EmailClient.Mailgun to IdempotentCookie.Email.
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
4. Test stack is standardized to xunit.v3, NSubstitute, and AwesomeAssertions; existing tests are adapted to this standard.
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
