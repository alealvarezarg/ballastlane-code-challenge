# Research: API DTO and Logging Improvements

## Decision 1: Keep DTO mapping explicit with the existing extension-method pattern

- **Decision**: Continue using the current API mapping helpers to convert domain/application models into response DTOs instead of adding AutoMapper or another mapping library.
- **Rationale**: The project already uses explicit mapping extensions, the number of affected models is small, and this keeps mapping behavior obvious while satisfying the DTO-only contract requirement.
- **Alternatives considered**:
  - AutoMapper: rejected because it adds configuration and indirection for a small, well-bounded API surface.
  - Returning anonymous objects from controllers: rejected because response contracts must remain explicit, named DTOs.

## Decision 2: Standardize `ManagementTaskStatus` as string JSON at the API boundary

- **Decision**: Configure JSON serialization/deserialization so `ManagementTaskStatus` values are exchanged as readable strings such as `Pending`, `InProgress`, and `Completed` across request DTOs, response DTOs, and query-bound models.
- **Rationale**: A centralized JSON configuration keeps behavior consistent everywhere the enum crosses the HTTP boundary and avoids duplicating conversion logic in controllers or validators.
- **Alternatives considered**:
  - Replacing enum properties with raw strings in DTOs: rejected because it weakens type safety and pushes conversion work into controllers or services.
  - Per-property custom converters: rejected because the same behavior is required broadly and should remain centralized.

## Decision 3: Preserve the existing validation response shape for invalid status values

- **Decision**: Let invalid enum inputs fail through the existing ASP.NET Core model-binding plus API validation pipeline so clients continue receiving the standard `ErrorResponseDto` payload.
- **Rationale**: The project already centralizes invalid request handling in `InvalidModelStateResponseFactory`, and this feature explicitly requires preserving that behavior.
- **Alternatives considered**:
  - Catching conversion errors in controllers: rejected because it duplicates framework behavior and makes controllers heavier.
  - Accepting unknown strings and validating later in services: rejected because invalid transport input should fail at the API boundary.

## Decision 4: Use Serilog only as the configured provider, not as a direct dependency in business code

- **Decision**: Configure Serilog once at startup to write to the console, while all application and API code continues depending on `ILogger<T>`.
- **Rationale**: This satisfies the requirement to use Serilog without coupling controllers or services to a concrete logging library and keeps configuration centralized.
- **Alternatives considered**:
  - Calling `Log.Information` or other Serilog static APIs throughout the codebase: rejected because it bypasses the existing .NET logging abstractions.
  - Leaving the default ASP.NET Core logger in place: rejected because the user explicitly requested Serilog.

## Decision 5: Log successful operations in the Application layer

- **Decision**: Emit informational logs from application services when primary use cases complete successfully.
- **Rationale**: Services already orchestrate meaningful operations like create, update, login, query, and archive, which makes them the most stable location for business-relevant success logs without bloating controllers.
- **Alternatives considered**:
  - Logging successes in controllers: rejected because controllers should stay translation-only.
  - Logging only in repositories: rejected because repository operations lack the full use-case context.

## Decision 6: Centralize expected-failure severity decisions in exception handling where possible

- **Decision**: Emit warning logs for expected validation, not-found, and business conflict outcomes in the global exception handler, and emit error logs there for unexpected exceptions.
- **Rationale**: Expected failures already converge through the exception pipeline, so the exception handler is the narrowest place to assign warning versus error severity while preserving the current error payloads.
- **Alternatives considered**:
  - Logging every exception at error level: rejected because validation and not-found outcomes are expected client-facing conditions.
  - Scattering warning logs in every controller action: rejected because it repeats logic and risks inconsistency.

## Decision 7: Keep response DTO coverage explicit for every response case

- **Decision**: Audit all endpoint success payloads and ensure each uses a named `*Dto` model, including paged results, summary items, login responses, and collection item shapes.
- **Rationale**: The requirement applies to every response case, not just top-level single-entity responses, so the plan must treat collection and wrapper payloads as part of the same contract surface.
- **Alternatives considered**:
  - Treating collections of DTOs as sufficient without reviewing wrapper types: rejected because wrappers and nested items are part of the exposed contract too.
  - Limiting the work to endpoints currently returning domain types directly: rejected because the spec requires a full API-wide guarantee.

## Decision 8: Keep OpenAPI as the canonical external reflection of the new enum contract

- **Decision**: Update both the feature contract file and the root `openapi/openapi.yaml` so every affected `ManagementTaskStatus` occurrence is documented as a string enum with readable values.
- **Rationale**: The status format change is a client-visible contract change, so the documentation must match the runtime behavior in all published OpenAPI descriptions.
- **Alternatives considered**:
  - Updating runtime behavior only: rejected because it would leave generated documentation misleading.
  - Using free-form string schemas without enumerated values: rejected because clients benefit from explicit allowed values.
