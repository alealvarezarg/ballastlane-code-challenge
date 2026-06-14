# Tasks: ManagementUser Authentication

**Input**: Design documents from `/specs/007-management-user-auth/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md,
data-model.md, contracts/

**Tests**: Include the test tasks required by the constitution for any changed business behavior or
boundary-crossing integration. Do not omit required tests.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing
of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Backend source code lives under `src/`
- Automated tests live under `tests/`
- Feature design artifacts live under `specs/007-management-user-auth/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare feature-level documentation and test entry points for implementation

- [x] T001 Create the feature task inventory in `specs/007-management-user-auth/tasks.md`
- [x] T002 [P] Add feature-level API authentication test entry points in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementUserControllerTests.cs`
- [x] T003 [P] Add feature-level API authorization and contract test entry points in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementAuthorizationControllerTests.cs`
- [x] T004 [P] Add feature-level validator test entry points in `tests/TaskManagementSystem.Api.UnitTests/Validation/CreateManagementUserRequestDtoValidatorTests.cs`
- [x] T005 [P] Add feature-level application test entry points in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs`
- [x] T006 [P] Add feature-level infrastructure test entry points in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementUserRepositoryTests.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core authentication and persistence building blocks that MUST be complete before ANY user story can be implemented

**CRITICAL**: No user story work can begin until this phase is complete

- [x] T007 [P] Add shared management-user application abstractions in `src/TaskManagementSystem.Application/Abstractions/IManagementUserRepository.cs`
- [x] T008 [P] Add shared authentication abstractions in `src/TaskManagementSystem.Application/Abstractions/IPasswordHasher.cs` and `src/TaskManagementSystem.Application/Abstractions/IAccessTokenService.cs`
- [x] T009 [P] Add shared management-user application models in `src/TaskManagementSystem.Application/Models/AuthenticatedManagementUser.cs`
- [x] T010 [P] Add shared authentication configuration model in `src/TaskManagementSystem.Infrastructure/Authentication/JwtAuthenticationSettings.cs`
- [x] T011 [P] Add shared management-user persistence document in `src/TaskManagementSystem.Infrastructure/Repositories/ManagementUserDocument.cs`
- [x] T012 [P] Add shared authentication and user-specific exception types in `src/TaskManagementSystem.Domain/Exceptions/InvalidManagementUserCredentialsException.cs` and `src/TaskManagementSystem.Domain/Exceptions/DuplicateManagementUserException.cs`
- [x] T013 Add shared management-user dependency wiring in `src/TaskManagementSystem.Application/DependencyInjection.cs` and `src/TaskManagementSystem.Infrastructure/DependencyInjection.cs`
- [x] T014 Add shared API authentication configuration in `src/TaskManagementSystem.Api/ApiServiceCollectionExtensions.cs` and `src/TaskManagementSystem.Api/Program.cs`
- [x] T015 Add shared authentication configuration values in `src/TaskManagementSystem.Api/appsettings.json` and `src/TaskManagementSystem.Api/appsettings.Development.json`
- [x] T016 Update shared registration coverage for the new dependencies in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs` and `tests/TaskManagementSystem.Infrastructure.UnitTests/DependencyInjection/DependencyInjectionTests.cs`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Register and Sign In a Management User (Priority: P1) MVP

**Goal**: Let clients create a management user, sign in with credentials, and receive a safe authentication result

**Independent Test**: Create a management user through the public registration endpoint, sign in with the same credentials, and confirm the login response returns a bearer token plus safe user details.

### Tests for User Story 1

> **NOTE: Write these tests first, ensure they fail before implementation, and add only the
> coverage needed for the story's affected behaviors and boundaries.**

