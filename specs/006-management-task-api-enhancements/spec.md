# Feature Specification: ManagementTask API Enhancements

**Feature Branch**: `006-management-task-api-enhancements`

**Created**: 2026-06-13

**Status**: Completed

**Input**: User description: "Define the specification for the ManagementTask API enhancements. Extend the existing API to support filtering by status and user, combined filters, pagination, sorting, text search, status-only updates, overdue and due-within retrieval, deletion rules, status transition validation, default Pending status on creation, PATCH support, filtered retrieval by user and status, a status summary endpoint, idempotent creation, soft delete through archiving, restricted-field protection for non-editable statuses, and active-only retrieval. Invalid requests must return HTTP 400 with a consistent error response, known exceptions must be handled globally with traceId values, endpoints must return deterministic status codes, controllers must remain thin, only DTOs may be exposed, all request DTOs must use FluentValidation, route and payload identifiers must be consistent, read operations should use in-memory caching with invalidation after modifications, clean architecture boundaries must be preserved, repositories must stay in Infrastructure, and the solution should remain simple and readable."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Find Relevant Tasks Quickly (Priority: P1)

As an API consumer, I want to retrieve management tasks using filters, search, sorting, pagination, and time-based views so I can find the tasks that matter without pulling the full dataset.

**Why this priority**: Retrieval behavior is the most frequently used part of the API and unlocks immediate value for task dashboards, lists, and reporting.

**Independent Test**: Call the task retrieval endpoints with different combinations of query options and confirm the response contains only the expected active tasks, in the requested order, with the requested page boundaries and summary information.

**Acceptance Scenarios**:

1. **Given** active tasks exist with different users, statuses, titles, due dates, and descriptions, **When** a client requests tasks with status, user, text search, sorting, pagination, and active-only criteria together, **Then** the API returns only tasks that satisfy every supplied filter and orders and paginates them deterministically.
2. **Given** tasks include overdue and upcoming due dates, **When** a client requests overdue tasks or tasks due within a specified number of days, **Then** the API returns only tasks matching that time window.
3. **Given** tasks exist across multiple statuses, **When** a client requests the task summary, **Then** the API returns counts grouped by status for the same active task set used by standard retrieval unless the request explicitly asks otherwise.

---

### User Story 2 - Manage Task Lifecycle Safely (Priority: P1)

As an API consumer, I want task creation and modification rules enforced consistently so task state changes remain valid and archived or locked tasks are protected from incorrect updates.

**Why this priority**: Lifecycle rules protect data integrity and make the enhanced API safe to use in real workflows.

**Independent Test**: Create, partially update, update status, and delete tasks through the API and confirm defaults, transition rules, field restrictions, archiving behavior, and idempotency all behave consistently.

**Acceptance Scenarios**:

1. **Given** a create request omits status, **When** the task is created successfully, **Then** the task is stored and returned with status `Pending`.
2. **Given** a client repeats the same create request with the same idempotency key, **When** the earlier request already succeeded, **Then** the API does not create a duplicate task and returns the same logical creation outcome deterministically.
3. **Given** an existing task, **When** a client sends a dedicated status update request, **Then** the API applies the change only if the requested transition is allowed and rejects disallowed transitions with a client error.
4. **Given** a task is in a non-editable status, **When** a client attempts to modify restricted fields through full or partial update, **Then** the API rejects the change and leaves the task unchanged.
5. **Given** a task is in a status that does not permit deletion, **When** a client requests deletion, **Then** the API rejects the request with a deterministic client error.
6. **Given** a task is eligible for deletion, **When** a client requests deletion, **Then** the task is archived instead of being permanently removed and is excluded from active-only retrieval results.

---

### User Story 3 - Integrate With Predictable Contracts (Priority: P2)

As a client application developer, I want every endpoint to return validated DTOs, deterministic status codes, and consistent error payloads so I can integrate with the API without reverse-engineering business or infrastructure behavior.

**Why this priority**: Predictable contracts reduce integration effort and preserve the architecture boundary between HTTP delivery and business logic.

**Independent Test**: Exercise successful, invalid, conflicting, and not-found requests across the enhanced endpoints and confirm response shapes, status codes, validation behavior, and trace identifiers remain consistent.

**Acceptance Scenarios**:

1. **Given** a request body fails validation or conflicts with route identifiers, **When** the API processes the request, **Then** it returns HTTP 400 with the standard error response including a `traceId`.
2. **Given** a known exception occurs during processing, **When** the API maps the failure to an HTTP response, **Then** the response uses the standard error format, includes a `traceId`, and uses the documented status code for that failure type.
3. **Given** a task endpoint returns successful data, **When** the response body is inspected, **Then** it contains only API DTOs and never exposes domain entities directly.

### Edge Cases

