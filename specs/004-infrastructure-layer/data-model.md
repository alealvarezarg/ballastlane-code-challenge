# Data Model: Infrastructure Layer

## LiteDB Database Connection

- **Description**: Infrastructure-owned component that configures and exposes access to the embedded LiteDB database file.
- **Responsibilities**:
  - Open or provide access to the LiteDB database
  - Centralize database file configuration
  - Expose collection access for repository implementations
- **Rules**:
  - Lives under the `Database` folder
  - Remains Infrastructure-only and contains no business logic
  - Uses `LiteDbSettings` and `LiteDbContext` to centralize LiteDB access

## ManagementTask Document Storage

- **Description**: Persistent LiteDB document representation of `ManagementTask`.
- **Stored Data**:
  - Task identifier
  - Title
  - Description
  - Status
  - Due date
  - User identifier
- **Rules**:
  - Stored through LiteDB collections
  - Backed by a local embedded database file

## ManagementTaskRepository

- **Description**: Infrastructure repository implementation of `IManagementTaskRepository`.
- **Collaborators**:
  - `LiteDbContext`
- **Responsibilities**:
  - Create task documents
  - Retrieve one task by identifier
  - Retrieve all task documents
  - Update task documents
  - Delete task documents
- **Rules**:
  - Lives under the `Repositories` folder
  - Implements the full CRUD contract required by the Application layer
  - Contains persistence behavior only and no business logic
  - Uses `KeyNotFoundException` for missing updates without adding business-specific exceptions

## Infrastructure Dependency Registration

- **Description**: Infrastructure-owned registration entry point in `DependencyInjection.cs`.
- **Behavior**:
  - Registers the LiteDB database connection
  - Registers `ManagementTaskRepository` as the implementation of `IManagementTaskRepository`
- **Rules**:
  - Exposed through `AddInfrastructureDependencies`
  - Intended for consumption by outer layers without moving persistence registration outside Infrastructure
