<!--
Sync Impact Report
- Version change: template -> 1.0.0
- Modified principles:
  - Template Principle 1 -> I. Clean Architecture Boundaries
  - Template Principle 2 -> II. Business Logic Separation
  - Template Principle 3 -> III. Test-Backed Delivery
  - Template Principle 4 -> IV. Simplicity Before Abstraction
  - Template Principle 5 -> V. Explicit Contracts and Dependencies
- Added sections:
  - Operational Constraints
  - Delivery Workflow
- Removed sections:
  - None
- Templates requiring updates:
  - updated .specify/templates/plan-template.md
  - updated .specify/templates/spec-template.md
  - updated .specify/templates/tasks-template.md
- Follow-up TODOs:
  - None
-->
# TaskManagementSystem Constitution

## Core Principles

### I. Clean Architecture Boundaries
Business rules MUST remain independent from frameworks, transport layers, and persistence
details. Dependencies MUST point inward toward application and domain code, and controllers or
endpoints MUST only translate requests, responses, and orchestration concerns. Infrastructure
choices MUST be replaceable without rewriting core business behavior.

Rationale: Stable business rules keep the system maintainable as tooling, storage, or delivery
mechanisms change.

### II. Business Logic Separation
Business decisions, validation rules, and use-case coordination MUST live in the
application/domain layers. Repositories MUST focus on persistence concerns only, and delivery
layers MUST NOT contain business rules. Shared policies MUST be modeled once in the domain
instead of duplicated across controllers, handlers, or data access code.

Rationale: Clear separation of concerns improves testability, reduces duplication, and makes
changes safer.

### III. Test-Backed Delivery
New or changed business behavior MUST be backed by automated tests before work is considered
complete. Unit tests MUST cover domain and application rules, and integration or API tests MUST
be added whenever a change crosses process, persistence, or HTTP boundaries. Tests MUST verify
observable behavior rather than implementation details, and all required tests MUST pass before
merge.

Rationale: Test-backed delivery provides confidence, supports refactoring, and prevents regressions.

### IV. Simplicity Before Abstraction
The team MUST choose the simplest design that satisfies the current requirement. New abstractions,
patterns, or generic frameworks MUST be justified by an active need, not a hypothetical future
use case. When a simpler option is rejected, the implementation plan MUST record why.

Rationale: Avoiding speculative complexity keeps the codebase easier to understand and maintain.

### V. Explicit Contracts and Dependencies
Dependencies MUST be explicit, composable, and reviewable. External services and infrastructure
collaborators MUST be injected rather than hidden behind globals or ambient state. Public APIs
MUST use consistent request/response models, clear validation, and appropriate HTTP status codes.
AI-generated code MAY accelerate delivery, but it MUST be reviewed, understood, and validated by
tests before adoption.

Rationale: Explicit contracts reduce coupling, improve API predictability, and keep human judgment
in the loop for code quality.

## Operational Constraints

- Every deliverable MUST include setup instructions, seeded demo data or credentials when needed,
  an architecture overview, and the assumptions or design decisions required to operate it.
- Code MUST be readable without depending on excessive comments; names, structure, and boundaries
  MUST carry the explanation.
- Classes, services, and methods MUST keep a single clear responsibility, and dead code or
  duplication MUST be removed instead of documented away.

## Delivery Workflow

- Plans MUST complete a constitution check before implementation begins and again after design
  changes.
- Specs MUST document architecture boundary impacts, testing expectations, API contract changes,
  and documentation obligations when relevant.
- Tasks MUST include the work needed to satisfy tests, API consistency, dependency wiring, and
  documentation updates for the affected story or feature.
- Pull requests and reviews MUST verify constitution compliance, especially around business logic
  placement, unnecessary abstraction, and missing tests.

## Governance

This constitution overrides conflicting local practices for this repository. Amendments MUST be
documented in the constitution, reviewed by the maintainers, and accompanied by updates to any
affected templates or workflow guidance before they are considered adopted.

Versioning policy follows semantic versioning for governance changes:
- MAJOR: Removes or redefines a principle in a backward-incompatible way.
- MINOR: Adds a principle, section, or materially stronger guidance.
- PATCH: Clarifies wording without changing expected behavior.

Compliance review is mandatory during planning, implementation, and code review. Changes that add
complexity without clear business value, weaken architectural boundaries, or bypass required tests
MUST be rejected or explicitly justified in the implementation plan.

**Version**: 1.0.0 | **Ratified**: 2026-06-13 | **Last Amended**: 2026-06-13