- [x] T017 [P] [US1] Extend management-user domain invariant tests in `tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementUserTests.cs`
- [x] T018 [P] [US1] Add failing application tests for registration and login orchestration in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs`
- [x] T019 [P] [US1] Add failing infrastructure tests for management-user persistence, uniqueness checks, and password verification in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementUserRepositoryTests.cs`
- [x] T020 [P] [US1] Add failing API controller tests for create and login endpoints in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementUserControllerTests.cs`
- [x] T021 [P] [US1] Add failing request validation tests in `tests/TaskManagementSystem.Api.UnitTests/Validation/CreateManagementUserRequestDtoValidatorTests.cs` and `tests/TaskManagementSystem.Api.UnitTests/Validation/LoginManagementUserRequestDtoValidatorTests.cs`

### Implementation for User Story 1

- [x] T022 [P] [US1] Add management-user request and response DTOs in `src/TaskManagementSystem.Api/Models/CreateManagementUserRequestDto.cs`, `src/TaskManagementSystem.Api/Models/LoginManagementUserRequestDto.cs`, `src/TaskManagementSystem.Api/Models/ManagementUserResponseDto.cs`, and `src/TaskManagementSystem.Api/Models/ManagementUserLoginResponseDto.cs`
- [x] T023 [P] [US1] Add management-user DTO validators in `src/TaskManagementSystem.Api/Validation/CreateManagementUserRequestDtoValidator.cs` and `src/TaskManagementSystem.Api/Validation/LoginManagementUserRequestDtoValidator.cs`
- [x] T024 [P] [US1] Add management-user mapping helpers in `src/TaskManagementSystem.Api/Models/ManagementUserMappings.cs`
- [x] T025 [P] [US1] Implement the management-user repository in `src/TaskManagementSystem.Infrastructure/Repositories/ManagementUserRepository.cs`
- [x] T026 [P] [US1] Implement the password hasher and token service in `src/TaskManagementSystem.Infrastructure/Authentication/AspNetPasswordHasher.cs` and `src/TaskManagementSystem.Infrastructure/Authentication/JwtAccessTokenService.cs`
- [x] T027 [US1] Add the management-user service contract and orchestration in `src/TaskManagementSystem.Application/Abstractions/IManagementUserService.cs` and `src/TaskManagementSystem.Application/Services/ManagementUserService.cs`
- [x] T028 [US1] Implement the public management-user controller endpoints in `src/TaskManagementSystem.Api/Controllers/ManagementUserController.cs`
- [x] T029 [US1] Update the global exception handler for duplicate user and invalid credential outcomes in `src/TaskManagementSystem.Api/ExceptionHandling/GlobalExceptionHandler.cs`
- [x] T030 [US1] Update the feature contract and root OpenAPI description for registration and login in `specs/007-management-user-auth/contracts/management-users-auth.openapi.yaml` and `openapi/openapi.yaml`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Protect Secured API Endpoints Consistently (Priority: P1)

**Goal**: Require authentication for all non-public endpoints, especially the full ManagementTask API surface

**Independent Test**: Call protected endpoints without authentication and with valid authentication, and confirm only authenticated requests are allowed to proceed.

### Tests for User Story 2

- [x] T031 [P] [US2] Add failing API authorization coverage for public versus protected routes in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementAuthorizationControllerTests.cs`
- [x] T032 [P] [US2] Add failing API contract coverage for authentication requirements in `tests/TaskManagementSystem.Api.UnitTests/Controllers/OpenApiContractTests.cs`
- [x] T033 [P] [US2] Add failing API service-registration coverage for authentication services in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ProgramServiceRegistrationTests.cs`

### Implementation for User Story 2

- [x] T034 [US2] Apply explicit anonymous and authorized route attributes in `src/TaskManagementSystem.Api/Controllers/ManagementUserController.cs` and `src/TaskManagementSystem.Api/Controllers/ManagementTaskController.cs`
- [x] T035 [US2] Finalize bearer authentication pipeline behavior in `src/TaskManagementSystem.Api/ApiServiceCollectionExtensions.cs` and `src/TaskManagementSystem.Api/Program.cs`
- [x] T036 [US2] Update the feature contract and root OpenAPI description for protected endpoints in `specs/007-management-user-auth/contracts/management-users-auth.openapi.yaml` and `openapi/openapi.yaml`
- [x] T037 [US2] Update the feature quickstart validation steps for auth enforcement in `specs/007-management-user-auth/quickstart.md`

**Checkpoint**: At this point, User Stories 1 and 2 should both work independently

---

## Phase 5: User Story 3 - Validate Management User References in Task Creation (Priority: P2)

**Goal**: Prevent task creation when the referenced management user does not exist, while keeping the public user get-by-id endpoint available for testing

**Independent Test**: Retrieve a management user by id through the public endpoint, then attempt task creation with a missing user id and confirm the API returns a business validation error and persists no task.

### Tests for User Story 3

- [x] T038 [P] [US3] Add failing application tests for task-create user existence validation and public user retrieval in `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs` and `tests/TaskManagementSystem.Application.UnitTests/Services/ManagementTaskServiceTests.cs`
- [x] T039 [P] [US3] Add failing infrastructure tests for management-user lookup by id in `tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementUserRepositoryTests.cs`
- [x] T040 [P] [US3] Add failing API tests for public get-by-id and non-existing task-owner rejection in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementUserControllerTests.cs` and `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskControllerErrorTests.cs`

