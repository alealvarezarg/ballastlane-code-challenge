# Tasks: API Project

**Input**: Design documents from `/specs/005-api-project/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Include API-level and validation-focused automated tests because this feature crosses the HTTP boundary and the constitution requires test-backed delivery.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the API project structure, packages, and contract location for the feature

- [X] T001 Create API feature folders in `src/TaskManagementSystem.Api/Controllers`, `src/TaskManagementSystem.Api/Models`, `src/TaskManagementSystem.Api/Validation`, and `src/TaskManagementSystem.Api/ExceptionHandling`
- [X] T002 [P] Create API test folders in `tests/TaskManagementSystem.Api.UnitTests/Controllers`, `tests/TaskManagementSystem.Api.UnitTests/Validation`, and `tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling`
- [X] T003 [P] Add FluentValidation and API testing package references in `src/TaskManagementSystem.Api/TaskManagementSystem.Api.csproj` and `tests/TaskManagementSystem.Api.UnitTests/TaskManagementSystem.Api.UnitTests.csproj`
- [X] T004 [P] Create the canonical OpenAPI contract location in `openapi/openapi.yaml`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared API contracts, mapping, validation registration, and exception infrastructure before story work

- [X] T005 Create shared API DTOs and mapping helpers in `src/TaskManagementSystem.Api/Models/CreateManagementTaskRequestDto.cs`, `src/TaskManagementSystem.Api/Models/UpdateManagementTaskRequestDto.cs`, `src/TaskManagementSystem.Api/Models/ManagementTaskResponseDto.cs`, and `src/TaskManagementSystem.Api/Models/ManagementTaskMappings.cs`
- [X] T006 [P] Create error response models in `src/TaskManagementSystem.Api/Models/ErrorResponseDto.cs` and supporting types in `src/TaskManagementSystem.Api/Models/ValidationErrorDto.cs`
- [X] T007 [P] Implement global exception handling infrastructure in `src/TaskManagementSystem.Api/ExceptionHandling/GlobalExceptionHandler.cs`
- [X] T008 [P] Register FluentValidation, Application, Infrastructure, controller services, and exception handling in `src/TaskManagementSystem.Api/Program.cs`
- [X] T009 Mirror the planned HTTP surface in `openapi/openapi.yaml` and `specs/005-api-project/contracts/management-tasks.openapi.yaml`
- [X] T010 Remove placeholder API files that conflict with the feature design in `src/TaskManagementSystem.Api/WeatherForecast.cs` and `src/TaskManagementSystem.Api/Controllers/WeatherForecastController.cs`

**Checkpoint**: API foundation is ready for story implementation

---

## Phase 3: User Story 1 - Manage Tasks Through HTTP (Priority: P1) MVP

**Goal**: Expose CRUD endpoints for management tasks through a thin controller using DTOs only

**Independent Test**: Call the `management-tasks` endpoints and confirm create, read, update, and delete operations work using API request and response DTOs without exposing domain entities.

### Tests for User Story 1

- [X] T011 [P] [US1] Add controller CRUD success-path tests in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs`
- [X] T012 [P] [US1] Add DTO mapping tests in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs`

### Implementation for User Story 1

- [X] T013 [P] [US1] Implement `ManagementTaskController` read endpoints in `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs`
- [X] T014 [P] [US1] Implement `ManagementTaskController` write endpoints in `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs`
- [X] T015 [US1] Complete DTO-to-domain and domain-to-DTO mapping behavior in `src/TaskManagementSystem.Api/Models/ManagementTaskMappings.cs`
- [X] T016 [US1] Update `openapi/openapi.yaml` and `specs/005-api-project/contracts/management-tasks.openapi.yaml` to match the implemented CRUD request and response payloads

**Checkpoint**: User Story 1 is fully functional and testable through HTTP-oriented controller behavior

---

## Phase 4: User Story 2 - Receive Consistent API Models and Errors (Priority: P2)

**Goal**: Return predictable DTO validation results and consistent error payloads for invalid and failing requests

**Independent Test**: Submit invalid and failing requests and confirm the API returns DTO-based error responses with appropriate status codes and no leaked domain internals.

### Tests for User Story 2

- [X] T017 [P] [US2] Add FluentValidation tests for create and update DTOs in `tests/TaskManagementSystem.Api.UnitTests/Validation/CreateManagementTaskRequestDtoValidatorTests.cs` and `tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskRequestDtoValidatorTests.cs`
- [X] T018 [P] [US2] Add global exception handling tests for validation, not-found, and unexpected failures in `tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs`
- [X] T019 [P] [US2] Add controller error-response tests for invalid input and missing tasks in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerErrorTests.cs`

### Implementation for User Story 2

