# UI Contract: Management UI

## Purpose

Define the planned frontend route surface, API integrations, shared request handling rules, and feature boundaries for the Angular UI. This contract documents how the UI will consume the existing backend without extending it.

## Route Contract

### Public Routes

| Route | Purpose | Access | Notes |
|------|---------|--------|-------|
| `/login` | Authenticate an existing management user | Public | Primary public entry point |
| `/sign-up` | Create a new management user | Public | Secondary public entry point |

### Protected Routes

| Route | Purpose | Access | Notes |
|------|---------|--------|-------|
| `/tasks` | Management-task workspace | Authenticated | Default post-login landing page |
| `/tasks/:id` | Task-focused edit/detail workflow if needed | Authenticated | Only if route-driven editing proves clearer than dialog-only editing |

### Deferred Routes

| Route | Purpose | Access | Notes |
|------|---------|--------|-------|
| `/users` | Test-only management-user list | Deferred | Not implemented until backend exposes list retrieval |

## API Integration Contract

### Authentication

| UI Capability | Method | Endpoint | Request DTO | Response DTO |
|--------------|--------|----------|-------------|--------------|
| Sign up | `POST` | `/management-users` | `CreateManagementUserRequestDto` | `ManagementUserResponseDto` |
| Login | `POST` | `/management-users/login` | `LoginManagementUserRequestDto` | `ManagementUserLoginResponseDto` |

### Tasks

| UI Capability | Method | Endpoint | Request DTO | Response DTO |
|--------------|--------|----------|-------------|--------------|
| List tasks | `GET` | `/management-tasks` | Query DTO parameters | `PagedManagementTaskResponseDto` |
| Get task by id | `GET` | `/management-tasks/{id}` | Path id | `ManagementTaskResponseDto` |
| Create task | `POST` | `/management-tasks` | `CreateManagementTaskRequestDto` | `ManagementTaskResponseDto` |
| Update task | `PUT` | `/management-tasks/{id}` | `UpdateManagementTaskRequestDto` | `ManagementTaskResponseDto` |
| Patch task | `PATCH` | `/management-tasks/{id}` | `PatchManagementTaskRequestDto` | `ManagementTaskResponseDto` |
| Change task status | `PATCH` | `/management-tasks/{id}/status` | `UpdateManagementTaskStatusRequestDto` | `ManagementTaskResponseDto` |
| Archive task | `DELETE` | `/management-tasks/{id}` | Path id | `204 No Content` |
| Get overdue tasks | `GET` | `/management-tasks/overdue` | `includeArchived` query parameter | `ManagementTaskResponseDto[]` |
| Get tasks due within window | `GET` | `/management-tasks/due-within` | `days`, `includeArchived` query parameters | `ManagementTaskResponseDto[]` |
| Get status summary | `GET` | `/management-tasks/summary` | `includeArchived` query parameter | `ManagementTaskSummaryResponseDto` |

## HTTP Behavior Contract

### Auth Header Rules

- Protected requests must include the bearer token from the current authenticated session.
- Public authentication requests must not require an auth header.
- An authorization failure must clear or invalidate client auth state when the session is no longer usable.

### Error Handling Rules

- The UI must normalize API errors from the shared `ErrorResponseDto` shape.
- Field-level validation errors must be mapped back to forms when the response includes `errors`.
- Conflict, not-found, and unexpected errors must surface through the same feedback system but may present feature-specific wording.
- Trace identifiers should remain available for diagnostics without overwhelming the normal user flow.

### Validation Rules Mirrored Client-Side

| Form/Query | Mirrored Rules |
|-----------|----------------|
| Sign up | username required; email required and valid; password required with minimum length 8 |
| Login | email required and valid; password required |
| Task create | title required; description required; optional status must be valid; due date must be valid; user id required |
| Task update | id required; title required; description required; status valid; due date valid; user id required |
| Task patch | at least one mutable field required; provided title/description cannot be empty; provided due date must be valid; provided user id must be non-empty |
| Task status update | status must be valid |
| Task query | status valid when supplied; sortBy in `title/status/dueDate`; sortDirection in `asc/desc`; page > 0; pageSize between 1 and 200 |

## State Contract

### Auth State

- Current session
- Authenticated user
- Login request status
- Sign-up request status
- Route-guard readiness

### Tasks State

- Active query
- Paged list result
- Summary result
- Overdue collection
- Due-within collection
- Mutation request states for create, update, patch, status change, and archive

### Shared UI State

- Global success and error notifications
- Reusable loading patterns
- Page title and navigation context

## Deferred Scope Contract

- No permanent-delete workflow beyond archive
- No offline mode
- No client-owned business validation beyond mirrored API rules
- No management-user list screen until a list endpoint exists