- A request combines filters that produce no matches.
- A pagination request asks for a page beyond the available result set.
- A sorting request names an unsupported field or direction.
- A due-within request uses a non-positive number of days.
- A PATCH request omits all mutable fields.
- A route identifier and payload identifier refer to different tasks.
- A repeated create request reuses an idempotency key with different payload data.
- A request tries to modify or retrieve an archived task through an active-only flow.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST allow clients to retrieve management tasks filtered by status.
- **FR-002**: The system MUST allow clients to retrieve management tasks filtered by user.
- **FR-003**: The system MUST support combining multiple retrieval filters in a single request, including user and status together.
- **FR-004**: The system MUST support text search over management task title and description.
- **FR-005**: The system MUST support sorting task retrieval results by title, status, and due date in ascending or descending order.
- **FR-006**: The system MUST support paginated task retrieval using page and page size inputs.
- **FR-007**: The system MUST support retrieving overdue management tasks.
- **FR-008**: The system MUST support retrieving management tasks due within a client-specified number of days.
- **FR-009**: The system MUST provide a task summary response that returns counts grouped by status.
- **FR-010**: The system MUST support retrieving only active, non-archived management tasks.
- **FR-011**: The system MUST default a new management task to status `Pending` when creation input omits status.
- **FR-012**: The system MUST support partial updates to management tasks.
- **FR-013**: The system MUST provide a dedicated capability to update only the status of a management task.
- **FR-014**: The system MUST validate allowed status transitions before applying a status change.
- **FR-015**: The system MUST prevent modification of restricted fields when a management task is in a non-editable status.
- **FR-016**: The system MUST reject deletion requests when the current task status does not permit deletion.
- **FR-017**: The system MUST archive a management task instead of permanently deleting it when deletion is allowed.
- **FR-018**: The system MUST prevent duplicate task creation by honoring an idempotency mechanism for create requests.
- **FR-019**: The system MUST return deterministic HTTP status codes for successful, invalid, conflicting, forbidden, not-found, and unexpected outcomes across all management task endpoints.
- **FR-020**: The system MUST return HTTP 400 with a consistent error response for invalid requests.
- **FR-021**: The system MUST include a `traceId` in every error response.
- **FR-022**: The system MUST handle known exceptions through global exception handling and map them to consistent HTTP responses.
- **FR-023**: The system MUST validate all request DTOs through FluentValidation before business processing occurs.
- **FR-024**: The system MUST validate that route identifiers and payload identifiers are consistent whenever both are supplied.
- **FR-025**: The system MUST keep controllers thin and free of business logic.
- **FR-026**: The system MUST return DTOs only and MUST NOT expose domain entities through the API.
- **FR-027**: The system MUST use in-memory caching for eligible read operations and MUST invalidate affected cached results after task creation, update, status change, patch, archive, or other modification.
- **FR-028**: The system MUST preserve clean architecture boundaries between Domain, Application, Infrastructure, and API layers.
- **FR-029**: The system MUST keep repository implementations isolated within the Infrastructure layer.
- **FR-030**: The system MUST keep the enhanced API behavior simple, readable, and free of unnecessary abstractions.

## Constitution Alignment *(mandatory)*

- **Boundary Impact**: The feature expands API retrieval and lifecycle behaviors while keeping filtering rules, status-transition rules, deletion eligibility, idempotency decisions, and edit restrictions outside controllers and repositories.
- **Testing Impact**: Coverage is required for combined filters, pagination, sorting, search, overdue and due-within retrieval, summary results, default status assignment, PATCH behavior, status-only updates, transition validation, deletion blocking, archiving, idempotency, cache invalidation, validation failures, and global exception mapping.
- **Contract Impact**: The public API gains richer read query options, status-only and summary capabilities, partial update support, active-versus-archived behavior, and a standard error response with trace identifiers.
- **Documentation Impact**: Delivery requires updated endpoint behavior documentation, error-response expectations, assumptions about active versus archived records, and usage examples for filtering, pagination, and lifecycle operations.

### Key Entities *(include if feature involves data)*

- **ManagementTask Query Request**: The client-supplied retrieval criteria used to filter, search, sort, paginate, and scope task results.
- **ManagementTask Lifecycle Update**: A change request that can create, patch, change status, or archive a task while enforcing allowed transitions and edit restrictions.
- **ManagementTask Summary**: A grouped count view that reports how many tasks currently exist in each status.
- **Standard Error Response**: The consistent error payload returned for validation, rule, not-found, and unexpected failures, always including a trace identifier.
- **Idempotent Create Request**: A creation attempt tied to a unique client-supplied identifier so repeated submissions do not create duplicate tasks.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can retrieve a narrowed task list with any supported combination of filters, search, sorting, and pagination in a single request without receiving unrelated tasks.
- **SC-002**: 100% of invalid create, update, patch, status-change, and delete requests return the documented error shape with a trace identifier.
- **SC-003**: 100% of repeated create requests using the same idempotency key and equivalent payload avoid creating duplicate tasks.
- **SC-004**: Archived tasks never appear in active-only retrieval results, and allowed delete operations always preserve the archived record for later reference.
- **SC-005**: Clients can determine overdue workload, near-term due work, and per-status counts without downloading the full task set and calculating those results themselves.

## Assumptions

- Existing management task identities, users, and statuses already exist and remain the source of truth for the enhancement.
- Active-only retrieval is the default behavior for standard list and summary experiences unless a request explicitly asks for archived data.
- Known exception types needed for invalid transitions, invalid deletion attempts, duplicate-idempotency conflicts, and missing tasks either already exist or will be introduced within the existing architecture boundaries.
- The idempotency mechanism uses a client-supplied identifier that is required only for create operations that need duplicate protection.
- Non-editable statuses and deletion-allowed statuses are defined by business rules outside the controller layer.
