# Implementation Plan: Domain Model

**Branch**: `002-domain-model` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-domain-model/spec.md`

## Summary

Add a focused Domain-only model for task management by introducing `ManagementTask` and `ManagementUser` entities,
the `ManagementTaskStatus` lifecycle enum, and domain-specific validation exceptions. Keep all behavior
encapsulated inside the Domain project and back the rules with unit tests.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: .NET base class library only for Domain behavior

**Storage**: N/A

**Testing**: xUnit + Shouldly in the Domain unit test project

**Target Platform**: .NET backend

**Project Type**: Clean Architecture backend domain library

**Performance Goals**: Domain validation and state changes should remain simple and immediate for single-entity operations

**Constraints**: No repositories, services, handlers, DTOs, API models, persistence concerns, navigation properties, or dependency injection

**Scale/Scope**: Two domain entities, one enum, and domain validation exceptions limited to the Domain project

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Domain and application behavior stays isolated from frameworks, transport, and persistence.
- Business rules, validation, and use-case decisions live outside controllers, handlers, and repositories.
- Required automated tests are identified for every changed business behavior and boundary crossing.
- Simpler designs were preferred; any added abstraction has an explicit justification.
- Dependency injection, API contract consistency, and documentation updates are accounted for.

**Gate Status**: Pass. The feature remains entirely inside the Domain layer, uses direct entity
behavior instead of extra abstraction, and requires only domain unit tests.

## Project Structure

### Documentation (this feature)

```text
specs/002-domain-model/
|-- plan.md
|-- research.md
|-- data-model.md
`-- quickstart.md
```

### Source Code (repository root)

```text
src/
`-- TaskManagementSystem.Domain/
    |-- Entities/
    |-- Enums/
    `-- Exceptions/

tests/
`-- TaskManagementSystem.Domain.UnitTests/
    `-- Entities/
```

**Structure Decision**: Use the existing Clean Architecture backend layout and limit all code
changes to the Domain project plus its unit tests. No external interface contracts are needed
because this feature does not expose transport or integration boundaries.

## Phase Plan

### Phase 0

- Confirm the domain stays persistence-agnostic and no external contracts are required.
- Define the minimum validation approach for required text fields, due date, and allowed status values.

### Phase 1

- Model the `ManagementTask` and `ManagementUser` entities with encapsulated state and explicit behavior methods.
- Define the `ManagementTaskStatus` enum and domain validation exception types.
- Document entity rules, invariants, and validation scenarios.

### Phase 2

- Add domain unit tests for valid creation, invalid creation, updates, and status changes.
- Validate the Domain project builds and the domain unit tests pass.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |
