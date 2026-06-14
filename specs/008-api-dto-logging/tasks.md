# Tasks: API DTO and Logging Improvements

**Input**: Design documents from `/specs/008-api-dto-logging/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md,
data-model.md, contracts/, quickstart.md

**Tests**: Required. This feature explicitly requires automated coverage for changed API
contracts, validation behavior, and logging outcomes, and the constitution requires tests for
changed business behavior and HTTP boundaries.

**Organization**: Tasks are grouped by user story so each story can be implemented and validated
independently where dependencies allow.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (`[US1]`, `[US2]`, `[US3]`)
- Every task includes the primary file path to change

## Path Conventions

- Backend source code lives under `src/`
- Automated tests live under `tests/`
- Feature design artifacts live under `specs/008-api-dto-logging/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare feature-level contracts, docs, and test entry points before implementation starts

- [X] T001 Align the feature task inventory in `specs/008-api-dto-logging/tasks.md`
- [X] T002 [P] Add API DTO contract test entry points in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs`
- [X] T003 [P] Add API status serialization and validation test entry points in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs` and `tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskRequestDtoValidatorTests.cs`
- [X] T004 [P] Add application logging test entry points in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs` and `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs`
- [X] T005 [P] Add exception-handling logging test entry points in `tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared contract and logging primitives that MUST be complete before ANY user story can be implemented

**CRITICAL**: No user story work should begin until this phase is complete

- [X] T006 [P] Add shared API logging package references in `src/TaskManagementSystem.Api/TaskManagementSystem.Api.csproj`
- [X] T007 [P] Add centralized Serilog console configuration in `src/TaskManagementSystem.Api/Program.cs`
- [X] T008 [P] Add shared JSON enum configuration for API request and response handling in `src/TaskManagementSystem.Api/ApiServiceCollectionExtensions.cs`
- [X] T009 [P] Update the feature-level OpenAPI contract baseline in `specs/008-api-dto-logging/contracts/api-dto-logging.openapi.yaml`
- [X] T010 [P] Update the canonical public contract baseline in `openapi/openapi.yaml`
- [X] T011 Add shared API and application dependency wiring expectations for logging and serialization in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs`

**Checkpoint**: Shared serialization, logging configuration, and contract baselines are ready for story work

---

## Phase 3: User Story 1 - Receive DTO-Only API Responses (Priority: P1) MVP

**Goal**: Ensure every endpoint returns explicit response DTOs only, with controllers returning mapped transport models instead of domain entities

**Independent Test**: Call representative ManagementTask and ManagementUser endpoints and confirm the returned payloads contain only explicit DTO fields with no direct domain entity exposure.

### Tests for User Story 1

> **NOTE**: Write these tests first, ensure they fail before implementation, and keep the cycle
> red-green-refactor for each slice.

- [X] T012 [P] [US1] Add failing API mapping tests for task response, paged response, and summary response DTOs in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs`
- [X] T013 [P] [US1] Add failing API controller tests that assert DTO-only responses for task endpoints in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs`
- [X] T014 [P] [US1] Add failing API controller tests that assert DTO-only responses for management-user endpoints in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementUserControllerTests.cs`
- [X] T015 [P] [US1] Add failing OpenAPI contract tests for explicit response DTO schemas in `tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs`

### Implementation for User Story 1

- [X] T016 [P] [US1] Update task response DTO models to remain explicit transport contracts in `src/TaskManagementSystem.Api/Models/ManagementTaskResponseDto.cs`, `src/TaskManagementSystem.Api/Models/PagedManagementTaskResponseDto.cs`, `src/TaskManagementSystem.Api/Models/ManagementTaskSummaryResponseDto.cs`, and `src/TaskManagementSystem.Api/Models/ManagementTaskStatusCountDto.cs`
- [X] T017 [P] [US1] Update task mapping helpers to return DTO-only models for every success case in `src/TaskManagementSystem.Api/Models/ManagementTaskMappings.cs`
- [X] T018 [P] [US1] Update management-user mapping helpers to keep all user success paths DTO-only in `src/TaskManagementSystem.Api/Models/ManagementUserMappings.cs`
- [X] T019 [US1] Update controller response metadata and return paths to use explicit DTO contracts only in `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs` and `src/TaskManagementSystem.Api/Controllers/ManagementUserController.cs`
- [X] T020 [US1] Finalize DTO-only response schemas in `specs/008-api-dto-logging/contracts/api-dto-logging.openapi.yaml` and `openapi/openapi.yaml`

