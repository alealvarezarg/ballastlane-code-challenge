# Data Model: ManagementUser Authentication

## ManagementUser

- **Description**: Core user aggregate used for registration, login lookup, task ownership validation, and API response mapping.
- **Fields**:
  - `Id`
  - `Username`
  - `Email`
  - `PasswordHash`
- **Validation Rules**:
  - `Id` must be a non-empty identifier
  - `Username`, `Email`, and `PasswordHash` are required and trimmed
  - `PasswordHash` must already be hashed before persistence
- **Rules**:
  - Plaintext passwords are never stored on the aggregate
  - `Username` and `Email` are candidate unique lookup fields for registration and login flows

## CreateManagementUserRequestDto

- **Description**: Public request model used to register a new management user.
- **Fields**:
  - `Username`
  - `Email`
  - `Password`
- **Validation Rules**:
  - All fields are required
  - `Email` must be in a valid email format
  - `Password` must satisfy the project's minimum strength rule for this feature
- **Rules**:
  - `Password` is accepted only on the request boundary and is converted to a hash before persistence
  - Duplicate unique login values are rejected with a business validation error

## LoginManagementUserRequestDto

- **Description**: Public request model used to authenticate a management user.
- **Fields**:
  - `Email`
  - `Password`
- **Validation Rules**:
  - Both fields are required
  - `Email` must be in a valid email format
- **Rules**:
  - Invalid credentials do not reveal whether the email or password was incorrect

## ManagementUserResponseDto

- **Description**: Public response model used for create and get-by-id responses.
- **Fields**:
  - `Id`
  - `Username`
  - `Email`
- **Rules**:
  - Never includes password or password-hash fields

## ManagementUserLoginResponseDto

- **Description**: Public response model returned after successful login.
- **Fields**:
  - `AccessToken`
  - `TokenType`
  - `ExpiresAt`
  - `User`
- **Nested Fields**:
  - `Id`
  - `Username`
  - `Email`
- **Rules**:
  - `TokenType` is always `Bearer`
  - `User` uses the same safe response shape as other public user responses

## AuthenticatedUserIdentity

- **Description**: Application-facing representation of the authenticated principal extracted from the bearer token.
- **Fields**:
  - `UserId`
  - `Username`
  - `Email`
- **Rules**:
  - Used for authorization context only
  - Built from token claims rather than persisted as a separate entity

## ManagementUserDocument

- **Description**: Infrastructure persistence document stored in LiteDB for management users.
- **Fields**:
  - `Id`
  - `Username`
  - `Email`
  - `PasswordHash`
- **Validation Rules**:
  - `Id` must be unique
  - Unique indexes should exist for the chosen login identifier fields
- **Rules**:
  - Persists only hashed password material

## Task Ownership Reference

- **Description**: The `UserId` supplied during management task creation that must point to an existing `ManagementUser`.
- **Fields**:
  - `UserId`
- **Rules**:
  - Checked before the task is created
  - Failing the check prevents task persistence entirely

## ErrorResponseDto

- **Description**: Standard error payload returned for validation, authentication, authorization, business rule, not-found, conflict, and unexpected failures.
- **Fields**:
  - `StatusCode`
  - `Message`
  - `Errors`
  - `TraceId`
- **Rules**:
  - Present for every non-successful response
  - Continues to use the existing global exception and validation pipeline

## Relationships

- `ManagementUserController` accepts `CreateManagementUserRequestDto` and `LoginManagementUserRequestDto`.
- `ManagementUserController` returns `ManagementUserResponseDto`, `ManagementUserLoginResponseDto`, and `ErrorResponseDto`.
- `ManagementTask` creation depends on `Task Ownership Reference` resolving to an existing `ManagementUser`.
- Application services depend on repository, password-hashing, and token-issuing abstractions rather than framework types directly.
- Infrastructure stores `ManagementUserDocument` in LiteDB and implements password verification plus token creation helpers used by the application layer.
