# Feature Specification: ManagementUser Authentication

**Feature Branch**: `007-management-user-auth`

**Created**: 2026-06-14

**Status**: Completed

**Input**: User description: "Extend the existing solution to support ManagementUser with public create, login, and get-by-id endpoints; require authentication for all other endpoints including every ManagementTask endpoint; validate that referenced ManagementUser records exist before creating ManagementTask records; keep controllers thin and preserve the existing architecture, DTO conventions, validation pipeline, OpenAPI documentation, and TDD testing patterns."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Register and Sign In a Management User (Priority: P1)

As a client application or tester, I want to create a management user account and sign in through public endpoints so I can obtain authenticated access to the secured API surface.

**Why this priority**: Authentication is the entry point for every secured workflow, and the feature is incomplete without a supported way to create and authenticate a management user.

**Independent Test**: Create a management user through the public registration endpoint, sign in with the same credentials, and confirm the sign-in response grants access to a secured endpoint.

**Acceptance Scenarios**:

1. **Given** a valid new management user registration request, **When** the client submits it to the public create endpoint, **Then** the system stores the user and returns a DTO describing the created management user.
2. **Given** a previously created management user with valid credentials, **When** the client submits those credentials to the public login endpoint, **Then** the system authenticates the user and returns the authentication result needed to call protected endpoints.
3. **Given** invalid sign-in credentials, **When** the client submits a login request, **Then** the system rejects the request with the documented client error response and does not authenticate the user.

---

### User Story 2 - Protect Secured API Endpoints Consistently (Priority: P1)

As an API consumer, I want every non-public endpoint to require authentication so that task and other secured operations are not accessible anonymously.

**Why this priority**: The primary behavioral change in this feature is enforcing consistent protection across the existing API, especially the full ManagementTask surface.

**Independent Test**: Call protected endpoints with no authentication, invalid authentication, and valid authentication, and confirm anonymous or invalid requests are rejected while authenticated requests continue to work.

**Acceptance Scenarios**:

1. **Given** a request to any ManagementTask endpoint without valid authentication, **When** the request reaches the API, **Then** the system rejects it as unauthorized.
2. **Given** a request to a protected non-task endpoint without valid authentication, **When** the request reaches the API, **Then** the system rejects it as unauthorized.
3. **Given** a request to a protected endpoint with valid authentication, **When** the request reaches the API, **Then** the system allows normal processing and returns the endpoint's documented result.

---

### User Story 3 - Validate Management User References in Task Creation (Priority: P2)

As a client application, I want task creation to fail when it references a management user that does not exist so the system does not persist orphaned tasks.

**Why this priority**: Referential validation protects data integrity and directly affects one of the key task workflows already present in the solution.

**Independent Test**: Attempt to create a management task with a non-existing user identifier and confirm the API returns the expected business validation error and no task is saved.

**Acceptance Scenarios**:

1. **Given** an authenticated task creation request that references an existing management user, **When** the request is processed, **Then** the task is created successfully.
2. **Given** an authenticated task creation request that references a non-existing management user, **When** the request is processed, **Then** the system rejects the request with the appropriate business validation error and does not persist the task.
3. **Given** a public get-by-id request for an existing management user, **When** the request is processed, **Then** the system returns the matching management user DTO for testing purposes.

### Edge Cases

