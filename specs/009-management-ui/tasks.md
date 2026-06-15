# Tasks: Management UI

**Input**: Design documents from `/specs/009-management-ui/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md,
data-model.md, contracts/

**Tests**: Include the test tasks required by the constitution for any changed business behavior or
boundary-crossing integration. Do not omit required tests.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing
of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Frontend app: `ui/src/`
- Frontend tests: `ui/src/**/*.spec.ts`
- Feature docs: `specs/009-management-ui/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Initialize the Angular workspace, shared toolchain, and base UI project structure

- [X] T001 Scaffold the Angular 22.0.1 workspace and core config files in `ui/angular.json`, `ui/package.json`, `ui/tsconfig.json`, `ui/tsconfig.app.json`, and `ui/tsconfig.spec.json`
- [X] T002 Create the standalone app bootstrap and root shell files in `ui/src/main.ts`, `ui/src/app/app.config.ts`, `ui/src/app/app.component.ts`, `ui/src/app/app.component.html`, and `ui/src/app/app.routes.ts`
- [X] T003 [P] Configure Angular Material theming, global styles, and responsive layout tokens in `ui/src/styles.scss` and `ui/src/app/styles/_theme.scss`
- [X] T004 [P] Create the feature-first directory skeleton with placeholder index files under `ui/src/app/core/`, `ui/src/app/features/auth/`, `ui/src/app/features/tasks/`, and `ui/src/app/shared/`
- [X] T005 [P] Configure frontend test tooling and scripts for Angular CLI, Karma, and coverage in `ui/package.json`, `ui/angular.json`, and `ui/tsconfig.spec.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**CRITICAL**: No user story work can begin until this phase is complete

- [X] T006 Create shared API environment and injection tokens in `ui/src/app/core/providers/api-config.provider.ts`, `ui/src/app/shared/models/app-environment.model.ts`, and `ui/src/environments/`
- [X] T007 [P] Define DTO-aligned shared models for auth, tasks, pagination, and API errors in `ui/src/app/shared/models/auth.models.ts`, `ui/src/app/shared/models/task.models.ts`, and `ui/src/app/shared/models/api-error.models.ts`
- [X] T008 [P] Implement shared mapping and validation utility helpers in `ui/src/app/shared/utils/api-error.mapper.ts`, `ui/src/app/shared/utils/date-format.util.ts`, and `ui/src/app/shared/utils/task-query.util.ts`
- [X] T009 Implement core auth persistence and session services in `ui/src/app/core/services/session-storage.service.ts` and `ui/src/app/core/services/auth-session.service.ts`
- [X] T010 [P] Implement functional auth guard and route redirect helpers in `ui/src/app/core/guards/auth.guard.ts` and `ui/src/app/core/guards/public-only.guard.ts`
- [X] T011 [P] Implement HTTP interceptors for auth headers and normalized API error handling in `ui/src/app/core/interceptors/auth.interceptor.ts` and `ui/src/app/core/interceptors/api-error.interceptor.ts`
- [X] T012 [P] Implement shared feedback infrastructure with Material snack bars and reusable page-state components in `ui/src/app/core/services/notification.service.ts`, `ui/src/app/shared/components/page-state/page-state.component.ts`, and `ui/src/app/shared/components/inline-feedback/inline-feedback.component.ts`
- [X] T013 Configure root providers for router, HttpClient, animations, store, effects, and router-store in `ui/src/app/app.config.ts`
- [ ] T014 Document frontend setup, dependency wiring, and deferred `/users` scope note in `specs/009-management-ui/quickstart.md` and `specs/009-management-ui/contracts/ui-contract.md`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Sign Up and Access the Application (Priority: P1) MVP

**Goal**: Deliver public sign-up and login flows that establish an authenticated session and gate protected navigation correctly

**Independent Test**: Open the app while signed out, validate public-only access to `/login` and `/sign-up`, submit invalid and valid credentials, and confirm successful login redirects to `/tasks` while protected routes remain guarded

### Tests for User Story 1

- [X] T015 [P] [US1] Add auth session and reducer tests in `ui/src/app/features/auth/state/auth.reducer.spec.ts`
- [ ] T016 [P] [US1] Add auth effects tests for sign-up, login, and auth recovery in `ui/src/app/features/auth/state/auth.effects.spec.ts`
- [X] T017 [P] [US1] Add API service tests for sign-up and login endpoints in `ui/src/app/features/auth/services/auth-api.service.spec.ts`
- [X] T018 [P] [US1] Add route guard and navigation tests in `ui/src/app/core/guards/auth.guard.spec.ts` and `ui/src/app/core/guards/public-only.guard.spec.ts`
- [ ] T019 [P] [US1] Add login and sign-up page component tests in `ui/src/app/features/auth/pages/login-page/login-page.component.spec.ts` and `ui/src/app/features/auth/pages/sign-up-page/sign-up-page.component.spec.ts`

### Implementation for User Story 1

- [X] T020 [P] [US1] Create auth feature models and typed form contracts in `ui/src/app/features/auth/models/auth-form.models.ts`
- [X] T021 [P] [US1] Implement auth validators mirroring backend rules in `ui/src/app/features/auth/validators/login-form.validators.ts` and `ui/src/app/features/auth/validators/sign-up-form.validators.ts`
- [X] T022 [P] [US1] Implement auth API service in `ui/src/app/features/auth/services/auth-api.service.ts`
- [X] T023 [P] [US1] Implement auth actions, reducer, selectors, and effects in `ui/src/app/features/auth/state/auth.actions.ts`, `ui/src/app/features/auth/state/auth.reducer.ts`, `ui/src/app/features/auth/state/auth.selectors.ts`, and `ui/src/app/features/auth/state/auth.effects.ts`
- [X] T024 [US1] Implement reusable auth form shell and field components in `ui/src/app/features/auth/components/auth-form-shell/auth-form-shell.component.ts` and `ui/src/app/features/auth/components/password-field/password-field.component.ts`
- [X] T025 [US1] Implement login and sign-up standalone pages in `ui/src/app/features/auth/pages/login-page/login-page.component.ts`, `ui/src/app/features/auth/pages/login-page/login-page.component.html`, `ui/src/app/features/auth/pages/sign-up-page/sign-up-page.component.ts`, and `ui/src/app/features/auth/pages/sign-up-page/sign-up-page.component.html`
- [X] T026 [US1] Register lazy auth routes and public-only route behavior in `ui/src/app/features/auth/auth.routes.ts` and `ui/src/app/app.routes.ts`
- [X] T027 [US1] Wire auth feature providers and session hydration into app startup in `ui/src/app/features/auth/auth.providers.ts` and `ui/src/app/app.component.ts`
- [ ] T028 [US1] Update story-level validation guidance and auth flow notes in `specs/009-management-ui/quickstart.md`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Manage Tasks Through the Available API Workflows (Priority: P1)

**Goal**: Deliver the protected task workspace with list retrieval, query controls, create/update flows, status changes, archive behavior, and summary/time-based data strictly aligned to the current API

**Independent Test**: Sign in, load `/tasks`, retrieve and filter tasks, create and update a task, change its status, archive it when allowed, and confirm the UI reflects success, validation, and conflict responses correctly

### Tests for User Story 2

- [ ] T029 [P] [US2] Add task reducer and selector tests in `ui/src/app/features/tasks/state/tasks.reducer.spec.ts` and `ui/src/app/features/tasks/state/tasks.selectors.spec.ts`
- [ ] T030 [P] [US2] Add task effects tests for query, summary, overdue, due-within, create, update, patch, status, and archive flows in `ui/src/app/features/tasks/state/tasks.effects.spec.ts`
- [ ] T031 [P] [US2] Add task API service tests for all task endpoints in `ui/src/app/features/tasks/services/tasks-api.service.spec.ts`
- [ ] T032 [P] [US2] Add validator tests for task forms and task query input in `ui/src/app/features/tasks/validators/task-form.validators.spec.ts` and `ui/src/app/features/tasks/validators/task-query.validators.spec.ts`
- [ ] T033 [P] [US2] Add task workspace, task form, and task list component tests in `ui/src/app/features/tasks/pages/tasks-page/tasks-page.component.spec.ts`, `ui/src/app/features/tasks/components/task-form/task-form.component.spec.ts`, and `ui/src/app/features/tasks/components/task-list/task-list.component.spec.ts`

### Implementation for User Story 2

- [X] T034 [P] [US2] Create task feature models for drafts, patch payloads, query state, summaries, and view state in `ui/src/app/features/tasks/models/task-draft.models.ts` and `ui/src/app/features/tasks/models/task-view.models.ts`
- [X] T035 [P] [US2] Implement task validators mirroring backend create, update, patch, status, and query rules in `ui/src/app/features/tasks/validators/task-form.validators.ts` and `ui/src/app/features/tasks/validators/task-query.validators.ts`
- [X] T036 [P] [US2] Implement task API service and request mappers in `ui/src/app/features/tasks/services/tasks-api.service.ts` and `ui/src/app/features/tasks/services/task-request-mapper.service.ts`
- [X] T037 [P] [US2] Implement task actions, reducer, selectors, and effects in `ui/src/app/features/tasks/state/tasks.actions.ts`, `ui/src/app/features/tasks/state/tasks.reducer.ts`, `ui/src/app/features/tasks/state/tasks.selectors.ts`, and `ui/src/app/features/tasks/state/tasks.effects.ts`
- [X] T038 [P] [US2] Implement reusable task query bar, task list, and task summary components in `ui/src/app/features/tasks/components/task-query-bar/task-query-bar.component.ts`, `ui/src/app/features/tasks/components/task-list/task-list.component.ts`, and `ui/src/app/features/tasks/components/task-summary/task-summary.component.ts`
- [X] T039 [P] [US2] Implement task form and status action components in `ui/src/app/features/tasks/components/task-form/task-form.component.ts` and `ui/src/app/features/tasks/components/task-status-menu/task-status-menu.component.ts`
- [X] T040 [US2] Implement the protected tasks page with Material layout, signals-based local view derivations, and store orchestration in `ui/src/app/features/tasks/pages/tasks-page/tasks-page.component.ts` and `ui/src/app/features/tasks/pages/tasks-page/tasks-page.component.html`
- [X] T041 [US2] Register lazy protected task routes and route-level providers in `ui/src/app/features/tasks/tasks.routes.ts` and `ui/src/app/app.routes.ts`
- [X] T042 [US2] Wire task feature providers, facade services, and auth-aware navigation behavior in `ui/src/app/features/tasks/tasks.providers.ts` and `ui/src/app/features/tasks/services/tasks-facade.service.ts`
- [ ] T043 [US2] Update feature documentation for deferred `/users` scope and task workflow validation steps in `specs/009-management-ui/quickstart.md` and `specs/009-management-ui/contracts/ui-contract.md`

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Understand API Outcomes Without Guesswork (Priority: P2)

**Goal**: Make loading, success, validation, authorization, and unexpected error states consistent and accessible across auth and task workflows

**Independent Test**: Exercise successful and failing auth/task requests and confirm the same feedback patterns, message placement, and recovery behavior are used throughout the app

### Tests for User Story 3

- [ ] T044 [P] [US3] Add notification service and normalized API error mapping tests in `ui/src/app/core/services/notification.service.spec.ts` and `ui/src/app/shared/utils/api-error.mapper.spec.ts`
- [ ] T045 [P] [US3] Add interceptor tests for authorization and error normalization behavior in `ui/src/app/core/interceptors/auth.interceptor.spec.ts` and `ui/src/app/core/interceptors/api-error.interceptor.spec.ts`
- [ ] T046 [P] [US3] Add shared feedback component tests in `ui/src/app/shared/components/page-state/page-state.component.spec.ts` and `ui/src/app/shared/components/inline-feedback/inline-feedback.component.spec.ts`
- [X] T047 [P] [US3] Add app-shell feedback integration tests in `ui/src/app/app.component.spec.ts`

### Implementation for User Story 3

- [X] T048 [P] [US3] Refine shared API error normalization and form-error mapping helpers in `ui/src/app/shared/utils/api-error.mapper.ts` and `ui/src/app/shared/utils/form-error-applier.util.ts`
- [X] T049 [P] [US3] Implement reusable loading, empty-state, and error-state presentation variants in `ui/src/app/shared/components/page-state/page-state.component.ts` and `ui/src/app/shared/components/inline-feedback/inline-feedback.component.ts`
- [X] T050 [P] [US3] Implement a shared request-status model and notification integration in `ui/src/app/shared/models/request-status.model.ts` and `ui/src/app/core/services/notification.service.ts`
- [X] T051 [US3] Integrate consistent success, validation, authorization, conflict, and unexpected-error handling into auth flows in `ui/src/app/features/auth/pages/login-page/login-page.component.ts`, `ui/src/app/features/auth/pages/sign-up-page/sign-up-page.component.ts`, and `ui/src/app/features/auth/state/auth.effects.ts`
- [X] T052 [US3] Integrate consistent success, validation, authorization, conflict, and unexpected-error handling into task flows in `ui/src/app/features/tasks/pages/tasks-page/tasks-page.component.ts`, `ui/src/app/features/tasks/components/task-form/task-form.component.ts`, and `ui/src/app/features/tasks/state/tasks.effects.ts`
- [X] T053 [US3] Finalize accessible page titles, route metadata, and shell-level feedback behavior in `ui/src/app/app.routes.ts`, `ui/src/app/app.component.ts`, and `ui/src/app/core/services/app-title.service.ts`

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T054 [P] Add responsive layout and accessibility polish across auth and task features in `ui/src/app/features/auth/**/*.scss`, `ui/src/app/features/tasks/**/*.scss`, and `ui/src/styles.scss`
- [ ] T055 [P] Add remaining store and utility coverage for any gaps found during integration in `ui/src/app/features/**/*.spec.ts` and `ui/src/app/shared/**/*.spec.ts`
- [ ] T056 Verify deferred scope messaging for the test-only `/users` screen in `specs/009-management-ui/spec.md`, `specs/009-management-ui/plan.md`, and `specs/009-management-ui/contracts/ui-contract.md`
- [ ] T057 Run the end-to-end quickstart validation and capture any setup corrections in `specs/009-management-ui/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion and authenticated app shell from US1
- **User Story 3 (Phase 5)**: Depends on US1 and US2 because it normalizes feedback across both flows
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational - establishes the public auth flow and protected navigation baseline
- **User Story 2 (P1)**: Starts after Foundational and uses authenticated session behavior from US1
- **User Story 3 (P2)**: Starts after the auth and tasks flows exist so shared feedback can be applied consistently

### Within Each User Story

- Required tests MUST be written and fail before implementation
- Models and validators before API orchestration
- API services before store effects
- Store wiring before page integration
- Shared component integration before story sign-off
- Documentation updates before story completion

### Parallel Opportunities

- Setup tasks marked `[P]` can run in parallel
- Foundational tasks T007-T012 can run in parallel once T006 is in place
- In US1, tests T015-T019 and implementation tasks T020-T023 can be split across teammates
- In US2, tests T029-T033 and implementation tasks T034-T039 can run in parallel across models, validators, services, and UI components
- In US3, tests T044-T047 and implementation tasks T048-T050 can run in parallel before integration tasks T051-T053

---

## Parallel Example: User Story 1

```text
Task: "Add auth session and reducer tests in ui/src/app/features/auth/state/auth.reducer.spec.ts"
Task: "Add API service tests for sign-up and login endpoints in ui/src/app/features/auth/services/auth-api.service.spec.ts"
Task: "Create auth feature models and typed form contracts in ui/src/app/features/auth/models/auth-form.models.ts"
Task: "Implement auth validators mirroring backend rules in ui/src/app/features/auth/validators/login-form.validators.ts and ui/src/app/features/auth/validators/sign-up-form.validators.ts"
```

## Parallel Example: User Story 2

```text
Task: "Add task reducer and selector tests in ui/src/app/features/tasks/state/tasks.reducer.spec.ts and ui/src/app/features/tasks/state/tasks.selectors.spec.ts"
Task: "Implement task API service and request mappers in ui/src/app/features/tasks/services/tasks-api.service.ts and ui/src/app/features/tasks/services/task-request-mapper.service.ts"
Task: "Implement reusable task query bar, task list, and task summary components in ui/src/app/features/tasks/components/task-query-bar/task-query-bar.component.ts, ui/src/app/features/tasks/components/task-list/task-list.component.ts, and ui/src/app/features/tasks/components/task-summary/task-summary.component.ts"
Task: "Implement task form and status action components in ui/src/app/features/tasks/components/task-form/task-form.component.ts and ui/src/app/features/tasks/components/task-status-menu/task-status-menu.component.ts"
```

## Parallel Example: User Story 3

```text
Task: "Add notification service and normalized API error mapping tests in ui/src/app/core/services/notification.service.spec.ts and ui/src/app/shared/utils/api-error.mapper.spec.ts"
Task: "Add interceptor tests for authorization and error normalization behavior in ui/src/app/core/interceptors/auth.interceptor.spec.ts and ui/src/app/core/interceptors/api-error.interceptor.spec.ts"
Task: "Refine shared API error normalization and form-error mapping helpers in ui/src/app/shared/utils/api-error.mapper.ts and ui/src/app/shared/utils/form-error-applier.util.ts"
Task: "Implement a shared request-status model and notification integration in ui/src/app/shared/models/request-status.model.ts and ui/src/app/core/services/notification.service.ts"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test unauthenticated routing, sign-up, login, and post-login redirect to `/tasks`
5. Demo the public auth experience before building the task workspace

### Incremental Delivery

1. Complete Setup + Foundational -> foundation ready
2. Add User Story 1 -> test independently -> demo
3. Add User Story 2 -> test independently -> demo
4. Add User Story 3 -> test independently -> demo
5. Finish polish tasks and rerun quickstart validation

### Parallel Team Strategy

With multiple developers:

1. Pair on Setup + Foundational to establish the workspace
2. After foundation is stable:
3. Developer A: auth state and public pages
4. Developer B: task data layer and NgRx state
5. Developer C: task UI components and shared feedback components
6. Merge on story checkpoints, then finish shared feedback integration together

---

## Notes

- `[P]` tasks touch different files and can be split safely once their prerequisites are met
- `[US1]`, `[US2]`, and `[US3]` map directly to the three active user stories in `spec.md`
- The requested `/users` testing screen remains intentionally deferred until the backend exposes list retrieval
- Each story is scoped so it can be validated independently before moving to the next phase
