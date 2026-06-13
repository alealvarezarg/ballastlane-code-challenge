# Constitution

Canonical governance lives in [.specify/memory/constitution.md](.specify/memory/constitution.md).
This root file mirrors the same rules for contributors who look here first.

## Purpose

Build maintainable software that prioritizes clarity, simplicity, testability, and business
value over unnecessary complexity.

## I. Clean Architecture Boundaries

- Business rules must remain independent from frameworks, transport layers, and persistence.
- Dependencies must point inward toward application and domain code.
- Controllers and endpoints must orchestrate requests and responses only.
- Infrastructure choices must be replaceable without changing core business behavior.

**Rationale:** Stable business rules keep the system maintainable as technology choices evolve.

## II. Business Logic Separation

- Business rules belong in the application and domain layers.
- Validation and business decisions must not live inside controllers or repositories.
- Data access components must focus on persistence concerns only.
- Shared policies must be modeled once instead of duplicated across layers.

**Rationale:** Separation of concerns improves maintainability, clarity, and testing.

## III. Test-Backed Delivery

- New or changed business behavior must be backed by automated tests before completion.
- Business logic must be covered by unit tests.
- Integration or API tests must be added when work crosses HTTP, persistence, or service boundaries.
- Tests should verify observable behavior instead of implementation details.

**Rationale:** Tests provide confidence, support refactoring, and protect against regressions.

## IV. Simplicity Before Abstraction

- Prefer the simplest solution that satisfies the current requirement.
- Avoid unnecessary abstractions, patterns, or generic frameworks.
- Do not add code for hypothetical future requirements.
- Record the reason whenever a more complex design is chosen over a simpler one.

**Rationale:** Simpler code is easier to understand, change, and maintain.

## V. Explicit Contracts and Dependencies

- Avoid hidden dependencies and global state.
- Use dependency injection for external services and infrastructure collaborators.
- Public APIs must use consistent request and response models.
- Validate input and return appropriate HTTP status codes.
- AI-generated code must always be reviewed, understood, and validated through tests.

**Rationale:** Explicit contracts improve modularity, predictability, and code quality.

## VI. Documentation and Code Quality

- Code must be self-explanatory before relying on comments.
- Methods, classes, and services must have one clear responsibility.
- Remove dead code and duplication instead of documenting around them.
- Every deliverable should include setup instructions, an architecture overview, and important
  assumptions or seeded access details when needed.

**Rationale:** Readable code and practical documentation reduce onboarding and maintenance costs.

## Governance

Any change that increases complexity without clear business value should be rejected or explicitly
justified. Reviews must verify architecture boundaries, testing obligations, dependency clarity,
and documentation updates.

When multiple solutions exist, prefer the one that is:

1. Simpler
2. Easier to test
3. Easier to maintain
4. More aligned with Clean Architecture principles