- A management user registration request reuses a unique credential value that is already assigned to another management user.
- A management user login request omits required credentials or supplies malformed values.
- A protected endpoint receives an expired, invalid, or missing authentication token.
- A public get-by-id request references a management user identifier that does not exist.
- A task creation request is authenticated successfully but references a management user identifier that does not exist in storage.
- OpenAPI documentation must clearly distinguish the three public management user endpoints from the secured endpoints.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a public endpoint that creates a management user from a validated request DTO and returns a response DTO for the created user.
- **FR-002**: The system MUST provide a public endpoint that authenticates a management user using validated credentials and returns the authentication result required for subsequent protected requests.
- **FR-003**: The system MUST provide a public endpoint that retrieves a management user by identifier for testing purposes only.
- **FR-004**: The system MUST require authentication for every endpoint that is not explicitly designated as public.
- **FR-005**: The system MUST require authentication for every ManagementTask endpoint.
- **FR-006**: The system MUST reject anonymous or invalidly authenticated requests to protected endpoints with the documented authorization error response.
- **FR-007**: The system MUST continue to allow anonymous access only to management user creation, management user login, and management user get-by-id.
- **FR-008**: The system MUST validate all new API request DTOs before business processing occurs.
- **FR-009**: The system MUST ensure all API request and response models introduced for this feature follow the existing DTO naming convention and end with the `Dto` suffix.
- **FR-010**: The system MUST keep business rules and use-case orchestration outside controllers.
- **FR-011**: The system MUST keep persistence logic for management users and task-user existence checks outside the API and application delivery layers.
- **FR-012**: The system MUST validate during management task creation that the referenced management user exists in the data store before any task is persisted.
- **FR-013**: The system MUST fail task creation with an appropriate business validation error when the referenced management user does not exist.
- **FR-014**: The system MUST leave the data store unchanged when task creation fails because the referenced management user does not exist.
- **FR-015**: The system MUST integrate authentication into the existing solution so protected endpoints are secured consistently through the same application behavior.
- **FR-016**: The system MUST continue to route validation and business rule failures through the existing global error-handling behavior so clients receive the established error format.
- **FR-017**: The system MUST update the public API documentation to include the new management user endpoints and clearly identify authentication requirements for protected endpoints.
- **FR-018**: The system MUST include automated test coverage for management user creation, management user login, management user retrieval, authorization behavior on protected endpoints, authentication requirements across all ManagementTask endpoints, and rejection of task creation for a non-existing management user.
- **FR-019**: The system MUST preserve the established test-first delivery approach when implementing this feature.
- **FR-020**: The system MUST preserve the existing architecture, conventions, and design decisions used throughout the solution when adding management user support.

## Constitution Alignment *(mandatory)*

- **Boundary Impact**: The feature adds management user registration, authentication, and retrieval capabilities while keeping HTTP translation in controllers, use-case and authorization orchestration in the application layer, business rules in the domain layer, and persistence plus user existence checks in the infrastructure layer.
- **Testing Impact**: Coverage is required for management user creation, login success and failure, get-by-id behavior, anonymous access to the three public endpoints, authorization failures on protected endpoints, authenticated access to secured endpoints, and task creation rejection when the referenced management user does not exist.
- **Contract Impact**: The public API gains management user request and response DTOs, a login result contract, a test-only public get-by-id endpoint, and documented authentication requirements across the secured API surface.
- **Documentation Impact**: Delivery requires OpenAPI updates for the new endpoints, security requirements on protected endpoints, and clear notes describing the testing-only purpose of the public management user get-by-id endpoint.

### Key Entities *(include if feature involves data)*

- **ManagementUser**: A secured application user record that contains the identifying and sign-in information required to authenticate and to be referenced by management tasks.
- **ManagementUser Registration Request**: The client-supplied information needed to create a new management user account.
- **ManagementUser Login Request**: The client-supplied credentials used to authenticate a management user.
- **Authentication Result**: The response returned after successful login that allows the client to call protected endpoints.
- **Task Ownership Reference**: The management user identifier supplied during task creation that must resolve to an existing management user before the task can be stored.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of successful management user registration requests return a created user response that can be used in a subsequent login flow.
- **SC-002**: 100% of protected endpoint requests made without valid authentication are rejected, while authenticated requests can continue to the endpoint's normal business behavior.
- **SC-003**: 100% of ManagementTask endpoints are documented and enforced as protected endpoints.
- **SC-004**: 100% of management task creation attempts that reference a non-existing management user fail without persisting a task.
- **SC-005**: A tester can create a management user, sign in, and retrieve that user by identifier through the documented public endpoints in one end-to-end flow without consulting source code.

## Assumptions

- Management user authentication will use the solution's standard credential-based sign-in approach rather than introducing a separate identity provider for this feature.
- A management user has at least one unique credential value that can be used for login, and duplicate values for that credential are not allowed.
- The existing ManagementTask create flow already carries a management user identifier that should be validated against stored management users.
- The public get-by-id endpoint is temporary support for testing and documentation verification rather than a long-term unrestricted production access pattern.
