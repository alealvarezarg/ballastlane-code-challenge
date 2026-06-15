# Research: Management UI

## Decision 1: Use standalone Angular bootstrap with functional providers

- **Decision**: Build the UI with standalone components, `app.config.ts`, `provideRouter`, `provideHttpClient`, and functional guards/interceptors/providers rather than NgModule-centric composition.
- **Rationale**: Angular v22 documentation centers routing and dependency injection around standalone configuration and provider functions, which keeps the application bootstrap explicit and aligned with the requested Angular standards. Functional guards also fit the protected-route requirement cleanly because Angular's injection context supports `inject()` inside guard functions.
- **Alternatives considered**:
  - Reintroducing root and feature NgModules: rejected because it adds structure the requested Angular style no longer needs.
  - Class-based guards and interceptor registration everywhere: rejected because functional providers are simpler for new code and keep dependencies local to the route or provider setup.

## Decision 2: Use route-level lazy loading for `auth` and `tasks`

- **Decision**: Define the application routes in `app.routes.ts` and lazy-load the auth and tasks feature areas, with the authenticated shell and task workspace behind a guard.
- **Rationale**: Angular routing guidance for v22 continues to emphasize routes as the primary navigation unit and provider entry point. Lazy loading keeps the public auth experience lightweight while isolating the protected task feature for cleaner ownership and smaller initial loads.
- **Alternatives considered**:
  - Eager-loading all routes in one bundle: rejected because the app has a clean split between public and protected flows.
  - Excessive route fragmentation by screen or widget: rejected because the current scope is too small to justify micro-feature routing.

## Decision 3: Use Angular Material as the primary UI system with custom theming

- **Decision**: Use Angular Material and CDK for layout primitives, form fields, dialogs, snack bars, navigation chrome, and accessible interaction states, with a small app-specific theme layer for branding and spacing consistency.
- **Rationale**: Angular Material provides a cohesive, accessible component system that matches the requirement for a professional, responsive, and consistent UI while staying close to the Angular ecosystem.
- **Alternatives considered**:
  - Building a custom component library from scratch: rejected because it adds cost and accessibility risk without product value for this scope.
  - Mixing multiple UI libraries: rejected because it would weaken consistency and complicate styling.

## Decision 4: Use NgRx selectively for shared domain state and signals for local view state

- **Decision**: Use NgRx Store, Effects, Entity, and Router Store for auth session state and the task collection/query workflow, while using Angular signals for local derivations such as filter form summaries, dialog state, and view-only computed values.
- **Rationale**: The requested architecture calls for NgRx, but the feature scope does not justify putting every UI toggle into global state. Angular signals are intended for granular reactive state, and NgRx remains the right fit where events, async orchestration, and cross-screen state matter.
- **Alternatives considered**:
  - Using only signals and skipping NgRx: rejected because the user explicitly requested NgRx and the task/auth flows benefit from centralized event handling.
  - Storing all UI state in NgRx: rejected because it would add ceremony and reduce readability for ephemeral component concerns.

## Decision 5: Mirror backend validation rules in typed frontend validators

- **Decision**: Implement typed client-side validators for sign-up, login, task create, task update, patch, status update, and task-query inputs using the backend validator behavior as the reference source.
- **Rationale**: The backend already defines concrete validation rules for required values, email format, password length, valid status values, non-default due dates, valid identifiers, sortable fields, page/page-size constraints, and patch minimum-field behavior. Mirroring those rules client-side improves responsiveness while still allowing backend error responses to win.
- **Alternatives considered**:
  - Relying only on backend validation: rejected because the spec requires immediate user feedback.
  - Adding stronger client-only validation than the API enforces: rejected because the UI must not invent unsupported behavior.

## Decision 6: Centralize HTTP auth and API error normalization in interceptors plus feature-aware mappers

- **Decision**: Use a small interceptor stack to attach bearer tokens, normalize API error payloads, and react consistently to authorization failures, while leaving feature-specific presentation mapping in auth/task services and state effects.
- **Rationale**: The backend uses a consistent error DTO shape with `statusCode`, `message`, `errors`, and `traceId`, which makes a shared normalization step straightforward. Keeping presentation mapping outside the interceptor avoids a god-object network layer.
- **Alternatives considered**:
  - Handling every error separately in each component: rejected because it would duplicate logic and weaken consistency.
  - Moving all feature logic into interceptors: rejected because that hides domain-specific user feedback behind transport code.

## Decision 7: Use DTO-aligned frontend models with thin mapping helpers

- **Decision**: Define frontend models that align one-to-one with request and response DTO contracts, plus small UI-only view-state wrappers where needed for form defaults, filter persistence, and display formatting.
- **Rationale**: The spec forbids dependency on domain entities. DTO-aligned client models keep API boundaries explicit and reduce accidental leakage of backend-only concepts into the UI.
- **Alternatives considered**:
  - Reusing backend domain names and richer domain semantics in the UI: rejected because it would blur the boundary the spec explicitly protects.
  - Mapping every endpoint into a heavily abstracted client SDK: rejected because the current API surface is small and readable enough without that layer.

## Decision 8: Test the UI through Angular-focused unit and integration-style component tests

- **Decision**: Use Angular TestBed for components and services, HttpTestingController for API services and interceptors, RouterTestingHarness for route protection and navigation, and NgRx testing utilities for reducers, selectors, and effects.
- **Rationale**: This stack covers the main user flows required by the spec while staying close to Angular's supported testing surface and the repository's TDD expectations.
- **Alternatives considered**:
  - Relying mostly on end-to-end browser tests: rejected because the spec emphasizes unit coverage and the repo culture is test-first.
  - Testing only reducers and services while skipping routed page behavior: rejected because protected navigation and feedback states are central to the feature.