- [X] T020 [P] [US2] Implement `CreateManagementTaskRequestDtoValidator` in `src/TaskManagementSystem.Api/Validation/CreateManagementTaskRequestDtoValidator.cs`
- [X] T021 [P] [US2] Implement `UpdateManagementTaskRequestDtoValidator` in `src/TaskManagementSystem.Api/Validation/UpdateManagementTaskRequestDtoValidator.cs`
- [X] T022 [US2] Complete consistent exception-to-error-response translation in `src/TaskManagementSystem.Api/ExceptionHandling/GlobalExceptionHandler.cs`
- [X] T023 [US2] Update `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs` to enforce route/body identifier alignment and consistent error-oriented action results
- [X] T024 [US2] Update `openapi/openapi.yaml` and `specs/005-api-project/contracts/management-tasks.openapi.yaml` to reflect validation and error response behavior

**Checkpoint**: User Story 2 returns consistent DTO and error contracts independently of User Story 3

---

## Phase 5: User Story 3 - Discover and Wire the API Easily (Priority: P3)

**Goal**: Make the API easy to start, wire, and explore through dependency registration and OpenAPI exposure

**Independent Test**: Start the API configuration, verify required services are registered, and confirm the OpenAPI surface and root YAML contract describe the same endpoints.

### Tests for User Story 3

- [X] T025 [P] [US3] Add dependency registration tests for API startup in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs`
- [X] T026 [P] [US3] Add OpenAPI contract alignment tests in `tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs`

### Implementation for User Story 3

- [X] T027 [US3] Finalize API startup wiring and OpenAPI exposure in `src/TaskManagementSystem.Api/Program.cs`
- [X] T028 [US3] Update API configuration guidance in `src/TaskManagementSystem.Api/appsettings.json` and `src/TaskManagementSystem.Api/appsettings.Development.json` if required for local validation
- [X] T029 [US3] Finalize the canonical OpenAPI document in `openapi/openapi.yaml` and keep the mirrored planning contract in `specs/005-api-project/contracts/management-tasks.openapi.yaml` synchronized

**Checkpoint**: User Story 3 provides a discoverable and correctly wired API surface

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and feature-level cleanup across all stories

- [X] T030 [P] Verify placeholder test files do not conflict with API feature tests in `tests/TaskManagementSystem.Api.UnitTests/Class1.cs`
- [X] T031 [P] Update feature validation guidance if implementation details differ from the design in `specs/005-api-project/quickstart.md`, `specs/005-api-project/data-model.md`, and `specs/005-api-project/research.md`
- [X] T032 Run API feature validation for `tests/TaskManagementSystem.Api.UnitTests/TaskManagementSystem.Api.UnitTests.csproj`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on User Story 1 because error behavior and validation complete the CRUD surface introduced there
- **User Story 3 (Phase 5)**: Depends on User Story 2 because startup and contract alignment should reflect the finalized endpoint and error behavior
- **Polish (Phase 6)**: Depends on completion of the implemented user stories

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and delivers the MVP CRUD API surface
- **User Story 2 (P2)**: Starts after User Story 1 and completes validation and consistent error handling
- **User Story 3 (P3)**: Starts after User Story 2 and completes startup wiring and external contract discoverability

### Within Each User Story

- Required tests MUST be written before implementation
- DTOs and validators before controller behavior that depends on them
- Controller behavior before OpenAPI synchronization sign-off
- Startup/documentation work after endpoint and error behavior are settled

### Parallel Opportunities

- T002, T003, and T004 can run in parallel
- T006, T007, and T008 can run in parallel
- T011 and T012 can run in parallel within User Story 1
- T017, T018, and T019 can run in parallel within User Story 2
- T020 and T021 can run in parallel within User Story 2
- T025 and T026 can run in parallel within User Story 3
- T030 and T031 can run in parallel during polish

---

## Parallel Example: User Story 1

```text
Task: "Add controller CRUD success-path tests in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerTests.cs"
Task: "Add DTO mapping tests in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "Add FluentValidation tests for create and update DTOs in tests/TaskManagementSystem.Api.UnitTests/Validation/CreateManagementTaskRequestDtoValidatorTests.cs and tests/TaskManagementSystem.Api.UnitTests/Validation/UpdateManagementTaskRequestDtoValidatorTests.cs"
Task: "Add global exception handling tests for validation, not-found, and unexpected failures in tests/TaskManagementSystem.Api.UnitTests/ExceptionHandling/GlobalExceptionHandlerTests.cs"
Task: "Add controller error-response tests for invalid input and missing tasks in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerErrorTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "Add dependency registration tests for API startup in tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs"
Task: "Add OpenAPI contract alignment tests in tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Stop and validate the CRUD API surface independently before layering on error handling and discoverability refinements

### Incremental Delivery

1. Deliver the CRUD controller and DTO surface
2. Add validation and consistent error handling
3. Finalize startup wiring and OpenAPI synchronization
4. Finish with cleanup and full feature validation

### Parallel Team Strategy

1. One contributor prepares DTOs, mapping, and project wiring
2. One contributor drives controller and API behavior tests
3. One contributor drives validation, exception handling, and contract synchronization after the foundation is ready

---

## Notes

- All tasks follow the required checklist format with task IDs, story labels where required, and exact file paths
- `openapi/openapi.yaml` is the canonical API contract and must stay synchronized with implementation
- FluentValidation should be applied with one validator per DTO that requires inbound validation
- Suggested MVP scope is User Story 1
