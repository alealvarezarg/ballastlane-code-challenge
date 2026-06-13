# Tasks: Initial Project Scaffolding

**Input**: Design documents from `/specs/001-project-scaffolding/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, quickstart.md

**Tests**: Include setup for the required unit-test stack and validate the scaffold with restore,
build, and test commands.

**Organization**: Tasks are grouped by user story to keep the scaffold independently deliverable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish repo-wide defaults for the scaffold

- [X] T001 Create root solution and shared build configuration in `TaskManagementSystem.slnx`, `global.json`, and `Directory.Build.props`
- [X] T002 [P] Update `.gitignore` to cover .NET build outputs, IDE artifacts, and local tool caches in `.gitignore`
- [X] T003 [P] Point agent guidance to the active scaffold plan in `AGENTS.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Define the base project layout and dependency rules before story work

- [X] T004 Create backend project folders under `src/TaskManagementSystem.Domain`, `src/TaskManagementSystem.Application`, `src/TaskManagementSystem.Infrastructure`, and `src/TaskManagementSystem.Api`
- [X] T005 [P] Create concrete project files for the four backend layers in `src/TaskManagementSystem.Domain/TaskManagementSystem.Domain.csproj`, `src/TaskManagementSystem.Application/TaskManagementSystem.Application.csproj`, `src/TaskManagementSystem.Infrastructure/TaskManagementSystem.Infrastructure.csproj`, and `src/TaskManagementSystem.Api/TaskManagementSystem.Api.csproj`
- [X] T006 Define inward-only project references across backend layers in `src/TaskManagementSystem.Application/TaskManagementSystem.Application.csproj`, `src/TaskManagementSystem.Infrastructure/TaskManagementSystem.Infrastructure.csproj`, and `src/TaskManagementSystem.Api/TaskManagementSystem.Api.csproj`
- [X] T007 Create unit test project files in `tests/TaskManagementSystem.Domain.UnitTests/TaskManagementSystem.Domain.UnitTests.csproj` and `tests/TaskManagementSystem.Application.UnitTests/TaskManagementSystem.Application.UnitTests.csproj`
- [X] T008 Add xUnit, FakeItEasy, and Shouldly package references plus project references in `tests/TaskManagementSystem.Domain.UnitTests/TaskManagementSystem.Domain.UnitTests.csproj` and `tests/TaskManagementSystem.Application.UnitTests/TaskManagementSystem.Application.UnitTests.csproj`
- [X] T009 Remove generated placeholder source and test files from `src/` and `tests/` while preserving the intended structure

**Checkpoint**: Foundation ready - the scaffold can now be completed as one independently reviewable increment

---

## Phase 3: User Story 1 - Establish Project Skeleton (Priority: P1) MVP

**Goal**: Deliver a clear Clean Architecture solution skeleton with consistent naming and ready-to-use test projects

**Independent Test**: Review the solution structure and project references, then run the root restore/build/test commands successfully

### Implementation for User Story 1

- [X] T010 [US1] Add all backend and test projects to `TaskManagementSystem.slnx`
- [X] T011 [P] [US1] Create minimal startup and configuration files in `src/TaskManagementSystem.Api/Program.cs`, `src/TaskManagementSystem.Api/appsettings.json`, and `src/TaskManagementSystem.Api/appsettings.Development.json`
- [X] T012 [P] [US1] Create minimal layer marker or placeholder types in `src/TaskManagementSystem.Domain`, `src/TaskManagementSystem.Application`, and `src/TaskManagementSystem.Infrastructure` to keep each project buildable
- [X] T013 [P] [US1] Create minimal test bootstrap files in `tests/TaskManagementSystem.Domain.UnitTests` and `tests/TaskManagementSystem.Application.UnitTests`
- [X] T014 [US1] Verify dependency direction and naming consistency across `src/` and `tests/`
- [X] T015 [US1] Document the final scaffold structure and contributor commands in `specs/001-project-scaffolding/quickstart.md`

**Checkpoint**: The project skeleton is fully reviewable and ready for later feature work

---

## Phase 4: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup across the scaffold

- [X] T016 Run scaffold validation commands from the repo root using `dotnet restore TaskManagementSystem.slnx`, `dotnet build TaskManagementSystem.slnx`, and `dotnet test TaskManagementSystem.slnx`
- [X] T017 [P] Clean up stray generated artifacts or mismatched folders that conflict with the final scaffold layout in `src/`, `tests/`, and the repo root
- [X] T018 [P] Update planning artifacts if the final scaffold differs from the current design in `specs/001-project-scaffolding/plan.md`, `specs/001-project-scaffolding/data-model.md`, and `specs/001-project-scaffolding/research.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS user story work
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **Polish (Phase 4)**: Depends on User Story 1 completion

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and delivers the full scaffold MVP

### Within Each User Story

- Solution and project files before validation
- Dependency wiring before final review
- Documentation updates before sign-off

### Parallel Opportunities

- T002 and T003 can run in parallel
- T005 and T007 can run in parallel once folders exist
- T011, T012, and T013 can run in parallel after the solution and project files are in place
- T017 and T018 can run in parallel during polish

---

## Parallel Example: User Story 1

```text
Task: "Create minimal startup and configuration files in src/TaskManagementSystem.Api/Program.cs, src/TaskManagementSystem.Api/appsettings.json, and src/TaskManagementSystem.Api/appsettings.Development.json"
Task: "Create minimal layer marker or placeholder types in src/TaskManagementSystem.Domain, src/TaskManagementSystem.Application, and src/TaskManagementSystem.Infrastructure to keep each project buildable"
Task: "Create minimal test bootstrap files in tests/TaskManagementSystem.Domain.UnitTests and tests/TaskManagementSystem.Application.UnitTests"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Setup
2. Complete Foundational work
3. Complete User Story 1
4. Validate the scaffold from the solution root

### Incremental Delivery

1. Lock repo-wide configuration
2. Create and wire projects
3. Add the test stack
4. Validate and document the scaffold

### Parallel Team Strategy

1. One contributor owns solution/configuration
2. One contributor owns backend project files and references
3. One contributor owns test project setup and quickstart updates

---

## Notes

- All tasks follow the required checklist format with IDs and file paths
- MVP scope is the single scaffolding story in User Story 1
- The task list intentionally avoids business logic, endpoints, and feature behavior