### Implementation for User Story 3

- [x] T041 [US3] Add management-user retrieval support to the service contract and implementation in `src/TaskManagementSystem.Application/Abstractions/IManagementUserService.cs` and `src/TaskManagementSystem.Application/Services/ManagementUserService.cs`
- [x] T042 [US3] Add public management-user get-by-id endpoint behavior in `src/TaskManagementSystem.Api/Controllers/ManagementUserController.cs`
- [x] T043 [US3] Enforce task-owner existence checks before persistence in `src/TaskManagementSystem.Application/Services/ManagementTaskService.cs`
- [x] T044 [US3] Add the business validation exception for missing task owners in `src/TaskManagementSystem.Domain/Exceptions/ManagementUserNotFoundException.cs` and map it in `src/TaskManagementSystem.Api/ExceptionHandling/GlobalExceptionHandler.cs`
- [x] T045 [US3] Update root task request documentation for user-existence validation in `openapi/openapi.yaml`

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [x] T046 [P] Refresh API model and mapping coverage in `tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementTaskMappingsTests.cs`
- [x] T047 [P] Refresh feature documentation and assumptions in `specs/007-management-user-auth/plan.md`, `specs/007-management-user-auth/research.md`, and `specs/007-management-user-auth/data-model.md`
- [x] T048 Run the feature quickstart validation and record any follow-up fixes in `specs/007-management-user-auth/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - establishes management-user registration and login capabilities needed for secured validation flows
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - depends on the shared authentication pipeline but remains independently testable from US3
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) and should be completed after US1 because it reuses management-user service and persistence behavior

### Within Each User Story

- Required tests MUST be written and fail before implementation
- DTOs and domain-supporting models before service orchestration
- Service orchestration before controller endpoints
- Exception mapping and OpenAPI updates before story sign-off
- Story complete before moving to the next priority checkpoint

### Parallel Opportunities

- T002-T006 can run in parallel during Setup because they create separate test entry points
- T007-T012 can run in parallel during Foundational because they touch separate abstraction, model, and exception files
- T017-T021 can run in parallel within US1 because they add separate failing tests
- T022-T026 can run in parallel within US1 after the failing tests exist
- T031-T033 can run in parallel within US2 because they cover separate authorization surfaces
- T038-T040 can run in parallel within US3 because they target separate application, infrastructure, and API layers

---

## Parallel Example: User Story 1

```text
Task: "Extend management-user domain invariant tests in tests/TaskManagementSystem.Domain.UnitTests/Entities/ManagementUserTests.cs"
Task: "Add failing application tests for registration and login orchestration in tests/TaskManagementSystem.Application.UnitTests/Services/ManagementUserServiceTests.cs"
Task: "Add failing infrastructure tests for management-user persistence, uniqueness checks, and password verification in tests/TaskManagementSystem.Infrastructure.UnitTests/Repositories/ManagementUserRepositoryTests.cs"
Task: "Add failing API controller tests for create and login endpoints in tests/TaskManagementSystem.Api.UnitTests/Controllers/ManagementUserControllerTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test registration and login independently before broad authorization rollout

### Incremental Delivery

1. Complete Setup and Foundational work
2. Add User Story 1 and validate public registration/login
3. Add User Story 2 and validate secured endpoint protection
4. Add User Story 3 and validate task-owner existence checks plus public get-by-id
5. Finish Polish and quickstart validation

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup and Foundational work together
2. One developer implements US1 registration/login flows
3. One developer implements US2 authorization enforcement and OpenAPI security updates
4. One developer prepares US3 task-owner validation once the management-user service contract is stable

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify required tests fail before implementing
- Prefer extending existing controllers, validators, services, and repositories before introducing new structure
