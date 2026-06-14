# Tasks: Application Layer

**Input**: Design documents from `/specs/003-application-layer/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, quickstart.md

**Tests**: Include Application unit tests because the feature specification and constitution require automated coverage for orchestration and caching behavior.

**Organization**: Tasks are grouped by user story to keep each increment independently implementable and testable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the Application project structure for contracts, services, and tests

- [X] T001 Create Application folders for abstractions and services in `src/TaskManagementSystem.Application/Abstractions` and `src/TaskManagementSystem.Application/Services`
- [X] T002 [P] Create Application unit test folders in `tests/TaskManagementSystem.Application.UnitTests/Services` and `tests/TaskManagementSystem.Application.UnitTests/DependencyInjection`
- [X] T003 [P] Add Application package references required for caching and dependency registration in `src/TaskManagementSystem.Application/TaskManagementSystem.Application.csproj`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define the shared Application contracts and registration entry point before story work begins

- [X] T004 Create `IManagementTaskRepository` CRUD contract in `src/TaskManagementSystem.Application/Abstractions/IManagementTaskRepository.cs`
- [X] T005 [P] Create `IManagementTaskService` contract in `src/TaskManagementSystem.Application/Abstractions/IManagementTaskService.cs`
- [X] T006 Create `DependencyInjection.cs` with `AddApplicationDependencies` extension scaffolding in `src/TaskManagementSystem.Application/DependencyInjection.cs`

**Checkpoint**: Application contracts and registration entry point are ready for service implementation

---

## Phase 3: User Story 1 - Manage Tasks Through Application Contracts (Priority: P1) MVP

**Goal**: Provide Application-layer contracts and service orchestration for task CRUD operations without leaking Infrastructure concerns

**Independent Test**: Review and run tests to confirm `IManagementTaskService` exposes task operations and `ManagementTaskService` implements them while depending only on `IManagementTaskRepository`

### Tests for User Story 1

- [X] T007 [P] [US1] Add CRUD orchestration tests for `ManagementTaskService` in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs`

### Implementation for User Story 1

- [X] T008 [US1] Implement `ManagementTaskService` with repository-based CRUD orchestration in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [X] T009 [US1] Register `IManagementTaskService` and `ManagementTaskService` in `src/TaskManagementSystem.Application/DependencyInjection.cs`

**Checkpoint**: Application-layer CRUD orchestration is available through the service contract and stays independent from Infrastructure and API

---

## Phase 4: User Story 2 - Improve Read Performance and Registration (Priority: P2)

**Goal**: Add cache-aware reads, cache invalidation after writes, and complete Application dependency registration

**Independent Test**: Run tests to confirm `GetById` and `GetAll` use cache-aware behavior, write operations invalidate stale cache entries, and `AddApplicationDependencies` registers the Application-layer collaborators

### Tests for User Story 2

- [X] T010 [P] [US2] Add cache hit, cache miss, and invalidation tests for `ManagementTaskService` in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs`
- [X] T011 [P] [US2] Add dependency registration tests for `AddApplicationDependencies` in `tests/TaskManagementSystem.Application.UnitTests/DependencyInjection/DependencyInjectionTests.cs`

### Implementation for User Story 2

- [X] T012 [US2] Add `IMemoryCache`-backed `GetById` and `GetAll` behavior to `ManagementTaskService` in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [X] T013 [US2] Add cache invalidation for create, update, and delete operations in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [X] T014 [US2] Complete Application-layer dependency registrations for caching and service wiring in `src/TaskManagementSystem.Application/DependencyInjection.cs`

**Checkpoint**: Read performance and Application registration behavior are complete and verified

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup across the Application feature

- [X] T015 Verify placeholder source files do not conflict with the Application design in `src/TaskManagementSystem.Application`
- [X] T016 [P] Verify placeholder test files do not conflict with Application unit tests in `tests/TaskManagementSystem.Application.UnitTests`
- [X] T017 Run Application feature validation with `dotnet test tests/TaskManagementSystem.Application.UnitTests/TaskManagementSystem.Application.UnitTests.csproj`
- [X] T018 [P] Update planning artifacts if final implementation details differ from the current design in `specs/003-application-layer/data-model.md`, `specs/003-application-layer/quickstart.md`, and `specs/003-application-layer/research.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user story work
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on User Story 1 because caching and registration extend the service defined there
- **Polish (Phase 5)**: Depends on completion of the implemented user stories

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and delivers the MVP Application contracts and service orchestration
- **User Story 2 (P2)**: Starts after User Story 1 and adds cache-backed reads plus complete registration behavior

### Within Each User Story

- Write tests before implementation
- Implement service behavior before registration cleanup and final validation
- Keep dependencies limited to Domain, Application abstractions, and accepted Application collaborators

### Parallel Opportunities

- T001, T002, and T003 can run in parallel
- T004 and T005 can run in parallel after setup
- T010 and T011 can run in parallel during User Story 2
- T015 and T016 can run in parallel during polish

---

## Parallel Example: User Story 2

```text
Task: "Add cache hit, cache miss, and invalidation tests for ManagementTaskService in tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs"
Task: "Add dependency registration tests for AddApplicationDependencies in tests/TaskManagementSystem.Application.UnitTests/DependencyInjection/DependencyInjectionTests.cs"
```

## Parallel Example: Foundational

```text
Task: "Create IManagementTaskRepository CRUD contract in src/TaskManagementSystem.Application/Abstractions/IManagementTaskRepository.cs"
Task: "Create IManagementTaskService contract in src/TaskManagementSystem.Application/Abstractions/IManagementTaskService.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Setup
2. Complete Foundational work
3. Complete User Story 1
4. Validate the Application unit tests for service orchestration

### Incremental Delivery

1. Add contracts and registration scaffolding
2. Add Application service CRUD orchestration
3. Add cache-backed reads and invalidation
4. Validate Application tests and registration behavior

### Parallel Team Strategy

1. One contributor prepares project structure and contract files
2. One contributor drives service orchestration tests and implementation
3. One contributor drives cache and registration tests after the service exists

---

## Notes

- All tasks follow the required checklist format with IDs and exact file paths
- MVP scope is User Story 1 because Application contracts and service orchestration unlock outer layers first
- The task list intentionally excludes Infrastructure implementations and API delivery concerns
