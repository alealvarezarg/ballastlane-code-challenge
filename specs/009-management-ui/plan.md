# Implementation Plan: Management UI

**Branch**: `009-management-ui` | **Date**: 2026-06-14 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/009-management-ui/spec.md`

## Summary

Build a new Angular 22.0.1 frontend in the existing `ui/` workspace that exposes only the current backend capabilities: public sign-up and login, protected management-task workflows, and consistent feedback around loading, success, validation, authorization, conflict, and unexpected API failures. The design uses standalone components, Angular Material, route-level lazy loading, functional providers, and a selective NgRx architecture that keeps global auth/task state centralized while using signals for local view derivation and low-complexity UI state.

## Technical Context

**Language/Version**: TypeScript with Angular 22.0.1

**Primary Dependencies**: Angular Router, Angular HttpClient, Angular Material, Angular CDK, NgRx Store, NgRx Effects, NgRx Entity, NgRx Router Store, RxJS

**Storage**: Browser storage for authenticated session persistence; no client-owned business data storage beyond cached in-memory state

**Testing**: Angular TestBed, HttpTestingController, RouterTestingHarness, NgRx testing utilities, component and route-guard unit tests run through the Angular CLI test workflow

**Target Platform**: Modern desktop and mobile web browsers used against the existing .NET API

**Project Type**: Single-page web application

**Performance Goals**: Preserve a fast local-development experience, keep route transitions and common task interactions responsive, and avoid unnecessary client-side state duplication or full-page reload patterns

**Constraints**: Must consume only current REST DTO contracts; must not depend on domain entities; must keep login and sign-up public while guarding every task route; must mirror backend validation rules without replacing backend truth; must use standalone components, Angular Material, NgRx, functional providers where appropriate, and lazy loading where it adds value; must defer the requested users list screen until the backend supports it

**Scale/Scope**: New UI workspace covering app shell, public auth flow, protected task workspace, shared feedback patterns, route protection, API integration, and automated unit coverage for the main flows

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Domain and application behavior stays isolated from frameworks, transport, and persistence.
- Business rules, validation, and use-case decisions live outside controllers, handlers, and repositories.
- Required automated tests are identified for every changed business behavior and boundary crossing.
- Simpler designs were preferred; any added abstraction has an explicit justification.
- Dependency injection, API contract consistency, and documentation updates are accounted for.

**Gate Status**: Pass. The UI feature adds a separate presentation boundary under `ui/` while keeping backend behavior authoritative. The plan uses explicit DTO mapping, API-focused services, and narrowly-scoped NgRx slices rather than speculative cross-cutting abstractions.

## Project Structure

### Documentation (this feature)

```text
specs/009-management-ui/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- ui-contract.md
`-- tasks.md
```

### Source Code (repository root)

```text
api/
|-- src/
|   |-- TaskManagementSystem.Api/
|   |-- TaskManagementSystem.Application/
|   |-- TaskManagementSystem.Domain/
|   `-- TaskManagementSystem.Infrastructure/
`-- tests/

ui/
|-- src/
|   |-- app/
|   |   |-- app.component.ts
|   |   |-- app.config.ts
|   |   |-- app.routes.ts
|   |   |-- core/
|   |   |   |-- guards/
|   |   |   |-- interceptors/
|   |   |   |-- providers/
|   |   |   `-- services/
|   |   |-- features/
|   |   |   |-- auth/
|   |   |   |   |-- pages/
|   |   |   |   |-- components/
|   |   |   |   |-- services/
|   |   |   |   `-- state/
|   |   |   `-- tasks/
|   |   |       |-- pages/
|   |   |       |-- components/
|   |   |       |-- services/
|   |   |       |-- models/
|   |   |       `-- state/
|   |   |-- shared/
|   |   |   |-- components/
|   |   |   |-- models/
|   |   |   |-- ui/
|   |   |   `-- utils/
|   |   `-- styles/
|   |-- assets/
|   `-- styles.scss
|-- public/
`-- angular.json / package.json / tsconfig*.json
```

**Structure Decision**: Keep the existing backend untouched in `api/` and create the new Angular application inside `ui/`. Within the UI, favor a feature-first structure with a small `core/` area for app-wide concerns, colocated feature state and API services, and `shared/` only for genuinely reusable presentation or utility elements. This preserves scalability without introducing a generic multi-package frontend architecture the current scope does not need.

## Phase Plan

### Phase 0

- Confirm Angular 22 standalone bootstrapping, functional provider, routing, and signals guidance to keep the app aligned with current Angular patterns.
- Confirm the minimal NgRx slice layout that supports auth session state, task collection/query state, and shared request feedback without pushing every local UI concern into the store.
- Confirm Angular Material usage patterns for responsive shell layout, forms, table/list presentation, dialogs, snack bars, and accessible page structure.
- Confirm how backend validation and authorization errors should map into reusable frontend feedback utilities and interceptors.
- Confirm a low-complexity testing strategy for routes, guards, API services, form validation, NgRx effects, and main page flows.

### Phase 1

- Design the UI-facing data model around DTO contracts, query state, session state, and feedback state.
- Design the route map and lazy-loaded feature boundaries for public auth routes and protected task routes.
- Design the NgRx slices, selectors, actions, and effect boundaries for auth and tasks.
- Design the shared API contract and error-handling patterns, including interceptor responsibilities and guard behavior.
- Design the validation mirroring rules so the UI provides immediate feedback while preserving backend response mapping.
- Update the agent context to point to this UI plan.

### Phase 2

- Implement in TDD order:
  1. UI workspace scaffolding, standalone bootstrap, router, theming, and app shell tests.
  2. Auth API services, route guards, interceptors, and auth-state tests.
  3. Public login and sign-up pages with validation and feedback tests.
  4. Task state, API services, query handling, and task list retrieval tests.
  5. Task create, edit, patch/status change, and archive flows with page and component tests.
  6. Shared error, loading, and success feedback integration tests across auth and tasks.
- Keep refactors inside each slice after tests pass and before moving to the next behavior.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |
