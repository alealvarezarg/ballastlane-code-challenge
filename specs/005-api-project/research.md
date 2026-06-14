# Research: API Project

## Decision 1: Use one thin controller for task CRUD

- **Decision**: Implement a single `ManagementTaskController` for all `ManagementTask` CRUD endpoints.
- **Rationale**: The feature scope is limited to one resource, so one controller keeps routing and request translation easy to review.
- **Alternatives considered**:
  - Splitting read and write endpoints into separate controllers: rejected because it adds structure without solving a current problem.
  - Minimal APIs: rejected because the feature explicitly requests a controller-based structure.

## Decision 2: Use DTOs in the API boundary and map to domain tasks in the controller

- **Decision**: Define request and response DTOs in the API project and map them to and from `ManagementTask` in the controller layer.
- **Rationale**: This keeps domain entities out of HTTP contracts and preserves the API boundary required by the specification.
- **Alternatives considered**:
  - Returning domain entities directly: rejected because it couples API contracts to domain models.
  - Moving DTOs into Application: rejected because these shapes are delivery concerns rather than use-case contracts.

## Decision 3: Use FluentValidation for DTO validation

- **Decision**: Validate API DTOs with FluentValidation using one validator class per DTO where request validation applies, and keep that validation outside controller actions.
- **Rationale**: FluentValidation keeps API input validation explicit, testable, and separate from controller actions while still allowing domain validation to remain authoritative for business rules.
- **Alternatives considered**:
  - Only relying on domain exceptions: rejected because malformed HTTP input should be rejected consistently before reaching controller action logic where practical.
  - Only using data annotations: rejected because FluentValidation was explicitly requested and provides more expressive validation rules.

## Decision 4: Use global exception handling with one consistent error payload

- **Decision**: Add one global exception-handling entry point that translates validation, not-found, and unexpected exceptions into a shared error response model.
- **Rationale**: A single pipeline-based handler keeps controllers thin and ensures predictable client-facing failures.
- **Alternatives considered**:
  - Handling exceptions inside each controller action: rejected because it duplicates logic and makes the API boundary harder to maintain.
  - Returning framework-default error responses: rejected because the feature requires consistency.

## Decision 5: Keep dependency registration in `Program.cs` through extension methods

- **Decision**: Register Application and Infrastructure dependencies by calling their existing extension methods from `Program.cs`.
- **Rationale**: This matches the current project structure and keeps composition explicit without introducing another registration layer.
- **Alternatives considered**:
  - Re-registering individual services directly in the API project: rejected because it duplicates ownership that already belongs to the lower layers.
  - Creating a new aggregator extension solely for the API: rejected because it adds indirection without extra value.

## Decision 6: Maintain a root-level OpenAPI YAML as the canonical API contract

- **Decision**: Keep OpenAPI enabled in the API project and maintain the canonical contract as `openapi/openapi.yaml` at the solution root, with the feature-level contract artifact mirroring that definition for planning traceability.
- **Rationale**: The API feature must be easy to discover and test, and a root-level YAML file gives the solution one obvious source of truth for external API behavior.
- **Alternatives considered**:
  - Leaving endpoint discovery to source inspection: rejected because it does not meet the documentation and testing goals.
  - Keeping the contract only under `specs/`: rejected because the user explicitly requires a root-level `openapi` folder for the API definition.
