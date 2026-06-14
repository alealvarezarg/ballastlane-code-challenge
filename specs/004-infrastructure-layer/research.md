# Research: Infrastructure Layer

## Decision 1: Use one repository implementation for task persistence

- **Decision**: Implement `IManagementTaskRepository` with a single `ManagementTaskRepository`.
- **Rationale**: The feature scope is limited to one task persistence contract, so one repository keeps the Infrastructure layer simple and easy to review.
- **Alternatives considered**:
  - Splitting reads and writes into separate repositories: rejected because it adds abstraction without a stated need.
  - Generic repository infrastructure: rejected because it is broader than the current use case.

## Decision 2: Use LiteDB embedded file storage

- **Decision**: Store `ManagementTask` documents in a local embedded LiteDB file.
- **Rationale**: The specification explicitly requires LiteDB and an embedded local database file, which fits a lightweight persistence setup.
- **Alternatives considered**:
  - External relational database: rejected because it conflicts with the requested storage approach.
  - In-memory storage: rejected because it does not satisfy persistence requirements.

## Decision 3: Isolate database setup under `Database`

- **Decision**: Keep LiteDB connection and database exposure classes under a dedicated `Database` folder.
- **Rationale**: This matches the requested project organization and keeps connection concerns separate from repository behavior.
- **Alternatives considered**:
  - Embedding LiteDB connection logic directly in the repository: rejected because it mixes responsibilities.
  - Moving database setup into dependency registration only: rejected because it reduces clarity and reuse.

## Decision 4: Keep Infrastructure registration in one extension method

- **Decision**: Use `AddInfrastructureDependencies` in `DependencyInjection.cs` as the single Infrastructure registration entry point.
- **Rationale**: A single extension method keeps outer-layer composition predictable and matches the requested structure.
- **Alternatives considered**:
  - Registering repositories directly from API or Application: rejected because Infrastructure should own its own persistence registrations.
  - Multiple Infrastructure registration methods: rejected because they complicate composition without adding value.

## Decision 5: Return missing-task results without adding business logic

- **Decision**: Let repository read operations surface missing-document results through the repository contract without adding business rules in Infrastructure.
- **Rationale**: Infrastructure should translate persistence state only and must not introduce business decisions.
- **Alternatives considered**:
  - Throwing business-specific exceptions from Infrastructure: rejected because it mixes persistence and business behavior.

## Decision 6: Use simple LiteDB settings and context classes

- **Decision**: Use `LiteDbSettings` and `LiteDbContext` as the minimal Infrastructure-owned types for configuring and exposing the embedded LiteDB connection.
- **Rationale**: This keeps database setup explicit and reusable without introducing broader abstraction layers.
- **Alternatives considered**:
  - Registering raw `LiteDatabase` directly everywhere: rejected because it spreads connection setup details.
  - Adding factory and provider layers: rejected because they add complexity beyond the current scope.
