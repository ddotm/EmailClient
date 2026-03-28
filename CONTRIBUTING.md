# Contributing

## Prerequisites

- .NET SDK 10.x
- Git
- PowerShell 7+ if you want to run the commands below unchanged on Windows

## Branch Workflow

1. Branch from `main`.
2. Keep branches short-lived.
3. Open pull requests against `main`.

## Local Workflow

1. Run `dotnet restore IdempotentCookie.Email.slnx`.
2. Run `dotnet build IdempotentCookie.Email.slnx -c Release`.
3. Run `dotnet test IdempotentCookie.Email.slnx -c Release`.
4. Run `dotnet pack IdempotentCookie.Email.Package/IdempotentCookie.Email.Package.csproj -c Release --no-build` when packaging changes are involved.

## Coding Rules

- Keep public classes thin.
- Put behavior behind focused collaborators.
- Follow Single Responsibility Principle (SRP), Separation of Concerns (SoC), and Don't Repeat Yourself (DRY).
- Do not add failover logic for email providers.
- Keep the public contract provider-agnostic where possible.

## Test Rules

- Use xunit.v3, NSubstitute, and AwesomeAssertions.
- Use explicit `Arrange`, `Act`, and `Assert` sections.
- Name tests `ClassName_Method_Condition_ExpectedOutcome`.
- Cover validation, cancellation, and failure paths for provider changes.

## Pull Request Checklist

1. Build passes.
2. Tests pass.
3. Docs are updated when usage or workflow changed.
4. Public API and packaging changes are called out in the PR description.