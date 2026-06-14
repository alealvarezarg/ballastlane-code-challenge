# Implementation Plan: API Project

**Branch**: `005-api-project` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/005-api-project/spec.md`

## Summary

Add a focused API delivery layer for `ManagementTask` CRUD by introducing a thin
`ManagementTaskController`, DTO request/response models, FluentValidation-based DTO validation,
global exception handling, and a root-level OpenAPI YAML contract while registering existing
Application and Infrastructure dependencies through their extension methods.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core Web API, Application project, Infrastructure project, `Microsoft.AspNetCore.OpenApi`, FluentValidation.AspNetCore

**Storage**: LiteDB through the Infrastructure layer

**Testing**: xUnit + Shouldly + API/integration-style tests for controller endpoints, validation, and exception handling

**Target Platform**: .NET backend web API

**Project Type**: Clean Architecture backend API

**Performance Goals**: CRUD requests for `ManagementTask` should complete with normal local development responsiveness and expose documentation without extra setup

**Constraints**: Controllers must remain translation-only, each API DTO must have a dedicated FluentValidation validator where validation applies, domain entities must not be exposed through HTTP contracts, global exception handling must provide consistent error responses, and the root `openapi/openapi.yaml` file must stay synchronized with API behavior

**Scale/Scope**: One controller, one DTO model set, one validator set, one exception-handling entry point, and one OpenAPI-documented CRUD surface for `ManagementTask`

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Domain and application behavior stays isolated from frameworks, transport, and persistence.
- Business rules, validation, and use-case decisions live outside controllers, handlers, and repositories.
- Required automated tests are identified for every changed business behavior and boundary crossing.
- Simpler designs were preferred; any added abstraction has an explicit justification.
- Dependency injection, API contract consistency, and documentation updates are accounted for.

**Gate Status**: Pass. The design keeps HTTP translation, DTO validation, and exception handling in
the API layer while leaving business rules and persistence behavior in Application and
Infrastructure.

## Project Structure

### Documentation (this feature)

```text
specs/005-api-project/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- management-tasks.openapi.yaml
`-- tasks.md

openapi/
`-- openapi.yaml
```

### Source Code (repository root)

```text
src/
|-- TaskManagementSystem.Api/
|   |-- Controllers/
|   |-- ExceptionHandling/
|   |-- Models/
|   |-- Validation/
|   |-- Program.cs
|   `-- appsettings.json
|-- TaskManagementSystem.Application/
|   `-- Abstractions/
`-- TaskManagementSystem.Infrastructure/

tests/
|-- TaskManagementSystem.Api.UnitTests/
|   |-- Controllers/
|   |-- ExceptionHandling/
|   `-- Validation/
`-- TaskManagementSystem.Infrastructure.UnitTests/
```

**Structure Decision**: Use the existing `src/TaskManagementSystem.Api` project for all HTTP
delivery concerns, keep DTOs and validators local to the API layer with one validator per DTO
that accepts request validation, keep an API contract artifact under the feature docs for planning
traceability, and maintain the canonical OpenAPI document at `openapi/openapi.yaml`.

## Phase Plan

### Phase 0

- Confirm the simplest API contract shape for task CRUD endpoints.
- Confirm how FluentValidation should be applied with one validator per API DTO without moving business rules out of lower layers.
- Confirm the global exception-handling strategy and response shape for validation, not-found, and unexpected failures.
- Confirm how the root `openapi/openapi.yaml` file stays aligned with the implemented endpoints.

### Phase 1

- Design DTO request and response models under `Models`.
- Design one FluentValidation validator per API DTO under `Validation`.
- Design controller routing, status-code behavior, and exception translation.
- Design API dependency registration and OpenAPI exposure in `Program.cs`.
- Design the root `openapi/openapi.yaml` file as the canonical external contract.

### Phase 2

- Add API tests for controller behavior, validation failures, dependency wiring, exception responses, and OpenAPI contract alignment.
- Validate that the API remains a thin delivery layer and does not expose domain entities.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |
