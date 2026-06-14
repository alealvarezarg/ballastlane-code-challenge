# Feature Specification: API DTO and Logging Improvements

**Feature Branch**: `008-api-dto-logging`

**Created**: 2026-06-14

**Status**: Completed

**Input**: User description: "Define the specification for API improvements that require explicit response DTOs for all API responses, string-based `ManagementStatus` values for requests and responses, validation failures for invalid status values, application logging through the existing .NET logging abstractions, and unit test coverage while keeping the design simple and aligned with the current architecture."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Receive DTO-Only API Responses (Priority: P1)

As an API consumer, I want every endpoint to return explicit response DTOs instead of domain entities so I can integrate against a stable, transport-focused contract.

**Why this priority**: Response shape consistency is a cross-cutting contract requirement that affects every consumer-facing endpoint and reduces accidental exposure of internal domain structure.

**Independent Test**: Call representative API endpoints and confirm the response payloads are DTOs only, with no direct domain entity exposure and no contract shape regressions.

**Acceptance Scenarios**:

1. **Given** a successful endpoint response, **When** the payload is returned to the client, **Then** the response contains only explicit DTO fields and no domain entity is exposed directly.
2. **Given** an endpoint with multiple successful response cases, **When** each case is returned, **Then** every case uses an explicit DTO whose name ends with `Dto`.
3. **Given** existing domain-to-API mappings, **When** a controller returns data, **Then** the controller returns mapped response DTOs rather than the original domain objects.

---

### User Story 2 - Exchange Readable Management Status Values (Priority: P1)

As an API consumer, I want management status values to be sent and received as readable strings such as `Pending`, `InProgress`, and `Completed` so the contract is easier to understand and use.

**Why this priority**: Readable status values improve API usability and documentation quality while reducing ambiguity for clients.

**Independent Test**: Send and receive requests and responses that include management status values, confirm valid string values are accepted and returned, and confirm invalid strings produce the standard validation error response.

**Acceptance Scenarios**:

1. **Given** an API response that includes management status, **When** the response is serialized, **Then** the status is returned as a readable string rather than a numeric value.
2. **Given** a valid API request that includes management status as a string, **When** the request is processed, **Then** the status value is accepted and converted internally to the enum type.
3. **Given** an invalid management status string in an API request, **When** the request is processed, **Then** the system rejects the request with the existing validation error response.

---

### User Story 3 - Observe API Behavior Through Consistent Logging (Priority: P2)

As an operator or developer, I want the application to log successful operations, expected warnings, and unexpected failures through the existing logging abstractions so I can diagnose behavior without adding ad hoc instrumentation.

**Why this priority**: Logging improves supportability and troubleshooting but should follow the established architecture and remain simple.

**Independent Test**: Exercise successful operations, validation or not-found scenarios, and unexpected exceptions, then confirm the expected informational, warning, and error log behavior occurs at the appropriate layer boundaries.

**Acceptance Scenarios**:

1. **Given** a successful main operation, **When** the operation completes, **Then** the application writes an informational log describing the completed action.
2. **Given** a validation failure or not-found scenario, **When** the request is handled, **Then** the application writes a warning log for the expected non-success outcome.
3. **Given** an unexpected exception, **When** the exception is handled, **Then** the application writes an error log with enough context to support diagnosis.

### Edge Cases

- A response path currently returns a domain entity directly rather than a mapped DTO.
- A request supplies a management status string with incorrect casing, whitespace, or an unknown value.
- Different endpoints currently serialize the same management status using inconsistent formats.
- A validation or not-found outcome occurs before a main operation completes and still needs warning-level logging.
- An unexpected exception occurs after partial processing and still needs error-level logging without changing the public error contract.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST NOT expose domain entities directly from any API endpoint.
- **FR-002**: The system MUST return explicit response DTOs for every API response case.
- **FR-003**: Every new or updated API response DTO introduced by this feature MUST use the `Dto` suffix.
- **FR-004**: Controllers MUST return response DTOs only and MUST NOT return domain entities directly.
- **FR-005**: The system MUST map domain entities to response DTOs before data is returned to the client.
- **FR-006**: API responses that include management status MUST serialize the value as a readable string such as `Pending`, `InProgress`, or `Completed`.
- **FR-007**: API requests that include management status MUST accept the readable string forms of the supported status values.
- **FR-008**: The system MUST convert valid incoming management status strings to the internal enum type before business processing.
- **FR-009**: Invalid management status input values MUST return the existing validation error response format.
- **FR-010**: The system MUST keep status parsing and serialization behavior consistent across all affected endpoints and DTOs.
- **FR-011**: The system MUST add application logging through the existing .NET logging abstractions rather than ad hoc logging mechanisms.
- **FR-012**: The system MUST write informational logs for successful main operations.
- **FR-013**: The system MUST write warning logs for expected validation or not-found scenarios.
- **FR-014**: The system MUST write error logs for unexpected exceptions.
- **FR-015**: Logging changes MUST preserve the existing public error responses and MUST NOT alter the established validation pipeline behavior.
- **FR-016**: The system MUST keep the changes simple and consistent with the current architecture, patterns, and conventions.
- **FR-017**: The system MUST include automated unit test coverage for DTO-only responses, status string request/response behavior, invalid status validation, and logging behavior where applicable.

## Constitution Alignment *(mandatory)*

- **Boundary Impact**: The feature affects controller return contracts, API mapping behavior, request/response model handling for management status, and application-level logging while keeping business rules in Domain/Application and persistence in Infrastructure.
- **Testing Impact**: Coverage is required for response DTO mapping, string-based status serialization and input parsing, invalid status validation responses, and informational, warning, and error logging behavior at the affected boundaries.
- **Contract Impact**: Public API contracts change from numeric management status values to readable strings and require explicit DTO response models for all affected endpoints.
- **Documentation Impact**: Delivery requires updated API contract documentation, status value examples, and notes describing the logging expectations for success, expected failures, and unexpected exceptions.

### Key Entities *(include if feature involves data)*

- **Response DTO Contract**: The explicit transport-focused response model returned from an endpoint instead of a domain entity.
- **Management Status API Value**: The readable request and response representation of the internal management status enum.
- **Domain-to-DTO Mapping**: The translation step that converts internal domain objects to API-safe response DTOs before the controller returns them.
- **Application Log Event**: A structured informational, warning, or error record emitted through the existing logging abstractions for a meaningful API-related operation or outcome.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of affected endpoints return explicit response DTOs and no endpoint returns a domain entity directly.
- **SC-002**: 100% of affected API responses serialize management status as readable strings rather than numeric values.
- **SC-003**: 100% of affected API requests accept valid readable management status values and reject invalid values with the existing validation error response.
- **SC-004**: Successful main operations, expected validation or not-found outcomes, and unexpected exceptions each produce logs at their intended severity levels.
- **SC-005**: A client can understand and use the updated status contract from the API documentation and response examples without inspecting source code.

## Assumptions

- The set of supported management status values remains the existing set already used by the application.
- Existing request and response DTOs can be extended or replaced as needed without widening feature scope beyond the affected API contract surfaces.
- The current global validation and exception handling behavior remains the source of truth for invalid status responses and unexpected error payloads.
- Logging should be added at the existing application and API boundaries without introducing a new observability framework or external logging dependency.
