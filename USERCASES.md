# Implemented User Cases

The `specs/` folder is the source of truth for detailed functional requirements, acceptance scenarios, design notes, and task breakdowns. This file is only a lightweight index of user cases that are currently implemented in code.

| Title | Short description | Primary actor | Expected outcome | Spec reference(s) |
| --- | --- | --- | --- | --- |
| Sign up for a management account | A public client can create a management user through the API and the Angular sign-up screen. | Management user | A new user record is created and can be used for future login. | `specs/007-management-user-auth/spec.md`, `specs/009-management-ui/spec.md` |
| Log in and start an authenticated session | A public client can authenticate and receive a JWT-backed session used for protected requests. | Management user | The user receives an authenticated session and can access protected task routes. | `specs/007-management-user-auth/spec.md`, `specs/009-management-ui/spec.md` |
| Block anonymous access to task workflows | Protected API endpoints and the Angular `/tasks` route reject unauthenticated access. | Management user | Unauthenticated requests are denied and the UI redirects to login. | `specs/007-management-user-auth/spec.md`, `specs/009-management-ui/spec.md` |
| List management users through the API | The backend exposes a public list endpoint for management users. | Tester or API consumer | The caller receives the current set of management users as DTOs. | `specs/009-management-ui/spec.md`, `specs/007-management-user-auth/spec.md` |
| Query management tasks | An authenticated caller can retrieve paged task results with filtering support for status, user, search text, and archived inclusion. | API consumer | The caller receives a paged task result set. | `specs/005-api-project/spec.md`, `specs/006-management-task-api-enhancements/spec.md` |
| View the authenticated user's task workspace | The Angular tasks page loads the signed-in user's tasks and allows filtering within that view. | Management user | The user sees a protected task list scoped to the current authenticated user. | `specs/009-management-ui/spec.md` |
| Create a management task | Authenticated clients can create a task through the API, and the UI supports create from the tasks page. | Management user | A new task is saved with backend validation and default lifecycle rules applied. | `specs/005-api-project/spec.md`, `specs/006-management-task-api-enhancements/spec.md`, `specs/009-management-ui/spec.md` |
| Update a management task | Authenticated clients can fully update an existing task, and the UI supports edit through the task dialog. | Management user | The task is updated unless lifecycle or validation rules reject the change. | `specs/005-api-project/spec.md`, `specs/006-management-task-api-enhancements/spec.md`, `specs/009-management-ui/spec.md` |
| Partially patch a management task through the API | The backend exposes a PATCH endpoint for partial task updates. | API consumer | Mutable task fields can be updated without sending a full replacement payload. | `specs/006-management-task-api-enhancements/spec.md` |
| Change task status | Authenticated clients can update only the status of a task through a dedicated endpoint, and the UI exposes status changes. | Management user | The task status changes when the requested transition is allowed. | `specs/006-management-task-api-enhancements/spec.md`, `specs/009-management-ui/spec.md` |
| Archive a task | Deleting a task through the API archives it instead of removing it permanently, and the UI uses this behavior. | Management user | Eligible tasks are archived and excluded from active-only views. | `specs/006-management-task-api-enhancements/spec.md`, `specs/009-management-ui/spec.md` |
| Retrieve overdue and near-due work through the API | The backend exposes time-based task queries for overdue and due-within retrieval. | API consumer | The caller receives tasks that match the requested time window. | `specs/006-management-task-api-enhancements/spec.md` |
| Retrieve task status counts through the API | The backend exposes a task summary endpoint grouped by status. | API consumer | The caller receives per-status counts as DTOs. | `specs/006-management-task-api-enhancements/spec.md` |

## Not Implemented in the UI

- A users screen is not currently implemented in Angular.
- The Angular UI does not currently expose the backend partial patch, overdue, due-within, or summary endpoints directly.
