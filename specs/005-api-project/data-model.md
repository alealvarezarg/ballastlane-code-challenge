# Data Model: API Project

## CreateManagementTaskRequestDto

- **Description**: Request model used to create a new management task through the API.
- **Fields**:
  - `Title`
  - `Description`
  - `Status`
  - `DueDate`
  - `UserId`
- **Validation Rules**:
  - `Title` is required and cannot be blank
  - `Description` is required and cannot be blank
  - `Status` must be one of the supported task status values
  - `DueDate` must be a valid non-default date
  - `UserId` must be a non-empty identifier
- **Rules**:
  - Validated by a FluentValidation validator in the API layer
  - Mapped to a new `ManagementTask` before calling the application service

## UpdateManagementTaskRequestDto

- **Description**: Request model used to update an existing management task through the API.
- **Fields**:
  - `Id`
  - `Title`
  - `Description`
  - `Status`
  - `DueDate`
  - `UserId`
- **Validation Rules**:
  - `Id` must be a non-empty identifier
  - `Title` is required and cannot be blank
  - `Description` is required and cannot be blank
  - `Status` must be one of the supported task status values
  - `DueDate` must be a valid non-default date
  - `UserId` must be a non-empty identifier
- **Rules**:
  - Validated by a FluentValidation validator in the API layer
  - Route and payload identifiers must align before the update request is processed

## ManagementTaskResponseDto

- **Description**: Response model returned for single-task and multi-task API responses.
- **Fields**:
  - `Id`
  - `Title`
  - `Description`
  - `Status`
  - `DueDate`
  - `UserId`
- **Rules**:
  - Mapped from `ManagementTask`
  - Used for `GET`, `POST`, and `PUT` success responses
  - Prevents direct exposure of the domain entity through the API surface

## ErrorResponseDto

- **Description**: Standard error payload returned by the global exception handler.
- **Fields**:
  - `StatusCode`
  - `Message`
  - `Errors`
  - `TraceId`
- **Rules**:
  - Used for validation failures, not-found results, and unexpected exceptions
  - Keeps client-facing failures consistent across endpoints

## Validation Components

- **Description**: FluentValidation validators responsible for request-level API input validation.
- **Components**:
  - `CreateManagementTaskRequestDtoValidator`
  - `UpdateManagementTaskRequestDtoValidator`
- **Rules**:
  - Live in the API layer
  - One validator class is created per DTO that requires inbound validation
  - Validate transport input shape and required values
  - Do not replace domain or application business validation

## OpenAPI Contract Artifact

- **Description**: Root-level YAML document that defines the external API contract for management task endpoints.
- **Fields**:
  - Endpoint paths
  - Request DTO schemas
  - Response DTO schemas
  - Error response schema
  - HTTP status codes
- **Rules**:
  - Maintained at `openapi/openapi.yaml`
  - Must remain synchronized with controller behavior and DTO definitions
  - May be mirrored in planning artifacts for feature-specific review

## Relationships

- `ManagementTaskController` accepts `CreateManagementTaskRequestDto` and `UpdateManagementTaskRequestDto`.
- `ManagementTaskController` returns `ManagementTaskResponseDto` and `ErrorResponseDto`.
- FluentValidation validators validate request DTOs before controller actions complete.
- `IManagementTaskService` remains the only application dependency used by the controller.
