# Research: Initial Project Scaffolding

## Decision: Use .NET 10 across all backend projects

- **Rationale**: Keeps the solution consistent and avoids framework drift between layers.
- **Alternatives considered**: Per-project version selection.

## Decision: Use four backend projects

- **Rationale**: Cleanly separates domain, application, infrastructure, and delivery concerns.
- **Alternatives considered**: Fewer projects with mixed responsibilities.

## Decision: Use xUnit, FakeItEasy, and Shouldly for unit tests

- **Rationale**: Covers test runner, fakes, and expressive assertions from the first implementation step.
- **Alternatives considered**: xUnit only, or delaying helper libraries until later.

## Decision: Start unit testing in inner layers first

- **Rationale**: Domain and Application will hold the earliest core behavior and benefit most from fast feedback.
- **Alternatives considered**: A single shared test project or full test coverage across all layers immediately.
