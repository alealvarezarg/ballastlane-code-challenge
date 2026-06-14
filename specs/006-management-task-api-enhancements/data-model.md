# Data Model: ManagementTask API Enhancements

## ManagementTask

- **Description**: Core task aggregate used across Domain, Application, Infrastructure, and API mappings.
- **Fields**:
  - `Id`
  - `Title`
  - `Description`
  - `Status`
  - `DueDate`
  - `UserId`
  - `IsArchived`
  - `ArchivedAt`
- **Validation Rules**:
  - `Id` must be a non-empty identifier
  - `Title` and `Description` are required and trimmed
  - `DueDate` must be a valid date
  - `UserId` must be a non-empty identifier
  - Archived tasks cannot be modified through normal update or status-change flows
- **State Rules**:
  - New tasks default to `Pending` when status is omitted at creation time
  - Allowed transitions: `Pending -> InProgress`, `Pending -> Completed`, `InProgress -> Completed`
  - `Completed` tasks are non-editable and cannot transition further
  - Archiving is allowed only while the task is not completed

## ManagementTaskQueryRequestDto

- **Description**: Query shape used to retrieve filtered task lists.
- **Fields**:
  - `Status`
  - `UserId`
  - `Search`
  - `SortBy`
  - `SortDirection`
  - `Page`
  - `PageSize`
  - `IncludeArchived`
- **Validation Rules**:
  - `SortBy` accepts only `title`, `status`, or `dueDate`
  - `SortDirection` accepts only `asc` or `desc`
  - `Page` must be greater than zero when supplied
  - `PageSize` must be greater than zero and within the configured upper bound
  - `Search` targets `Title` and `Description`
- **Rules**:
  - All supplied filters are combined with logical AND
  - Active-only retrieval is the default when `IncludeArchived` is omitted or false

## CreateManagementTaskRequestDto

- **Description**: Request model used to create a task, optionally with an idempotency key header.
- **Fields**:
  - `Title`
  - `Description`
  - `Status` (optional)
  - `DueDate`
  - `UserId`
- **Validation Rules**:
  - Required text, date, and identifier rules match the task aggregate
  - If `Status` is supplied, it must be a supported value
- **Rules**:
  - Missing `Status` becomes `Pending`
  - An `Idempotency-Key` header binds duplicate protection to this request

## UpdateManagementTaskRequestDto

- **Description**: Full replacement request for mutable task fields.
- **Fields**:
  - `Id`
  - `Title`
  - `Description`
  - `Status`
  - `DueDate`
  - `UserId`
- **Validation Rules**:
  - `Id` must be present and match the route identifier
  - All mutable fields are required for full update
- **Rules**:
  - Rejected when the task is in a non-editable status or archived

## PatchManagementTaskRequestDto

- **Description**: Partial update request containing only mutable fields the client wants to change.
- **Fields**:
  - `Title`
  - `Description`
  - `DueDate`
  - `UserId`
- **Validation Rules**:
  - At least one mutable field must be supplied
  - Any supplied field must satisfy the same value rules as full update
- **Rules**:
  - `Status`, `IsArchived`, and identity fields are not patchable through this contract
  - Rejected when the task is in a non-editable status or archived

## UpdateManagementTaskStatusRequestDto

- **Description**: Narrow request used to change only task status.
- **Fields**:
  - `Status`
- **Validation Rules**:
  - `Status` must be a supported value
- **Rules**:
  - Transition must be allowed from the current status
  - Archived tasks cannot change status

## ManagementTaskSummaryResponseDto

- **Description**: Response model that reports counts of tasks grouped by status.
- **Fields**:
  - `Statuses`
- **Nested Fields**:
  - `Status`
  - `Count`
- **Rules**:
  - Counts reflect the same active/archive scope as the request

## ErrorResponseDto

- **Description**: Standard error payload returned for validation, rule, not-found, conflict, and unexpected failures.
- **Fields**:
  - `StatusCode`
  - `Message`
  - `Errors`
  - `TraceId`
- **Rules**:
  - Present for every non-successful response
  - `TraceId` is always populated

## IdempotencyRecord

- **Description**: Persistence model that tracks create-request deduplication.
- **Fields**:
  - `Key`
  - `RequestFingerprint`
  - `TaskId`
  - `CreatedAt`
- **Validation Rules**:
  - `Key` must be unique
  - `RequestFingerprint` must represent the original create payload
- **Rules**:
  - Exact repeats return the original created task
  - Same key with a different fingerprint results in a conflict response

## Relationships

- `ManagementTaskController` accepts `ManagementTaskQueryRequestDto`, `CreateManagementTaskRequestDto`, `UpdateManagementTaskRequestDto`, `PatchManagementTaskRequestDto`, and `UpdateManagementTaskStatusRequestDto`.
- `ManagementTaskController` returns `ManagementTaskResponseDto`, paged task list responses, `ManagementTaskSummaryResponseDto`, and `ErrorResponseDto`.
- `ManagementTask` uses archive metadata in addition to workflow status.
- `IdempotencyRecord` links a create request key to one resulting `ManagementTask`.
- Application caching stores list, item, overdue, due-within, and summary reads and clears them after writes.
