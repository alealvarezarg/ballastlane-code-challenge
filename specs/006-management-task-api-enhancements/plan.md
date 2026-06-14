# Implementation Plan: ManagementTask API Enhancements

**Branch**: `006-management-task-api-enhancements` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/006-management-task-api-enhancements/spec.md`

## Summary

Extend the existing `ManagementTask` API with richer query capabilities, lifecycle rules, soft deletion, idempotent creation, and consistent error handling while preserving thin controllers and clean architecture boundaries. Implementation will be strictly test-driven: each behavior slice starts with failing domain, application, API, or contract tests before production code is introduced.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core Web API, FluentValidation.AspNetCore, Application project, Domain project, Infrastructure project, LiteDB, `Microsoft.Extensions.Caching.Memory`, `Microsoft.AspNetCore.OpenApi`

**Storage**: LiteDB through the Infrastructure layer

**Testing**: xUnit, Shouldly, API unit/contract-style tests, Application unit tests, Domain unit tests, Infrastructure unit tests

**Target Platform**: .NET backend web API

**Project Type**: Clean Architecture backend web service

**Performance Goals**: Filtered and summary task reads should remain responsive for normal local-development datasets while avoiding unnecessary repeated repository reads through in-memory caching.

**Constraints**: TDD is mandatory; controllers remain translation-only; business rules stay in Domain/Application; repositories stay in Infrastructure; only DTOs cross the HTTP boundary; invalid requests return HTTP 400 with a standard error payload including `traceId`; cache invalidation must occur after every write; solution design should stay simple and avoid speculative abstractions.

**Scale/Scope**: Extends one existing API resource with additional read endpoints, PATCH support, status-only updates, soft delete behavior, idempotent create handling, cache-aware application services, and synchronized documentation/test coverage across API, Application, Domain, and Infrastructure layers.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Domain and application behavior stays isolated from frameworks, transport, and persistence.
- Business rules, validation, and use-case decisions live outside controllers, handlers, and repositories.
- Required automated tests are identified for every changed business behavior and boundary crossing, and each slice begins with failing tests before implementation.
- Simpler designs were preferred; any added abstraction has an explicit justification.
- Dependency injection, API contract consistency, and documentation updates are accounted for.

**Gate Status**: Pass. The planned design keeps HTTP translation in the API layer, task rules in Domain/Application, persistence in Infrastructure, and treats TDD as a delivery requirement rather than a follow-up activity.

## Project Structure

### Documentation (this feature)

```text
specs/006-management-task-api-enhancements/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- management-tasks.openapi.yaml
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
|   `-- Services/
|-- TaskManagementSystem.Domain/
|   |-- Entities/
|   |-- Enums/
|   `-- Exceptions/
`-- TaskManagementSystem.Infrastructure/
    |-- Database/
    `-- Repositories/

tests/
|-- TaskManagementSystem.Api.UnitTests/
|   |-- Controllers/
|   |-- ExceptionHandling/
|   `-- Validation/
|-- TaskManagementSystem.Application.UnitTests/
|-- TaskManagementSystem.Domain.UnitTests/
`-- TaskManagementSystem.Infrastructure.UnitTests/

openapi/
`-- openapi.yaml
```

**Structure Decision**: Preserve the existing four-project backend structure. Extend Domain for task state and editability rules, Application for filtered queries, idempotent create orchestration, and cache invalidation, Infrastructure for LiteDB query/idempotency persistence, and API for DTOs, validators, and route translation. Keep the canonical public contract in `openapi/openapi.yaml` and mirror the planned surface in this feature's `contracts/management-tasks.openapi.yaml`.

## Phase Plan

### Phase 0

- Confirm lifecycle rule defaults for the existing statuses: `Pending -> InProgress -> Completed`, with no edits or deletes once completed.
- Confirm the simplest HTTP surface for combined filters, overdue/due-within views, PATCH, status-only updates, and summary retrieval.
- Confirm the idempotency strategy and duplicate-key conflict behavior.
- Confirm the read-cache strategy for filtered queries and summary endpoints with simple invalidation after writes.
- Confirm the TDD sequence so every story is implemented red-green-refactor across the affected layers.

### Phase 1

- Design the enhanced task data model, query request shapes, partial update shape, summary response, archive metadata, and idempotency record.
- Design controller routes, DTO contracts, validation rules, deterministic status codes, and standard error responses.
- Design the Domain/Application/Infrastructure responsibilities for filtering, transition validation, archive behavior, idempotent create handling, and cache invalidation.
- Design the OpenAPI contract updates and validation quickstart for end-to-end feature verification.
- Update the agent context to point to this plan.

### Phase 2

- Implement in strict TDD order:
  1. Domain tests for status transitions, edit restrictions, archive behavior, and defaults.
  2. Application tests for filtered retrieval, summary generation, idempotent create handling, and cache invalidation.
  3. Infrastructure tests for LiteDB query behavior, archive persistence, and idempotency record storage.
  4. API tests for routing, DTO validation, deterministic status codes, error shapes, and OpenAPI alignment.
- Keep refactors within each slice after tests pass and before starting the next behavior.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |
