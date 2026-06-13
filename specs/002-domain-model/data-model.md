# Data Model: Domain Model

## ManagementTask

- **Description**: Represents a unit of work owned by one user.
- **Fields**:
  - `Id`: Unique task identifier.
  - `Title`: Required task title.
  - `Description`: Required task description.
  - `Status`: Current lifecycle state using `ManagementTaskStatus`.
  - `DueDate`: Required due date value.
  - `UserId`: Required identifier of the owning user.
- **Validation Rules**:
  - `Id` must be a valid, non-empty identifier.
  - `Title` must not be null, empty, or whitespace-only.
  - `Description` must not be null, empty, or whitespace-only.
  - `DueDate` must be a real, non-default date value.
  - `UserId` must reference one user through a valid identifier.
  - `Status` must be one of the defined `ManagementTaskStatus` values.
- **Behavior**:
  - Create in a valid state only.
  - Update task information through an explicit method.
  - Change status through an explicit method.
- **State Transitions**:
  - Allowed values: `Pending`, `InProgress`, `Completed`.
  - Any requested value outside the enum is invalid.

## ManagementUser

- **Description**: Represents the owner of tasks.
- **Fields**:
  - `Id`: Unique user identifier.
  - `Username`: Required user name.
  - `Email`: Required email value.
  - `PasswordHash`: Required stored password-hash value.
- **Validation Rules**:
  - `Id` must be a valid, non-empty identifier.
  - `Username` must not be null, empty, or whitespace-only.
  - `Email` must not be null, empty, or whitespace-only.
  - `PasswordHash` must not be null, empty, or whitespace-only.
- **Behavior**:
  - Create in a valid state only.

## ManagementTaskStatus

- **Description**: Allowed lifecycle states for a task.
- **Values**:
  - `Pending`
  - `InProgress`
  - `Completed`

## Domain Validation Error

- **Description**: Domain-specific failure raised when an entity is created or modified with invalid data.
- **Usage**:
  - Invalid task creation
  - Invalid task modification
  - Invalid status change
  - Invalid user creation

## Relationships

- A `ManagementTask` belongs to exactly one `ManagementUser` through `UserId`.
- No navigation properties or persistence-specific relationship structures are included.