**Checkpoint**: User Story 1 should now be fully functional and independently testable as the MVP

---

## Phase 4: User Story 2 - Exchange Readable Management Status Values (Priority: P1)

**Goal**: Accept and return readable `ManagementTaskStatus` string values consistently across bodies, query parameters, summary items, and OpenAPI

**Independent Test**: Send valid string `status` values through task-related requests and queries, confirm responses serialize string values, and confirm invalid values return the standard validation error response.

### Tests for User Story 2

- [X] T021 [P] [US2] Add failing API serialization tests for string-based task status values in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs`
- [X] T022 [P] [US2] Add failing API controller tests for string-based status request and response behavior in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs`
- [X] T023 [P] [US2] Add failing API validation tests for invalid body status values in `tests/TaskManagementSystem.Api.UnitTests/Validation/CreateManagementTaskRequestDtoValidatorTests.cs`, `tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskRequestDtoValidatorTests.cs`, and `tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskStatusRequestDtoValidatorTests.cs`
- [X] T024 [P] [US2] Add failing API validation tests for invalid query status values in `tests/TaskManagementSystem.Api.UnitTests/Validation/ManagementTaskQueryRequestDtoValidatorTests.cs`
- [X] T025 [P] [US2] Add failing OpenAPI contract tests for string enum status schemas in `tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs`

### Implementation for User Story 2

- [X] T026 [P] [US2] Update task request and query DTOs to use the centralized string-enum contract in `src/TaskManagementSystem.Api/Models/CreateManagementTaskRequestDto.cs`, `src/TaskManagementSystem.Api/Models/UpdateManagementTaskRequestDto.cs`, `src/TaskManagementSystem.Api/Models/UpdateManagementTaskStatusRequestDto.cs`, and `src/TaskManagementSystem.Api/Models/ManagementTaskQueryRequestDto.cs`
- [X] T027 [P] [US2] Update task response and summary DTOs for readable status serialization in `src/TaskManagementSystem.Api/Models/ManagementTaskResponseDto.cs` and `src/TaskManagementSystem.Api/Models/ManagementTaskStatusCountDto.cs`
- [X] T028 [P] [US2] Update request validators to preserve the existing validation error behavior for invalid status input in `src/TaskManagementSystem.Api/Validation/CreateManagementTaskRequestDtoValidator.cs`, `src/TaskManagementSystem.Api/Validation/UpdateManagementTaskRequestDtoValidator.cs`, `src/TaskManagementSystem.Api/Validation/UpdateManagementTaskStatusRequestDtoValidator.cs`, and `src/TaskManagementSystem.Api/Validation/ManagementTaskQueryRequestDtoValidator.cs`
- [X] T029 [US2] Update API serialization and query-binding configuration for consistent string enum handling in `src/TaskManagementSystem.Api/ApiServiceCollectionExtensions.cs`
- [X] T030 [US2] Finalize string-based status documentation and examples in `specs/008-api-dto-logging/contracts/api-dto-logging.openapi.yaml` and `openapi/openapi.yaml`

**Checkpoint**: User Stories 1 and 2 should both be independently functional and contract-aligned

---

## Phase 5: User Story 3 - Observe API Behavior Through Consistent Logging (Priority: P2)

**Goal**: Add centralized Serilog-backed logging through `ILogger<T>` for successful operations, expected handled warnings, and unexpected exceptions without changing public error contracts

**Independent Test**: Exercise successful operations, handled validation or not-found outcomes, and unexpected exceptions, then confirm informational, warning, and error logs are emitted at the intended boundaries and severities.

### Tests for User Story 3

- [X] T031 [P] [US3] Add failing application tests for informational success logs in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs` and `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs`
- [X] T032 [P] [US3] Add failing API exception-handling tests for warning logs on expected validation, not-found, conflict, and unauthorized outcomes in `tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs`
- [X] T033 [P] [US3] Add failing API exception-handling tests for error logs on unexpected exceptions in `tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs`
- [X] T034 [P] [US3] Add failing API startup and service-registration tests for Serilog console integration in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs`

### Implementation for User Story 3

- [X] T035 [P] [US3] Add informational operation logs to task workflows in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [X] T036 [P] [US3] Add informational operation logs to management-user workflows in `src/TaskManagementSystem.Application/Services/ManagementUserService.cs`
- [X] T037 [US3] Add warning and error logging to the global exception pipeline while preserving response payloads in `src/TaskManagementSystem.Api/ExceptionHandling/GlobalExceptionHandler.cs`
- [X] T038 [US3] Finalize centralized Serilog console setup and logging provider behavior in `src/TaskManagementSystem.Api/Program.cs` and `src/TaskManagementSystem.Api/ApiServiceCollectionExtensions.cs`
- [X] T039 [US3] Update feature quickstart validation steps for logging verification in `specs/008-api-dto-logging/quickstart.md`

