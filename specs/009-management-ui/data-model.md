# Data Model: Management UI

## Overview

The UI data model mirrors the existing API DTO contracts and adds only the presentation-level state needed to render, validate, and orchestrate user interactions. No frontend model introduces business rules that are not already enforced by the API.

## Entities

### AuthenticatedSession

- **Purpose**: Represents the authenticated state required for protected routes and authorized API requests.
- **Fields**:
  - `accessToken`: string
  - `tokenType`: string
  - `expiresAt`: ISO date-time string
  - `user`: `ManagementUser`
- **Relationships**:
  - Created by successful login
  - Consumed by auth guard, auth interceptor, and app shell
- **Validation/Rules**:
  - Missing or expired session prevents protected-route access
  - Session storage is presentation infrastructure only; backend authorization remains authoritative

### ManagementUser

- **Purpose**: Represents the user data returned by sign-up and login responses.
- **Fields**:
  - `id`: GUID string
  - `username`: string
  - `email`: string
- **Relationships**:
  - Nested inside `AuthenticatedSession`
  - Referenced indirectly by tasks through `userId`

### SignUpInput

- **Purpose**: Represents the UI form state for creating a management user.
- **Fields**:
  - `username`: string
  - `email`: string
  - `password`: string
- **Validation/Rules**:
  - `username` required
  - `email` required and email-shaped
  - `password` required and minimum length 8
  - Backend validation errors must still map back to the form

### LoginInput

- **Purpose**: Represents the UI form state for authenticating a management user.
- **Fields**:
  - `email`: string
  - `password`: string
- **Validation/Rules**:
  - `email` required and email-shaped
  - `password` required

### ManagementTask

- **Purpose**: Represents task records returned by the backend.
- **Fields**:
  - `id`: GUID string
  - `title`: string
  - `description`: string
  - `status`: `Pending | InProgress | Completed`
  - `dueDate`: ISO date-time string
  - `userId`: GUID string
  - `isArchived`: boolean
  - `archivedAt`: ISO date-time string or null
- **Relationships**:
  - Displayed in task list, detail/edit state, summary widgets, and time-based views
- **State transitions**:
  - Status changes only through the backend-supported status endpoint
  - Archive changes only through the backend delete/archive endpoint

### ManagementTaskDraft

- **Purpose**: Represents task form input for create and full update flows.
- **Fields**:
  - `id`: GUID string for full update only
  - `title`: string
  - `description`: string
  - `status`: `Pending | InProgress | Completed` or null for create
  - `dueDate`: ISO date-time string or date input representation
  - `userId`: GUID string
- **Validation/Rules**:
  - `title` required
  - `description` required
  - `status` must be a valid enum value when present
  - `dueDate` must be a valid non-default date
  - `userId` required and non-empty
  - Full update requires `id`

### ManagementTaskPatch

- **Purpose**: Represents partial update input for mutable task fields.
- **Fields**:
  - `title?`: string
  - `description?`: string
  - `dueDate?`: ISO date-time string
  - `userId?`: GUID string
- **Validation/Rules**:
  - At least one mutable field must be supplied
  - Provided `title` cannot be empty
  - Provided `description` cannot be empty
  - Provided `dueDate` must be valid
  - Provided `userId` must be non-empty

### ManagementTaskStatusUpdate

- **Purpose**: Represents the payload for status-only task updates.
- **Fields**:
  - `status`: `Pending | InProgress | Completed`
- **Validation/Rules**:
  - Status must be one of the backend-supported enum values

### TaskQueryState

- **Purpose**: Represents the active task-list criteria sent to the task query endpoint.
- **Fields**:
  - `status?`: `Pending | InProgress | Completed`
  - `userId?`: GUID string
  - `search?`: string
  - `sortBy?`: `title | status | dueDate`
  - `sortDirection?`: `asc | desc`
  - `page`: number, default 1
  - `pageSize`: number, default 50
  - `includeArchived`: boolean, default false
- **Validation/Rules**:
  - `sortBy` must be one of `title`, `status`, `dueDate`
  - `sortDirection` must be `asc` or `desc`
  - `page` must be greater than 0
  - `pageSize` must be greater than 0 and less than or equal to 200

### PagedTaskResult

- **Purpose**: Represents paginated task-list results.
- **Fields**:
  - `items`: `ManagementTask[]`
  - `page`: number
  - `pageSize`: number
  - `totalCount`: number
- **Relationships**:
  - Backed by `TaskQueryState`
  - Consumed by list/table view and pagination controls

### TaskStatusSummary

- **Purpose**: Represents grouped counts by task status.
- **Fields**:
  - `statuses`: array of status/count pairs
- **Relationships**:
  - Displayed as supplemental summary data in the tasks feature

### ApiErrorState

- **Purpose**: Represents a normalized API failure used consistently across auth and tasks.
- **Fields**:
  - `statusCode`: number
  - `message`: string
  - `traceId`: string
  - `errors`: array of `{ propertyName, message }`
- **Relationships**:
  - Produced by HTTP layer normalization
  - Mapped to field-level and page-level feedback states

## Store Boundaries

### Global or Feature Store Candidates

- `auth`: session, authenticated user, auth request status
- `tasks`: current query, paged list, summary, overdue/due-within collections, selected task workflow status
- `router`: route state for guard-aware selectors and debugging

### Local Signal State Candidates

- Current form mode and dialog visibility
- Derived filter chips and badge counts
- Inline loading indicators tied to a single component
- Display formatting derived from store or route data
