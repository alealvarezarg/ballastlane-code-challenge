# Implementation Plan: ManagementUser Authentication

**Branch**: `007-management-user-auth` | **Date**: 2026-06-14 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/007-management-user-auth/spec.md`

## Summary

Add a minimal `ManagementUser` authentication flow with three public endpoints for registration, login, and test-only retrieval, then require authentication everywhere else, including the full `ManagementTask` surface. The design keeps controllers thin, uses standard password hashing so plaintext passwords are never stored or returned, validates task ownership during create, and follows the existing four-layer architecture and TDD workflow.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core Web API, FluentValidation.AspNetCore, LiteDB, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.Identity`, `Microsoft.AspNetCore.OpenApi`

**Storage**: LiteDB through the Infrastructure layer

**Testing**: xUnit, Shouldly, API unit/contract-style tests, Application unit tests, Domain unit tests, Infrastructure unit tests

**Target Platform**: .NET backend web API

**Project Type**: Clean Architecture backend web service

**Performance Goals**: Authentication and task authorization should remain responsive for local-development usage and small team datasets without introducing extra moving parts beyond the existing in-process API and LiteDB store.

**Constraints**: TDD is mandatory; controllers remain translation-only; authentication must stay as simple as possible; passwords must never be stored or exposed in plain text; request and response models must stay DTO-only; FluentValidation and the global exception pipeline remain the validation/error boundary; all non-public endpoints, including every ManagementTask endpoint, must require authentication.

**Scale/Scope**: Adds one new secured user capability with three public endpoints, one simple token-issuing login flow, consistent authorization on the existing API surface, task-create ownership validation, dependency wiring, OpenAPI updates, and synchronized tests across Domain, Application, Infrastructure, and API projects.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Domain and application behavior stays isolated from frameworks, transport, and persistence.
- Business rules, validation, and use-case decisions live outside controllers, handlers, and repositories.
- Required automated tests are identified for every changed business behavior and boundary crossing.
- Simpler designs were preferred; any added abstraction has an explicit justification.
- Dependency injection, API contract consistency, and documentation updates are accounted for.

**Gate Status**: Pass. The planned design uses the existing project boundaries, keeps authentication intentionally lean with built-in framework primitives, places password hashing and token generation behind application-facing contracts, and keeps enforcement/documentation changes explicit.

## Project Structure

### Documentation (this feature)

```text
specs/007-management-user-auth/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- management-users-auth.openapi.yaml
`-- tasks.md
```

### Source Code (repository root)

```text
src/
|-- TaskManagementSystem.Api/
|   |-- Controllers/
|   |-- ExceptionHandling/
|   |-- Models/
|   |-- Validation/
|   |-- ApiServiceCollectionExtensions.cs
|   `-- Program.cs
|-- TaskManagementSystem.Application/
|   |-- Abstractions/
|   |-- Models/
|   `-- Services/
|-- TaskManagementSystem.Domain/
|   |-- Entities/
|   `-- Exceptions/
`-- TaskManagementSystem.Infrastructure/
    |-- Database/
    `-- Repositories/

tests/
|-- TaskManagementSystem.Api.UnitTests/
|   |-- Controllers/
|   `-- Validation/
|-- TaskManagementSystem.Application.UnitTests/
|-- TaskManagementSystem.Domain.UnitTests/
`-- TaskManagementSystem.Infrastructure.UnitTests/

openapi/
`-- openapi.yaml
```

**Structure Decision**: Preserve the existing four-project backend structure. Extend Domain only for management user invariants and any feature-specific exceptions, Application for registration, login, token issuance, and task-create user existence checks, Infrastructure for LiteDB user persistence plus password hashing/token helpers, and API for DTOs, validators, controller translation, authorization attributes, and OpenAPI alignment.

## Phase Plan

### Phase 0

- Confirm the simplest authentication approach that satisfies the spec: credential-based login with built-in password hashing and short bearer tokens for protected endpoints.
- Confirm where password hashing, token generation, and credential verification should live so business orchestration stays outside controllers.
- Confirm the simplest authorization rollout for the existing API surface, especially every `ManagementTask` route.
- Confirm how task creation should fail when the referenced management user does not exist while preserving the current validation and exception pipeline.
- Confirm the TDD slice order so each story starts with failing tests in the owning layer before implementation begins.

### Phase 1

- Design the management user data model, registration/login DTOs, authentication result DTO, and persistence document.
- Design application contracts for user lookup, user creation, password hashing, token issuance, and task-create user existence checks.
- Design the public management user endpoints, security scheme, and protected-route expectations in the OpenAPI contract.
- Design the validation rules, authorization failure expectations, and error-handling interactions needed to preserve the current API behavior.
- Update the agent context to point to this plan.

### Phase 2

- Implement in strict TDD order:
  1. Domain tests for management user invariants and any new user-specific validation exceptions.
  2. Application tests for registration, login success/failure, token issuance orchestration, protected-task create validation, and task-user existence checks.
  3. Infrastructure tests for LiteDB management user persistence, uniqueness checks, password hash verification, and token helper behavior.
  4. API tests for public-versus-protected endpoints, authorization failures, DTO validation, controller mappings, and OpenAPI alignment.
- Keep refactors within each slice after tests pass and before moving to the next behavior.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |
