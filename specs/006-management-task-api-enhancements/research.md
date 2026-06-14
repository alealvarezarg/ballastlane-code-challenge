# Research: ManagementTask API Enhancements

## Decision 1: Keep the controller thin and move new task rules into Domain/Application

- **Decision**: Continue using one `ManagementTaskController`, but place filtering orchestration, lifecycle checks, idempotent create handling, and cache invalidation in the Application and Domain layers.
- **Rationale**: The existing solution already uses a thin controller boundary, and extending that pattern preserves the constitution requirement that delivery code stays free of business logic.
- **Alternatives considered**:
  - Moving lifecycle checks into controller actions: rejected because it duplicates business rules in the HTTP layer.
  - Pushing rule enforcement into repositories: rejected because persistence code should not decide editability, transition, or deletion policies.

## Decision 2: Use query parameters on `GET /management-tasks` for combined filtering, search, sorting, pagination, and active scope

- **Decision**: Keep the main list endpoint as `GET /management-tasks` and add query parameters for `status`, `userId`, `search`, `sortBy`, `sortDirection`, `page`, `pageSize`, and `includeArchived`.
- **Rationale**: One list endpoint with composable query parameters is the simplest way to support combined filters without proliferating routes.
- **Alternatives considered**:
  - Separate routes for every filter combination: rejected because the route count grows quickly and becomes harder to document.
  - POST-based search endpoints: rejected because retrieval remains a read concern and fits naturally into query parameters.

## Decision 3: Use dedicated read endpoints for summary, overdue, and due-within views

- **Decision**: Add `GET /management-tasks/summary`, `GET /management-tasks/overdue`, and `GET /management-tasks/due-within?days={n}` while allowing standard list filters where relevant.
- **Rationale**: These are distinct client use cases with clear semantics that would be awkward to infer indirectly from a generic list response.
- **Alternatives considered**:
  - Returning summary metadata alongside every list response: rejected because many clients only need items and should not pay for extra response shape complexity.
  - Encoding overdue and due-within as opaque search keywords: rejected because explicit endpoints are clearer and easier to validate.

## Decision 4: Use object-based PATCH instead of JSON Patch

- **Decision**: Support `PATCH /management-tasks/{id}` with a partial DTO containing optional mutable fields rather than RFC 6902 JSON Patch operations.
- **Rationale**: Object-based PATCH is simpler to validate with FluentValidation, easier to keep deterministic, and avoids introducing patch-document parsing complexity for a small API.
- **Alternatives considered**:
  - JSON Patch documents: rejected because they add operational complexity and expand the mutation surface beyond the current need.
  - Treating PUT as partial update: rejected because the feature explicitly requires PATCH support and PUT should remain full replacement semantics.

## Decision 5: Add a dedicated status endpoint with explicit transition rules

- **Decision**: Add `PATCH /management-tasks/{id}/status` that only accepts a target status and validates transitions in the domain/application flow.
- **Rationale**: Status changes are a separate business action and deserve a narrower contract than full or partial updates.
- **Alternatives considered**:
  - Requiring all status changes through full update: rejected because clients should not resubmit unrelated fields to progress a task.
  - Treating status as unrestricted enum replacement: rejected because the feature requires transition validation.

## Decision 6: Model soft delete as archiving metadata instead of a new status

- **Decision**: Introduce archive state on the task record, using fields such as `IsArchived` and `ArchivedAt`, while keeping workflow status separate.
- **Rationale**: Archiving is a retention concern rather than a business-progress state, and keeping it separate avoids overloading the status enum.
- **Alternatives considered**:
  - Adding an `Archived` status value: rejected because it mixes lifecycle progression with retention visibility.
  - Physically deleting records and reconstructing history elsewhere: rejected because the feature explicitly requires soft delete.

## Decision 7: Default workflow rules to simple, explicit transitions and restrictions

- **Decision**: Use these defaults for the enhancement unless later requirements override them: `Pending -> InProgress`, `InProgress -> Completed`, `Pending -> Completed` allowed, no transitions out of `Completed`, `Completed` is non-editable, and archived tasks are non-editable. Deletion is allowed only while the task is not completed.
- **Rationale**: These defaults align with the current three-status model and create deterministic rules without inventing extra states.
- **Alternatives considered**:
  - Allowing free movement between all statuses: rejected because it conflicts with the requirement to validate transitions.
  - Adding more statuses solely to express deletion/edit rules: rejected because it expands the model beyond the current feature scope.

## Decision 8: Use a persisted idempotency key for create requests

- **Decision**: Accept an `Idempotency-Key` header on create requests, store the key with a request fingerprint and resulting task identifier, return the original successful result on exact repeats, and return a conflict response when the same key is reused with different payload data.
- **Rationale**: Persisting the key keeps duplicate protection stable beyond a single process lifetime and makes behavior deterministic for clients.
- **Alternatives considered**:
  - In-memory-only idempotency tracking: rejected because the protection would disappear across restarts.
  - Using a payload hash alone: rejected because distinct legitimate creates could share identical content without an explicit client intent to deduplicate.

## Decision 9: Use coarse-grained read-cache invalidation

- **Decision**: Keep caching in the Application layer and invalidate all management-task read caches after any successful write, including create, update, patch, status change, or archive.
- **Rationale**: Coarse invalidation is simple, safe, and sufficient for the current scale while still avoiding repeated reads between writes.
- **Alternatives considered**:
  - Fine-grained invalidation per query combination: rejected because the key management complexity is not justified for the feature scope.
  - Removing caching from enhanced reads: rejected because the specification explicitly asks to leverage in-memory caching where applicable.

## Decision 10: Make TDD an explicit implementation rule for every story slice

- **Decision**: Implement each behavior in red-green-refactor order, starting with failing tests in the lowest layer that owns the rule, then adding API tests for the exposed contract, and only then writing production code.
- **Rationale**: The user explicitly requires TDD, and the repository constitution already requires test-backed delivery.
- **Alternatives considered**:
  - Writing implementation first and backfilling tests: rejected because it violates the stated delivery requirement.
  - Relying only on API tests: rejected because domain and application rules need direct, fast feedback loops as well.
