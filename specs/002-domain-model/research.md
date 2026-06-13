# Research: Domain Model

## Decision 1: Keep validation inside entity behavior

- **Decision**: Enforce required-field and state validation directly inside the `ManagementTask` and `ManagementUser` entities.
- **Rationale**: The feature is explicitly scoped to the Domain project and requires entities to prevent invalid state without relying on outer layers.
- **Alternatives considered**:
  - External validators: rejected because they weaken entity invariants and add unnecessary indirection.
  - Application-layer validation only: rejected because invalid state could still leak into domain objects.

## Decision 2: Use explicit domain methods instead of public setters

- **Decision**: Expose dedicated behavior methods for task updates and task status changes.
- **Rationale**: This keeps state transitions intentional and ensures every change path runs through validation.
- **Alternatives considered**:
  - Public property setters: rejected because they allow bypassing invariants.
  - Separate command objects: rejected because they add abstraction without current value.

## Decision 3: Treat due date validity as a non-default date requirement

- **Decision**: Consider a due date valid when it is a real, non-default date value.
- **Rationale**: The specification requires validation but does not define business-calendar rules, so the simplest enforceable rule is sufficient for this iteration.
- **Alternatives considered**:
  - Requiring future dates only: rejected because the specification does not require it.
  - Allowing default dates: rejected because it does not represent meaningful task scheduling data.

## Decision 4: Skip interface contracts for this feature

- **Decision**: Do not create `contracts/` artifacts for this feature.
- **Rationale**: The feature is internal to the Domain layer and does not expose APIs, CLI commands, or other external interfaces.
- **Alternatives considered**:
  - Creating placeholder contracts: rejected because they would add noise and imply boundaries that do not exist.

## Decision 5: Treat identifiers as required domain values

- **Decision**: Reject empty identifiers for `ManagementTask`, `ManagementUser`, and `ManagementTask.UserId`.
- **Rationale**: The model must prevent invalid state, and empty identifiers do not represent a valid entity or owner reference.
- **Alternatives considered**:
  - Allowing empty identifiers during construction: rejected because it weakens entity integrity and defers core invariants to outer layers.
