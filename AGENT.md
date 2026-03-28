# AGENT.md

## Repo Map

- `IdempotentCookie.Email.Infrastructure/` contains shared contracts, models, and DI entry points.
- `IdempotentCookie.Email.Smtp/` contains the SMTP provider.
- `IdempotentCookie.Email.Mailgun/` contains the Mailgun provider.
- `IdempotentCookie.Email.SendGrid/` contains the SendGrid provider.
- `IdempotentCookie.Email.Office365/` contains the Office365 provider.
- `IdempotentCookie.Email.Package/` contains NuGet packaging metadata and package composition.
- `IdempotentCookie.Email.*.Tests/` contains unit and registration tests.
- `.pipelines/azure-pipelines.yml` contains CI and package publishing rules.

## Architecture Rules

- Provider order is fixed: SMTP, Mailgun, SendGrid, Office365.
- No failover logic is allowed.
- One active provider per application is allowed.
- Public contract is shared models, `IEmailClient`, config types, `CreateClient()` extensions, and `AddEmailSending().Use<Provider>(...)`.
- Provider implementation details stay out of the infrastructure project.
- Public classes stay thin; behavior belongs in focused collaborators.

## Code Style

- Follow `.editorconfig`.
- Keep public API XML docs current.
- Use `[]` for empty collection initialization.
- Add comments only when they remove real ambiguity.

## Test Rules

- Use xunit.v3, NSubstitute, and AwesomeAssertions.
- Use explicit `Arrange`, `Act`, and `Assert` sections.
- Name tests `ClassName_Method_Condition_ExpectedOutcome`.
- Use `TestContext.Current.CancellationToken` when testing cancellable APIs.

## Validation Commands

- `dotnet restore IdempotentCookie.Email.slnx`
- `dotnet build IdempotentCookie.Email.slnx -c Release`
- `dotnet test IdempotentCookie.Email.slnx -c Release`
- `dotnet pack IdempotentCookie.Email.Package/IdempotentCookie.Email.Package.csproj -c Release --no-build`

## Release Rules

- Default branch is `main`.
- Build on every push to `main`.
- Publish to NuGet from `main` only when functional package files changed.
- License is MIT.
