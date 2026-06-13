# Tasks: Domain Model

**Input**: Design documents from `/specs/002-domain-model/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, quickstart.md

**Tests**: Include domain unit tests because the feature specification and constitution require automated coverage for changed business behavior.

**Organization**: Tasks are grouped by user story to keep each increment independently implementable and testable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the Domain project structure for the new model without leaving the feature scope

- [X] T001 Create domain folders for entities, enums, and exceptions in `src/TaskManagementSystem.Domain/Entities`, `src/TaskManagementSystem.Domain/Enums`, and `src/TaskManagementSystem.Domain/Exceptions`
- [X] T002 [P] Create test folders for domain entity coverage in `tests/TaskManagementSystem.Domain.UnitTests/Entities`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define shared domain primitives needed by all stories before entity-specific work begins

- [X] T003 Create the task lifecycle enum in `src/TaskManagementSystem.Domain/Enums/ManagementTaskStatus.cs`
- [X] T004 Create the base domain exception types in `src/TaskManagementSystem.Domain/Exceptions/DomainException.cs` and `src/TaskManagementSystem.Domain/Exceptions/DomainValidationException.cs`
- [X] T005 [P] Create entity-specific validation exceptions in `src/TaskManagementSystem.Domain/Exceptions/InvalidManagementTaskException.cs` and `src/TaskManagementSystem.Domain/Exceptions/InvalidManagementUserException.cs`

**Checkpoint**: Shared domain types and validation error structure are ready for entity implementation

---

## Phase 3: User Story 1 - Preserve Task Integrity (Priority: P1) MVP

**Goal**: Ensure task data is always created, updated, and transitioned through explicit valid domain behavior

**Independent Test**: Create and update tasks through the domain model, verify invalid values are rejected, and confirm only allowed status values can be applied

### Tests for User Story 1

- [X] T006 [P] [US1] Add task creation and invariant tests in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskTests.cs`
- [X] T007 [P] [US1] Add task update and status change tests in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskTests.cs`

### Implementation for User Story 1

- [X] T008 [US1] Implement the `ManagementTask` entity with encapsulated properties and constructor validation in `src/TaskManagementSystem.Domain/Entities/ManagementTask.cs`
- [X] T009 [US1] Implement task information update behavior with invariant enforcement in `src/TaskManagementSystem.Domain/Entities/ManagementTask.cs`
- [X] T010 [US1] Implement task status change behavior with allowed enum validation in `src/TaskManagementSystem.Domain/Entities/ManagementTask.cs`

**Checkpoint**: Task rules are fully enforced inside the Domain layer and covered by unit tests

---

## Phase 4: User Story 2 - Preserve User Integrity (Priority: P2)

**Goal**: Ensure user data is created in a valid state so task ownership can rely on consistent domain records

**Independent Test**: Create users through the domain model and verify required values are enforced with domain validation errors

### Tests for User Story 2

- [X] T011 [P] [US2] Add user creation and validation tests in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementUserTests.cs`

### Implementation for User Story 2

- [X] T012 [US2] Implement the `ManagementUser` entity with encapsulated properties and constructor validation in `src/TaskManagementSystem.Domain/Entities/ManagementUser.cs`

**Checkpoint**: User rules are enforced inside the Domain layer and covered by unit tests

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup across the Domain feature

- [X] T013 Verify placeholder source files do not conflict with the domain model in `src/TaskManagementSystem.Domain`
- [X] T014 [P] Verify placeholder test files do not conflict with entity tests in `tests/TaskManagementSystem.Domain.UnitTests`
- [X] T015 Run domain feature validation with `dotnet test tests/TaskManagementSystem.Domain.UnitTests/TaskManagementSystem.Domain.UnitTests.csproj`
- [X] T016 [P] Update planning artifacts if final implementation details differ from the current design in `specs/002-domain-model/data-model.md`, `specs/002-domain-model/quickstart.md`, and `specs/002-domain-model/research.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user story work
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion and can proceed after User Story 1 if the team wants strict MVP-first delivery
- **Polish (Phase 5)**: Depends on completion of the implemented user stories

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and delivers the MVP domain behavior for tasks
- **User Story 2 (P2)**: Starts after Foundational and complements task ownership with valid user data rules

### Within Each User Story

- Write tests before implementation
- Implement entity invariants before cleanup and final validation
- Keep all file changes inside the Domain project and Domain unit tests

### Parallel Opportunities

- T001 and T002 can run in parallel
- T004 and T005 can run in parallel after T003
- T006 and T007 can run in parallel before task implementation
- T011 can run in parallel with late task work once foundational pieces exist
- T013 and T014 can run in parallel during polish

---

## Parallel Example: User Story 1

```text
Task: "Add task creation and invariant tests in tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskTests.cs"
Task: "Add task update and status change tests in tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskTests.cs"
```

## Parallel Example: Foundational

```text
Task: "Create the base domain exception types in src/TaskManagementSystem.Domain/Exceptions/DomainException.cs and src/TaskManagementSystem.Domain/Exceptions/DomainValidationException.cs"
Task: "Create entity-specific validation exceptions in src/TaskManagementSystem.Domain/Exceptions/InvalidManagementTaskException.cs and src/TaskManagementSystem.Domain/Exceptions/InvalidManagementUserException.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Setup
2. Complete Foundational work
3. Complete User Story 1
4. Validate the Domain unit tests

### Incremental Delivery

1. Add shared enum and exception types
2. Add task behavior with tests
3. Add user behavior with tests
4. Clean placeholders and validate the Domain project

### Parallel Team Strategy

1. One contributor prepares shared exceptions and enum
2. One contributor drives task tests and task entity behavior
3. One contributor drives user tests and user entity behavior after foundational work

---

## Notes

- All tasks follow the required checklist format with IDs and exact file paths
- MVP scope is User Story 1 because task integrity is the core behavior
- The task list intentionally excludes repositories, services, DTOs, handlers, API models, persistence code, and dependency injection
