# Data Model: Application Layer

## IManagementTaskRepository

- **Description**: Application contract for persistence-facing CRUD collaboration around `ManagementTask`.
- **Responsibilities**:
  - Create a task record
  - Retrieve one task by identifier
  - Retrieve all tasks
  - Update a task
  - Delete a task
- **Rules**:
  - Must support `ManagementTask` as the task entity type used by the Application layer.
  - Must remain an abstraction owned by the Application project.

## IManagementTaskService

- **Description**: Application contract for task management operations exposed to outer layers.
- **Responsibilities**:
  - Coordinate create, read, update, and delete task operations
  - Expose Application-layer task management behavior without leaking persistence concerns
- **Rules**:
  - Read operations include cache-aware behavior.
  - Write operations trigger cache invalidation for affected read results.

## ManagementTaskService

- **Description**: Application service implementation that coordinates task operations.
- **Collaborators**:
  - `IManagementTaskRepository`
  - `IMemoryCache`
- **Behavior**:
  - `GetById` checks the cache before querying the repository.
  - `GetAll` checks the cache before querying the repository.
  - Create, update, and delete operations execute through the repository and invalidate stale cached entries.
- **Rules**:
  - Must implement `IManagementTaskService`.
  - Must not depend on Infrastructure or API concerns.
  - Must keep caching behavior limited to read optimization and invalidation.

## Application Dependency Registration

- **Description**: Application-owned registration entry point in `DependencyInjection.cs`.
- **Behavior**:
  - Registers `IManagementTaskService` and `ManagementTaskService`
  - Registers any other dependencies owned by the Application layer
- **Rules**:
  - Exposed through `AddApplicationDependencies`
  - Intended for consumption by outer layers without moving registration logic out of Application

## Cache Model

- **Cached Read Types**:
  - Single task by identifier
  - Full task collection
- **Invalidation Rules**:
  - Create invalidates cached task collections
  - Update invalidates cached task collections and the affected single-task cache entry
  - Delete invalidates cached task collections and the affected single-task cache entry
