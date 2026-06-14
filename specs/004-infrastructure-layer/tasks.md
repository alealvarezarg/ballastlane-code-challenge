# Tasks: Infrastructure Layer

**Input**: Design documents from `/specs/004-infrastructure-layer/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, quickstart.md

**Tests**: Include Infrastructure tests because the feature specification and constitution require automated coverage for repository CRUD behavior and dependency registration.

**Organization**: Tasks are grouped by user story to keep each increment independently implementable and testable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the Infrastructure project structure and dependencies for LiteDB persistence

- [X] T001 Create Infrastructure folders for database and repositories in `src/TaskManagementSystem.Infrastructure/Database` and `src/TaskManagementSystem.Infrastructure/Repositories`
- [X] T002 [P] Create Infrastructure test folders in `tests/TaskManagementSystem.Infrastructure.UnitTests/Database`, `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories`, and `tests/TaskManagementSystem.Infrastructure.UnitTests/DependencyInjection`
- [X] T003 [P] Add LiteDB and dependency registration package references in `src/TaskManagementSystem.Infrastructure/TaskManagementSystem.Infrastructure.csproj`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define the shared Infrastructure database access and registration entry point before repository implementation

- [X] T004 Create LiteDB database access classes in `src/TaskManagementSystem.Infrastructure/Database`
- [X] T005 Create `DependencyInjection.cs` with `AddInfrastructureDependencies` extension scaffolding in `src/TaskManagementSystem.Infrastructure/DependencyInjection.cs`

**Checkpoint**: Infrastructure database access and registration entry point are ready for repository work

---

## Phase 3: User Story 1 - Persist Tasks Through Infrastructure (Priority: P1) MVP

**Goal**: Provide a LiteDB-backed `ManagementTaskRepository` that fulfills the Application repository contract

**Independent Test**: Run tests to confirm `ManagementTaskRepository` implements all CRUD operations required by `IManagementTaskRepository` using LiteDB collections

### Tests for User Story 1

- [X] T006 [P] [US1] Add CRUD persistence tests for `ManagementTaskRepository` in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryTests.cs`
- [X] T007 [P] [US1] Add missing-document behavior tests for `ManagementTaskRepository` in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryTests.cs`

### Implementation for User Story 1

- [X] T008 [US1] Implement `ManagementTaskRepository` with LiteDB collection CRUD behavior in `src/TaskManagementSystem.Infrastructure/Repositories/ManagementTaskRepository.cs`
- [X] T009 [US1] Map `ManagementTask` document persistence through the LiteDB database access classes in `src/TaskManagementSystem.Infrastructure/Database`

**Checkpoint**: Task persistence is fully implemented through the Infrastructure repository and verified against the Application contract

---

## Phase 4: User Story 2 - Configure Infrastructure Persistence and Registration (Priority: P2)

**Goal**: Complete LiteDB connection wiring and Infrastructure dependency registration without adding business logic

**Independent Test**: Run tests to confirm LiteDB database access is exposed through `Database`, `AddInfrastructureDependencies` registers the database and repository, and the layer remains persistence-only

### Tests for User Story 2

- [X] T010 [P] [US2] Add LiteDB database access tests in `tests/TaskManagementSystem.Infrastructure.UnitTests/Database`
- [X] T011 [P] [US2] Add dependency registration tests for `AddInfrastructureDependencies` in `tests/TaskManagementSystem.Infrastructure.UnitTests/DependencyInjection/DependencyInjectionTests.cs`

### Implementation for User Story 2

- [X] T012 [US2] Complete LiteDB database configuration and exposure classes in `src/TaskManagementSystem.Infrastructure/Database`
- [X] T013 [US2] Register LiteDB database access and `ManagementTaskRepository` in `src/TaskManagementSystem.Infrastructure/DependencyInjection.cs`

**Checkpoint**: Infrastructure database wiring and dependency registration are complete and verified

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup across the Infrastructure feature

- [X] T014 Verify placeholder source files do not conflict with the Infrastructure design in `src/TaskManagementSystem.Infrastructure`
- [X] T015 [P] Verify placeholder test files do not conflict with Infrastructure tests in `tests/TaskManagementSystem.Infrastructure.UnitTests`
- [X] T016 Run Infrastructure feature validation with `dotnet test tests/TaskManagementSystem.Infrastructure.UnitTests/TaskManagementSystem.Infrastructure.UnitTests.csproj`
- [X] T017 [P] Update planning artifacts if final implementation details differ from the current design in `specs/004-infrastructure-layer/data-model.md`, `specs/004-infrastructure-layer/quickstart.md`, and `specs/004-infrastructure-layer/research.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user story work
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on User Story 1 because registration and wiring complete the repository path introduced there
- **Polish (Phase 5)**: Depends on completion of the implemented user stories

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and delivers the MVP persistence behavior for `ManagementTask`
- **User Story 2 (P2)**: Starts after User Story 1 and completes database exposure and dependency registration

### Within Each User Story

- Write tests before implementation
- Implement persistence behavior before registration cleanup and final validation
- Keep Infrastructure limited to persistence concerns and contract fulfillment

### Parallel Opportunities

- T001, T002, and T003 can run in parallel
- T006 and T007 can run in parallel within User Story 1
- T010 and T011 can run in parallel within User Story 2
- T014 and T015 can run in parallel during polish

---

## Parallel Example: User Story 1

```text
Task: "Add CRUD persistence tests for ManagementTaskRepository in tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryTests.cs"
Task: "Add missing-document behavior tests for ManagementTaskRepository in tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "Add LiteDB database access tests in tests/TaskManagementSystem.Infrastructure.UnitTests/Database"
Task: "Add dependency registration tests for AddInfrastructureDependencies in tests/TaskManagementSystem.Infrastructure.UnitTests/DependencyInjection/DependencyInjectionTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Setup
2. Complete Foundational work
3. Complete User Story 1
4. Validate Infrastructure repository tests

### Incremental Delivery

1. Add LiteDB project structure and packages
2. Add database access scaffolding
3. Add repository CRUD behavior
4. Add dependency registration and final validation

### Parallel Team Strategy

1. One contributor prepares project structure and LiteDB setup
2. One contributor drives repository CRUD tests and implementation
3. One contributor drives registration and database access tests after repository behavior exists

---

## Notes

- All tasks follow the required checklist format with IDs and exact file paths
- MVP scope is User Story 1 because repository persistence unlocks the Infrastructure layer first
- The task list intentionally excludes business logic and API delivery concerns
