<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan:
`specs/009-management-ui/plan.md`
<!-- SPECKIT END -->

# Task Management System Agent Guide

This repository contains a .NET 10 backend and an Angular 22 frontend. Treat the codebase as a Clean Architecture solution with a separate web UI that consumes the API through DTO contracts only.

## Architecture Overview

- `api/src/TaskManagementSystem.Domain`: core business entities, enums, and domain exceptions.
- `api/src/TaskManagementSystem.Application`: use-case orchestration, repository abstractions, service contracts, caching, and cross-layer business coordination.
- `api/src/TaskManagementSystem.Infrastructure`: LiteDB persistence, repository implementations, JWT token generation, password hashing, and seed initialization.
- `api/src/TaskManagementSystem.Api`: HTTP delivery layer, DTOs, controllers, validation, authentication wiring, exception handling, Swagger, and CORS.
- `ui/src/app`: standalone Angular application that consumes the API through request/response models, route guards, interceptors, NgRx state, and feature components.

## Folder Responsibilities

- `.agents/`: local agent skills and supporting automation.
- `.prompts/`: prompt assets used during repository workflows.
- `.specify/`: spec workflow assets.
- `api/src/`: production backend code grouped by architecture layer.
- `api/tests/`: backend unit tests grouped by layer.
- `openapi/`: repository-level OpenAPI artifact for the current API contract snapshot.
- `specs/`: feature specifications, research, plans, tasks, quickstarts, contracts, and checklists.
- `ui/src/app/core/`: app-wide Angular concerns such as guards, interceptors, providers, and session/notification services.
- `ui/src/app/features/`: feature-first UI organization for `auth` and `tasks`.
- `ui/src/app/shared/`: reusable UI models, utilities, and cross-feature presentation components.
- `ui/src/app/styles/`: frontend theme primitives.

## Coding Conventions

- Keep changes scoped to the layer that owns the behavior.
- Prefer explicit DTOs, typed models, and named mappings over ad hoc object shaping.
- Keep controllers thin. They should translate HTTP input/output, not contain business rules.
- Keep Angular pages focused on composition and orchestration; move API access into services and state transitions into NgRx effects/reducers.
- Reuse existing naming patterns:
  - Backend DTOs end with `Dto`.
  - Backend dependency registration lives in `DependencyInjection.cs` or `ApiServiceCollectionExtensions.cs`.
  - Angular state uses `actions`, `reducer`, `selectors`, and `effects` files per feature.
- Follow the existing test-first style when adding or changing behavior.

## Clean Architecture Rules

- `Domain` must not depend on any other project.
- `Application` may depend on `Domain` only.
- `Infrastructure` may depend on `Application` and `Domain`, but not on `Api` or `ui`.
- `Api` may depend on `Application` and `Infrastructure`; it should never bypass abstractions for business behavior.
- `ui` must depend on API DTO-style contracts and frontend view models only. It must not reference backend domain concepts directly.

## Boundary Protection Rules

- Do not place validation, lifecycle rules, authorization decisions, or persistence policy inside controllers, Angular components, or repositories unless that concern already belongs there.
- Do not expose domain entities directly from the API.
- Do not move repository implementations out of `Infrastructure`.
- Do not add frontend behavior that invents backend capabilities.
- Do not let the UI become the source of truth for business rules; mirrored validation is allowed, but backend responses remain authoritative.

## Testing Expectations

- Backend changes should include or update unit tests in the matching `api/tests/*` project.
- Frontend changes should include or update Angular unit tests for validators, guards, reducers, services, or components when behavior changes.
- Prefer targeted test runs while iterating, then run the relevant suite before finishing.
- Current commonly used commands:
  - `dotnet restore TaskManagementSystem.slnx`
  - `dotnet test tests/TaskManagementSystem.Api.UnitTests/TaskManagementSystem.Api.UnitTests.csproj --no-restore`
  - `npm test`

## Documentation Standards

- Document only what is currently implemented in code.
- When specs and code diverge, call out the gap explicitly instead of describing planned behavior as complete.
- Update `README.md`, `QUICKSTART.md`, `USERCASES.md`, and relevant spec-adjacent docs when behavior, setup, or project structure changes.
- Keep onboarding documentation practical: include ports, commands, seed behavior, and any non-obvious constraints.

## Spec-Driven Delivery Rule

- Only implement functionality that is defined in the `specs/` folder and supported by the current agreed scope.
- If a requested feature is not defined in `specs/`, pause and clarify before implementing it.
- If a spec exists but the code does not yet implement it, document it as not implemented rather than silently filling the gap.
