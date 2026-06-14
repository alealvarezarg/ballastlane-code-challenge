# Tasks: ManagementTask API Enhancements

**Input**: Design documents from `/specs/006-management-task-api-enhancements/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Required. This feature is explicitly TDD-driven and the constitution requires automated
tests for every changed business behavior and HTTP boundary.

**Organization**: Tasks are grouped by user story so each story can be implemented and validated
independently where dependencies allow.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (`[US1]`, `[US2]`, `[US3]`)
- Every task includes the primary file path to change

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Align shared design artifacts and test entry points before implementation starts

- [X] T001 Align the planned enhancement contract in `specs/006-management-task-api-enhancements/contracts/management-tasks.openapi.yaml`
- [X] T002 Sync the canonical public contract scaffold in `openapi/openapi.yaml`
- [X] T003 Capture the TDD execution flow and validation scenarios in `specs/006-management-task-api-enhancements/quickstart.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared primitives that block all stories

**CRITICAL**: No user story work should begin until this phase is complete

- [X] T004 [P] Add shared enhancement domain and application test entry points in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskEnhancementTests.cs`
- [X] T005 [P] Add shared enhancement application and infrastructure test entry points in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceEnhancementTests.cs`
- [X] T006 [P] Add shared enhancement API test entry points in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskEnhancementControllerTests.cs`
- [X] T007 Extend shared application contracts for query, patch, status, archive, summary, and idempotency operations in `src/TaskManagementSystem.Application/Abstractions/IManagementTaskService.cs`
- [X] T008 Extend shared repository contracts for query, patch, status, archive, summary, and idempotency persistence in `src/TaskManagementSystem.Application/Abstractions/IManagementTaskRepository.cs`
- [X] T009 Add shared application models for filtering, paging, summaries, and idempotency in `src/TaskManagementSystem.Application/Models/ManagementTaskQueryOptions.cs`
- [X] T010 Extend persistence documents for archive metadata and idempotency storage in `src/TaskManagementSystem.Infrastructure/Repositories/ManagementTaskDocument.cs`
- [X] T011 Extend API task response mapping for archive-aware payloads in `src/TaskManagementSystem.Api/Models/ManagementTaskResponseDto.cs`

**Checkpoint**: Shared contracts, persistence shape, and test anchors are ready for story work

---

## Phase 3: User Story 1 - Find Relevant Tasks Quickly (Priority: P1) MVP

**Goal**: Deliver filtered, searchable, sortable, paginated, overdue, due-within, and summary read capabilities for active tasks

**Independent Test**: Call `GET /management-tasks`, `GET /management-tasks/overdue`, `GET /management-tasks/due-within`, and `GET /management-tasks/summary` with combined query options and confirm only the expected active tasks and grouped counts are returned deterministically.

### Tests for User Story 1

> **NOTE**: Write these tests first, ensure they fail before implementation, and keep the cycle
> red-green-refactor for each slice.

- [X] T012 [P] [US1] Add failing domain tests for archive-aware read eligibility in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskEnhancementTests.cs`
- [X] T013 [P] [US1] Add failing application tests for combined filters, search, sorting, pagination, overdue, due-within, summary, and cache reuse in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceEnhancementTests.cs`
- [X] T014 [P] [US1] Add failing infrastructure tests for filtered, overdue, due-within, and summary LiteDB queries in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryEnhancementTests.cs`
- [X] T015 [P] [US1] Add failing API endpoint and contract tests for retrieval and summary routes in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskEnhancementControllerTests.cs`

### Implementation for User Story 1

- [X] T016 [P] [US1] Add retrieval query and summary DTOs in `src/TaskManagementSystem.Api/Models/ManagementTaskQueryRequestDto.cs`
- [X] T017 [P] [US1] Add retrieval query validators in `src/TaskManagementSystem.Api/Validation/ManagementTaskQueryRequestDtoValidator.cs`
- [X] T018 [US1] Implement archive-aware read rules needed by retrieval flows in `src/TaskManagementSystem.Domain/Entities/ManagementTask.cs`
- [X] T019 [US1] Implement filtered, overdue, due-within, and summary queries in `src/TaskManagementSystem.Infrastructure/Repositories/ManagementTaskRepository.cs`
- [X] T020 [US1] Implement cache-backed retrieval and summary orchestration in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [X] T021 [US1] Implement list, overdue, due-within, and summary endpoints plus mappings in `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs`
- [X] T022 [US1] Finalize retrieval OpenAPI schemas and examples in `openapi/openapi.yaml`

**Checkpoint**: User Story 1 should now be fully functional and independently testable as the MVP

---

## Phase 4: User Story 2 - Manage Task Lifecycle Safely (Priority: P1)

**Goal**: Deliver safe create, patch, status-only update, idempotent create, and archive behavior with enforced lifecycle rules

**Independent Test**: Create tasks without status, repeat creates with the same idempotency key, patch mutable fields, change status through the dedicated endpoint, and archive eligible tasks while confirming invalid transitions, restricted edits, and forbidden deletes are rejected deterministically.

### Tests for User Story 2

- [X] T023 [P] [US2] Add failing domain tests for default Pending status, allowed transitions, restricted edits, and archive rules in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskEnhancementTests.cs`
- [X] T024 [P] [US2] Add failing application tests for idempotent create, patch, status changes, archive operations, and cache invalidation in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceEnhancementTests.cs`
- [X] T025 [P] [US2] Add failing infrastructure tests for archive persistence and idempotency key reuse handling in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryEnhancementTests.cs`
- [X] T026 [P] [US2] Add failing API tests for create defaulting, PATCH, status-only updates, delete blocking, and archive flows in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskLifecycleControllerTests.cs`

### Implementation for User Story 2

- [X] T027 [US2] Implement lifecycle defaults, transition validation, edit restrictions, and archive behavior in `src/TaskManagementSystem.Domain/Entities/ManagementTask.cs`
- [X] T028 [P] [US2] Add lifecycle and idempotency request models in `src/TaskManagementSystem.Api/Models/PatchManagementTaskRequestDto.cs`
- [X] T029 [P] [US2] Add lifecycle and idempotency validators in `src/TaskManagementSystem.Api/Validation/PatchManagementTaskRequestDtoValidator.cs`
- [X] T030 [US2] Implement idempotent create, patch, status-only update, archive orchestration, and write-cache invalidation in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [X] T031 [US2] Implement archive and idempotency persistence in `src/TaskManagementSystem.Infrastructure/Repositories/ManagementTaskRepository.cs`
- [X] T032 [US2] Implement create defaulting, PATCH, status-only, and archive endpoints plus mappings in `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs`
- [X] T033 [US2] Finalize lifecycle OpenAPI schemas and response codes in `specs/006-management-task-api-enhancements/contracts/management-tasks.openapi.yaml`

**Checkpoint**: User Story 2 should now be independently functional on top of the shared foundation

---

## Phase 5: User Story 3 - Integrate With Predictable Contracts (Priority: P2)

**Goal**: Ensure deterministic status codes, consistent error payloads with `traceId`, thin controllers, DTO-only responses, and correct dependency wiring for the enhanced surface

**Independent Test**: Exercise successful, invalid, conflict, not-found, and unexpected-failure paths across the enhanced endpoints and confirm all responses use DTOs, deterministic codes, and the standard error format with `traceId`.

### Tests for User Story 3

- [X] T034 [P] [US3] Add failing exception-handling tests for 400, 404, 409, and 500 responses with `traceId` in `tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerEnhancementTests.cs`
- [X] T035 [P] [US3] Add failing API validation and identifier-consistency tests in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskValidationControllerTests.cs`
- [X] T036 [P] [US3] Add failing service-registration and OpenAPI alignment tests for the enhanced routes in `tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiEnhancementContractTests.cs`

### Implementation for User Story 3

- [X] T037 [P] [US3] Add enhancement-specific domain/application exceptions for conflict and forbidden outcomes in `src/TaskManagementSystem.Domain/Exceptions/ManagementTaskLifecycleException.cs`
- [X] T038 [US3] Extend global exception mapping for deterministic enhanced error responses in `src/TaskManagementSystem.Api/ExceptionHandling/GlobalExceptionHandler.cs`
- [X] T039 [US3] Enforce route-body consistency and validation coverage across enhancement requests in `src/TaskManagementSystem.Api/Validation/UpdateManagementTaskRequestDtoValidator.cs`
- [X] T040 [US3] Update controller response metadata and DTO-only mappings for enhanced endpoints in `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs`
- [X] T041 [US3] Update enhanced dependency registration and API pipeline behavior in `src/TaskManagementSystem.Api/ApiServiceCollectionExtensions.cs`
- [X] T042 [US3] Finalize enhanced public contract alignment in `openapi/openapi.yaml`

**Checkpoint**: All enhanced endpoints should now expose predictable contracts and consistent error behavior

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, cleanup, and documentation across stories

- [X] T043 [P] Update end-to-end validation notes and story checkpoints in `specs/006-management-task-api-enhancements/quickstart.md`
- [X] T044 [P] Reconcile any final contract wording and examples in `specs/006-management-task-api-enhancements/contracts/management-tasks.openapi.yaml`
- [ ] T045 Run the full validation flow from `specs/006-management-task-api-enhancements/quickstart.md` and record any follow-up notes in `specs/006-management-task-api-enhancements/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all story work
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion
- **User Story 3 (Phase 5)**: Depends on User Story 1 and User Story 2 because it validates the final HTTP contract across the enhanced surface
- **Polish (Phase 6)**: Depends on all selected stories being complete

### User Story Dependencies

- **US1**: Independent after Foundational and is the recommended MVP slice
- **US2**: Independent after Foundational, but shares the extended contracts and persistence primitives from Phase 2
- **US3**: Depends on the endpoint surface delivered by US1 and US2

### Within Each User Story

- Tests must be written first and must fail before implementation begins
- Domain and application rules should be implemented before controller translation
- Repository changes must be in place before service orchestration can go green
- OpenAPI and documentation updates must be completed before story sign-off

### Parallel Opportunities

- **Setup**: T001-T003 can be split across docs/contract files
- **Foundational**: T004-T006 can run in parallel because they seed different test projects
- **US1**: T012-T015 can run in parallel as initial failing tests; T016-T017 can run in parallel as DTO/validator work
- **US2**: T023-T026 can run in parallel as initial failing tests; T028-T029 can run in parallel as DTO/validator work
- **US3**: T034-T036 can run in parallel as initial failing tests; T037 and T041 can run in parallel because they touch different files

---

## Parallel Example: User Story 1

```text
Task: "Add failing domain tests for archive-aware read eligibility in tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskEnhancementTests.cs"
Task: "Add failing application tests for combined filters, search, sorting, pagination, overdue, due-within, summary, and cache reuse in tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceEnhancementTests.cs"
Task: "Add failing infrastructure tests for filtered, overdue, due-within, and summary LiteDB queries in tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryEnhancementTests.cs"
Task: "Add failing API endpoint and contract tests for retrieval and summary routes in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskEnhancementControllerTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "Add failing domain tests for default Pending status, allowed transitions, restricted edits, and archive rules in tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementTaskEnhancementTests.cs"
Task: "Add failing application tests for idempotent create, patch, status changes, archive operations, and cache invalidation in tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceEnhancementTests.cs"
Task: "Add failing infrastructure tests for archive persistence and idempotency key reuse handling in tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementTaskRepositoryEnhancementTests.cs"
Task: "Add failing API tests for create defaulting, PATCH, status-only updates, delete blocking, and archive flows in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskLifecycleControllerTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "Add failing exception-handling tests for 400, 404, 409, and 500 responses with traceId in tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerEnhancementTests.cs"
Task: "Add failing API validation and identifier-consistency tests in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskValidationControllerTests.cs"
Task: "Add failing service-registration and OpenAPI alignment tests for the enhanced routes in tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiEnhancementContractTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate the retrieval endpoints independently using `specs/006-management-task-api-enhancements/quickstart.md`

### Incremental Delivery

1. Finish Setup + Foundational once
2. Deliver **US1** as the first demoable slice
3. Add **US2** for lifecycle safety and idempotent writes
4. Add **US3** to harden deterministic contracts and error responses
5. Finish with Phase 6 cross-cutting validation

### Parallel Team Strategy

1. One teammate can own the failing tests while another prepares DTO/validator files after the relevant test tasks exist
2. After Phase 2, US1 and US2 can proceed in parallel if coordination is tight around shared contracts
3. US3 should start after the enhanced endpoint surface from US1 and US2 is stable

---

## Notes

- All tasks follow the required checklist format with IDs, optional `[P]`, story labels where required, and file paths.
- TDD is mandatory for this feature: do not write production code for a story slice before its failing tests exist.
- Keep controllers thin and preserve the Domain/Application/Infrastructure/API boundaries while executing these tasks.
