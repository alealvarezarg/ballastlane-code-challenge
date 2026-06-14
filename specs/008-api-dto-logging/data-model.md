# Data Model: API DTO and Logging Improvements

## ManagementTaskResponseDto

- **Description**: Explicit API response model returned for single-task success responses.
- **Fields**:
  - `Id`
  - `Title`
  - `Description`
  - `Status`
  - `DueDate`
  - `UserId`
  - `IsArchived`
  - `ArchivedAt`
- **Rules**:
  - `Status` is serialized as the readable API value, not as a numeric enum
  - The DTO is populated from a domain `ManagementTask` through API-layer mapping

## PagedManagementTaskResponseDto

- **Description**: Wrapper DTO for paged task list responses.
- **Fields**:
  - `Items`
  - `Page`
  - `PageSize`
  - `TotalCount`
- **Rules**:
  - `Items` contains only `ManagementTaskResponseDto` instances
  - No domain entity or application paging model is returned directly

## ManagementTaskSummaryResponseDto

- **Description**: Wrapper DTO for grouped task counts.
- **Fields**:
  - `Statuses`
- **Rules**:
  - Every item in `Statuses` is a `ManagementTaskStatusCountDto`
  - Summary responses remain DTO-only even when sourced from application summary models

## ManagementTaskStatusCountDto

- **Description**: DTO that represents one task-status bucket in the summary response.
- **Fields**:
  - `Status`
  - `Count`
- **Rules**:
  - `Status` uses the same readable string contract as every other API surface
  - The DTO is created from `ManagementTaskStatusSummary`, not returned as that application model directly

## ManagementTaskQueryRequestDto

- **Description**: Query-bound API request model used to filter and paginate task retrieval.
- **Fields**:
  - `Status`
  - `UserId`
  - `Search`
  - `SortBy`
  - `SortDirection`
  - `Page`
  - `PageSize`
  - `IncludeArchived`
- **Rules**:
  - `Status`, when supplied, must accept readable enum strings and reject unknown values through the existing validation response path
  - Query DTO values are translated into application query options before service execution

## CreateManagementTaskRequestDto / UpdateManagementTaskRequestDto / UpdateManagementTaskStatusRequestDto

- **Description**: Existing write DTOs that accept task status input.
- **Fields affected**:
  - `Status`
- **Rules**:
  - Valid string forms such as `Pending`, `InProgress`, and `Completed` are accepted
  - Invalid values are rejected with the current `ErrorResponseDto` validation format
  - Internal business logic still receives the enum type after model binding

## ManagementUserResponseDto

- **Description**: Explicit response DTO for management user create and retrieval responses.
- **Fields**:
  - `Id`
  - `Username`
  - `Email`
- **Rules**:
  - Remains transport-only and never exposes password material or a domain entity directly

## ManagementUserLoginResponseDto

- **Description**: Explicit response DTO for successful authentication.
- **Fields**:
  - `AccessToken`
  - `TokenType`
  - `ExpiresAt`
  - `User`
- **Rules**:
  - Nested `User` uses `ManagementUserResponseDto`
  - Response continues to avoid exposing persistence or domain internals

## ManagementStatusApiValue

- **Description**: The public string representation of `ManagementTaskStatus` accepted and returned by the API.
- **Allowed Values**:
  - `Pending`
  - `InProgress`
  - `Completed`
- **Rules**:
  - The same set of string values is used in request bodies, query parameters, response payloads, and OpenAPI documentation
  - Invalid or unknown values are treated as request validation failures

## ApplicationLogEvent

- **Description**: Structured operational log emitted through `ILogger<T>` for meaningful API use cases and outcomes.
- **Fields**:
  - `Category`
  - `Level`
  - `MessageTemplate`
  - Context fields such as `OperationName`, `EntityId`, `StatusCode`, `TraceId`, or `UserId` when relevant
- **Rules**:
  - `Information` is used for successful main operations
  - `Warning` is used for expected validation, not-found, authentication, and business-conflict scenarios
  - `Error` is used for unexpected exceptions

## LoggingConfiguration

- **Description**: Centralized startup configuration that binds Serilog into the ASP.NET Core logging pipeline.
- **Fields**:
  - Console sink enablement
  - Minimum/default levels as needed for local observability
- **Rules**:
  - Serilog is configured once during application startup
  - Console output is the only sink in scope for this feature
  - Downstream code continues using `ILogger<T>` rather than concrete Serilog APIs

## Relationships

- `ManagementTaskController` returns `ManagementTaskResponseDto`, `PagedManagementTaskResponseDto`, `ManagementTaskSummaryResponseDto`, and DTO collections only.
- `ManagementTaskMappings` converts domain/application task models into DTO-only response models.
- `ManagementUserController` continues returning only `ManagementUserResponseDto` and `ManagementUserLoginResponseDto`.
- JSON serialization settings define the external `ManagementStatusApiValue` contract for every DTO property and query parameter involving `ManagementTaskStatus`.
- `ManagementTaskService` and `ManagementUserService` emit informational logs for completed operations and may emit warnings for expected non-exception outcomes.
- `GlobalExceptionHandler` emits warning logs for expected handled failures and error logs for unexpected exceptions while preserving `ErrorResponseDto`.
