# Research: ManagementUser Authentication

## Decision 1: Use simple bearer-token authentication with built-in ASP.NET Core primitives

- **Decision**: Protect secured endpoints with bearer-token authentication and issue a signed token from the login endpoint rather than introducing cookies, refresh tokens, or an external identity provider.
- **Rationale**: Bearer tokens fit the current API-only solution, keep clients simple, and let the feature secure all existing endpoints with minimal surface-area changes.
- **Alternatives considered**:
  - Cookie-based authentication: rejected because this project is currently an API without a browser-session requirement.
  - External identity provider integration: rejected because it adds substantial infrastructure and configuration complexity outside the requested scope.
  - Custom opaque token storage: rejected because it adds persistence and revocation mechanics that are unnecessary for the current feature size.

## Decision 2: Use the built-in password hasher instead of a custom hashing implementation

- **Decision**: Hash management user passwords with the standard `PasswordHasher<TUser>` behavior and persist only the resulting hash.
- **Rationale**: The built-in hasher is well-understood, salted, versioned, and avoids reinventing sensitive security logic while satisfying the requirement to avoid plaintext password storage or exposure.
- **Alternatives considered**:
  - Writing a custom PBKDF2 or BCrypt wrapper from scratch: rejected because the feature explicitly asks to avoid unnecessary complexity.
  - Storing encrypted passwords: rejected because passwords should be hashed, not reversibly stored.
  - Keeping plaintext only for local development: rejected because it violates the feature requirement and would create unsafe behavior differences.

## Decision 3: Keep authentication dependencies behind application-facing contracts

- **Decision**: Model password hashing/verification and token creation as application abstractions that are implemented in Infrastructure.
- **Rationale**: This preserves clean architecture boundaries while keeping controllers and application services independent from framework-specific security types.
- **Alternatives considered**:
  - Hashing and signing directly inside controllers: rejected because it moves security behavior into the delivery layer.
  - Using framework helpers directly in application services without abstractions: rejected because it would couple the application layer to transport/security implementation details.

## Decision 4: Add a dedicated ManagementUser service and repository path

- **Decision**: Introduce explicit application contracts and service orchestration for management users instead of overloading the existing task service.
- **Rationale**: User registration and login are separate use cases with their own persistence, validation, and token responsibilities, and keeping them distinct avoids unrelated growth in `ManagementTaskService`.
- **Alternatives considered**:
  - Extending `ManagementTaskService` to own user authentication: rejected because it mixes unrelated responsibilities.
  - Putting login logic directly in a controller: rejected because credential verification and token issuance are business/application concerns.

## Decision 5: Treat task-user existence validation as an application rule in the task create flow

- **Decision**: Before persisting a new management task, the application layer will query the management user repository to verify that the referenced user exists and will fail fast with a business validation exception if not.
- **Rationale**: The rule coordinates two aggregates and is best owned by the application layer, while repositories remain focused on storage and controllers remain free of business logic.
- **Alternatives considered**:
  - Deferring the check to the repository or database layer: rejected because the rule should be expressed as an explicit business outcome, not a persistence accident.
  - Only validating the identifier shape: rejected because the feature requires existence validation against stored data.

## Decision 6: Mark the three ManagementUser endpoints public and require authentication everywhere else

- **Decision**: Keep registration, login, and test-only user retrieval anonymous, and protect all remaining controllers/actions by default, including every ManagementTask route.
- **Rationale**: This is the smallest rule set that matches the specification and is easy to reason about in code and documentation.
- **Alternatives considered**:
  - Leaving some task reads anonymous: rejected because the requirement explicitly covers every ManagementTask endpoint.
  - Applying authorization only per task action and not across the broader API: rejected because the feature requires consistent protection for all non-public endpoints.

## Decision 7: Keep login responses minimal and never return password material

- **Decision**: Return only the authenticated user details needed by clients plus the issued bearer token and its basic expiration metadata; never echo plaintext passwords or stored hashes in any response.
- **Rationale**: A narrow authentication result reduces accidental exposure while still giving clients everything needed to authenticate subsequent requests.
- **Alternatives considered**:
  - Returning the full persisted user record: rejected because it would expose internal security fields.
  - Returning only a token without user context: rejected because lightweight user context simplifies testing and client flow validation.

## Decision 8: Preserve the existing error pipeline and use business exceptions for invalid credentials and missing task owners

- **Decision**: Surface invalid credentials and missing management-user ownership through known exception types that map through the existing global exception handler and error response shape.
- **Rationale**: This keeps failure handling consistent with the rest of the API and avoids ad hoc controller-specific error formatting.
- **Alternatives considered**:
  - Returning handcrafted error objects directly from controllers: rejected because it bypasses the existing error-handling pattern.
  - Treating all credential failures as generic unexpected errors: rejected because clients need deterministic authentication failure behavior.

## Decision 9: Keep the implementation slice order explicitly test-first

- **Decision**: Implement the feature in red-green-refactor order, starting with failing tests for user invariants and orchestration rules, then infrastructure helpers, and finally API authorization and contract coverage.
- **Rationale**: The repository constitution and the user request both require TDD, and the authentication surface benefits from strong regression coverage.
- **Alternatives considered**:
  - Building the auth flow first and adding tests afterward: rejected because it violates the required delivery approach.
  - Relying only on end-to-end API tests: rejected because credential hashing, login orchestration, and task ownership checks need faster lower-layer feedback.