**Checkpoint**: All user stories should now be independently functional with consistent operational visibility

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, cleanup, and documentation across stories

- [X] T040 [P] Reconcile final DTO, enum, and logging wording across `specs/008-api-dto-logging/plan.md`, `specs/008-api-dto-logging/research.md`, and `specs/008-api-dto-logging/data-model.md`
- [X] T041 [P] Refresh canonical API examples and schema notes in `openapi/openapi.yaml`
- [X] T042 Run the validation flow from `specs/008-api-dto-logging/quickstart.md` and record any follow-up notes in `specs/008-api-dto-logging/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all story work
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion and should follow US1 because it builds on the audited DTO contract surface
- **User Story 3 (Phase 5)**: Depends on Foundational completion and is best completed after US1 and US2 so logs cover the final contract behavior
- **Polish (Phase 6)**: Depends on all selected stories being complete

### User Story Dependencies

- **US1**: Independent after Foundational and is the recommended MVP slice
- **US2**: Depends on the shared serialization baseline from Foundational and should reuse the DTO surface stabilized in US1
- **US3**: Depends on the application services and global exception paths exercised by US1 and US2

### Within Each User Story

- Tests must be written first and must fail before implementation begins
- DTO/model and mapping updates should happen before controller or documentation sign-off
- Shared API configuration changes should be in place before status-contract and logging tests can go green
- OpenAPI and quickstart updates must be completed before story sign-off

### Parallel Opportunities

- **Setup**: T002-T005 can run in parallel because they seed separate test areas
- **Foundational**: T006-T010 can run in parallel because they touch separate startup, config, and contract files
- **US1**: T012-T015 can run in parallel as initial failing tests; T016-T018 can run in parallel as DTO/mapping work
- **US2**: T021-T025 can run in parallel as initial failing tests; T026-T028 can run in parallel as DTO/validator work
- **US3**: T031-T034 can run in parallel as initial failing tests; T035-T036 can run in parallel as service logging work

---

## Parallel Example: User Story 1

```text
Task: "Add failing API mapping tests for task response, paged response, and summary response DTOs in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs"
Task: "Add failing API controller tests that assert DTO-only responses for task endpoints in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs"
Task: "Add failing API controller tests that assert DTO-only responses for management-user endpoints in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementUserControllerTests.cs"
Task: "Add failing OpenAPI contract tests for explicit response DTO schemas in tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "Add failing API serialization tests for string-based task status values in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs"
Task: "Add failing API controller tests for string-based status request and response behavior in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs"
Task: "Add failing API validation tests for invalid body status values in tests/TaskManagementSystem.Api.UnitTests/Validation/CreateManagementTaskRequestDtoValidatorTests.cs, tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskRequestDtoValidatorTests.cs, and tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskStatusRequestDtoValidatorTests.cs"
Task: "Add failing OpenAPI contract tests for string enum status schemas in tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "Add failing application tests for informational success logs in tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs and tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs"
Task: "Add failing API exception-handling tests for warning logs on expected validation, not-found, conflict, and unauthorized outcomes in tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs"
Task: "Add failing API exception-handling tests for error logs on unexpected exceptions in tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs"
Task: "Add failing API startup and service-registration tests for Serilog console integration in tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate DTO-only response behavior independently using `specs/008-api-dto-logging/quickstart.md`

### Incremental Delivery

1. Finish Setup + Foundational once
2. Deliver **US1** for DTO-only response safety
3. Add **US2** for readable string status contracts
4. Add **US3** for centralized operational logging
5. Finish with Phase 6 cross-cutting validation

### Parallel Team Strategy

1. One teammate can prepare API tests while another updates feature contracts and docs in Setup/Foundational
2. After Phase 2, one developer can own DTO/mapping work for **US1** while another prepares status validation tests for **US2**
3. Logging work for **US3** should start after the service and exception surfaces touched by **US1** and **US2** are stable

---

## Notes

- All tasks follow the required checklist format with IDs, optional `[P]`, story labels where required, and file paths.
- TDD is mandatory for this feature: do not write production code for a story slice before its failing tests exist.
- Keep controllers thin and preserve the Domain/Application/Infrastructure/API boundaries while executing these tasks.
