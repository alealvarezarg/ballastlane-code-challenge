# Quickstart: Management UI

## Purpose

Validate the Angular UI feature end to end against the existing backend API once implementation begins.

## Prerequisites

- The backend API builds and runs locally from the existing `api/` solution.
- The UI workspace has been scaffolded under `ui/` with Angular 22.0.1 and Angular Material.
- A backend configuration exists that allows local sign-up and login.
- At least one management user can be created through the public sign-up flow.

## Setup

1. Start the backend API from the repository root or `api/` workspace using the project’s normal local run command.
2. Start the Angular UI from `ui/` using the Angular CLI development server.
3. Open the UI in a browser and confirm the public entry route loads.

## Validation Scenarios

### Scenario 1: Public auth flow

1. Open the application while unauthenticated.
2. Confirm the app routes to a public screen such as `/login`.
3. Navigate to sign-up and create a management user with valid input.
4. Confirm success feedback appears and the next step in the auth flow is clear.
5. Submit invalid login input and confirm inline validation feedback appears.
6. Submit valid login credentials and confirm navigation to `/tasks`.

**Expected outcome**:

- Only public auth pages are accessible before login.
- Client-side validation responds immediately.
- Successful login stores an authenticated session and unlocks protected routes.

### Scenario 2: Protected navigation and auth recovery

1. Attempt to open `/tasks` directly while signed out.
2. Confirm the guard redirects to the public auth flow.
3. Sign in successfully and revisit `/tasks`.
4. Simulate an expired or invalid token during a protected request.
5. Confirm the app handles the authorization failure consistently and returns the user to a valid recovery path.

**Expected outcome**:

- Protected routes are never visible without a valid session.
- Authorization failures do not leave the UI in a broken or ambiguous state.

### Scenario 3: Task retrieval and query options

1. Load the tasks page after authentication.
2. Confirm the initial paged task list loads successfully.
3. Apply search, status, user, sort, pagination, and include-archived criteria.
4. Load the summary view and any overdue or due-within views represented in the final UI.
5. Confirm empty states and loading states appear appropriately when filters return no data or while requests are in flight.

**Expected outcome**:

- Query controls map to the existing backend behavior only.
- The visible result set reflects the active query state exactly.
- Summary and time-based results remain consistent with API responses.

### Scenario 4: Task mutations

1. Create a task with valid input and confirm success feedback plus refreshed task data.
2. Attempt task creation with invalid input and confirm field-level feedback is shown.
3. Edit an existing task through the supported full-update or patch flow.
4. Change the status of a task through the dedicated status action.
5. Archive a task and confirm it disappears from active-only results.
6. Attempt an action that the backend rejects with a conflict and confirm the UI surfaces the failure cleanly.

**Expected outcome**:

- Successful mutations update the visible UI state quickly and correctly.
- Validation, conflict, and not-found failures are understandable and consistent.
- Archive behavior matches backend semantics and does not appear as permanent deletion.

## Test Commands

1. Run backend unit tests as needed from `api/tests/`.
2. Run UI unit tests from `ui/` through the Angular CLI test command.
3. Run focused test suites for auth, tasks, guards, interceptors, and state slices while implementing.

## Reference Artifacts

- Feature spec: [spec.md](./spec.md)
- Implementation plan: [plan.md](./plan.md)
- Data model: [data-model.md](./data-model.md)
- UI contract: [contracts/ui-contract.md](./contracts/ui-contract.md)
