# Research: Application Layer

## Decision 1: Keep CRUD orchestration in one application service

- **Decision**: Use a single `ManagementTaskService` to coordinate task CRUD operations exposed through `IManagementTaskService`.
- **Rationale**: The feature scope is small and centered on one task-management use case, so one service keeps the layer easy to review and avoids unnecessary splitting.
- **Alternatives considered**:
  - Multiple operation-specific services: rejected because they add coordination overhead without current value.
  - Direct repository usage from outer layers: rejected because it bypasses the Application boundary.

## Decision 2: Define repository behavior through one repository contract

- **Decision**: Use `IManagementTaskRepository` as the only persistence-facing contract for `ManagementTask` CRUD support.
- **Rationale**: The specification explicitly asks for one repository abstraction and the Application layer should depend on that contract rather than persistence details.
- **Alternatives considered**:
  - Separate read and write repositories: rejected because they add extra abstraction without a stated need.
  - Generic repository patterns: rejected because they are broader than the current use case.

## Decision 3: Cache only read operations

- **Decision**: Use `IMemoryCache` for `GetById` and `GetAll`, and do not cache create, update, or delete operations.
- **Rationale**: The feature explicitly targets read performance, and write operations should remain direct while triggering invalidation of stale read entries.
- **Alternatives considered**:
  - Caching all operations: rejected because writes do not benefit from the same pattern and it complicates consistency.
  - No caching: rejected because it misses an explicit feature requirement.

## Decision 4: Invalidate cached task collections and affected task items after writes

- **Decision**: Invalidate the cached `GetAll` result after any create, update, or delete, and invalidate the relevant `GetById` cache entry when a specific task changes.
- **Rationale**: This is the simplest strategy that keeps read results consistent without introducing advanced cache coordination.
- **Alternatives considered**:
  - Time-based expiration only: rejected because stale data could remain after writes.
  - Fine-grained collection patching: rejected because it adds unnecessary complexity for the current scope.

## Decision 5: Register Application-layer dependencies in one extension method

- **Decision**: Use `AddApplicationDependencies` in `DependencyInjection.cs` as the single registration entry point for Application-layer dependencies.
- **Rationale**: A single extension method keeps composition predictable for outer layers and matches the requested structure.
- **Alternatives considered**:
  - Inline registration in API or Infrastructure: rejected because it weakens Application ownership.
  - Multiple registration entry points: rejected because they complicate consumption without adding value.
