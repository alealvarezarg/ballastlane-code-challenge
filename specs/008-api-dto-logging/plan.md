# Implementation Plan: API DTO and Logging Improvements

**Branch**: `008-api-dto-logging` | **Date**: 2026-06-14 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/008-api-dto-logging/spec.md`

## Summary

Tighten the API contract so every endpoint returns explicit response DTOs only, represent `ManagementTaskStatus` consistently as readable strings on both requests and responses, and add simple centralized application logging backed by Serilog console output. The design keeps controller logic thin, preserves the existing validation and exception pipeline, and introduces logging through `ILogger<T>` usage at the current API and Application boundaries.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core Web API, FluentValidation.AspNetCore, LiteDB, `Microsoft.AspNetCore.Authentication.JwtBearer`, Swashbuckle.AspNetCore, `Serilog.AspNetCore`, `Serilog.Sinks.Console`

**Storage**: LiteDB through the Infrastructure layer

**Testing**: xUnit, Shouldly, API unit/contract-style tests, Application unit tests, Infrastructure unit tests

**Target Platform**: .NET backend web API

**Project Type**: Clean Architecture backend web service

**Performance Goals**: Preserve current local-development responsiveness while keeping DTO mapping, enum conversion, and logging overhead lightweight and synchronous enough for the existing in-process API usage.

**Constraints**: TDD is mandatory; controllers remain translation-only; no endpoint may expose domain entities directly; request/response status values must be string-based and validation-backed; invalid status values must continue through the existing validation error response shape; logging must use the .NET logging abstractions with Serilog configured centrally for console output only; public error contracts must remain unchanged.

**Scale/Scope**: Cross-cutting refinement over the existing ManagementTask and ManagementUser API surfaces, shared API serialization/mapping behavior, centralized logging configuration, and supporting automated tests across API/Application/Infrastructure projects.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Domain and application behavior stays isolated from frameworks, transport, and persistence.
- Business rules, validation, and use-case decisions live outside controllers, handlers, and repositories.
- Required automated tests are identified for every changed business behavior and boundary crossing.
- Simpler designs were preferred; any added abstraction has an explicit justification.
- Dependency injection, API contract consistency, and documentation updates are accounted for.

**Gate Status**: Pass. The feature is primarily an API contract and observability refinement. It preserves the current layer boundaries by keeping DTO mapping in the API layer, operational orchestration logging in services, and exception severity decisions in centralized exception handling, while using Serilog only as the configured logging provider behind `ILogger<T>`.

## Project Structure

### Documentation (this feature)

```text
specs/008-api-dto-logging/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- api-dto-logging.openapi.yaml
`-- tasks.md
```

### Source Code (repository root)

```text
src/
|-- TaskManagementSystem.Api/
|   |-- Controllers/
|   |-- ExceptionHandling/
|   |-- Models/
|   |-- Validation/
|   |-- ApiServiceCollectionExtensions.cs
|   `-- Program.cs
|-- TaskManagementSystem.Application/
|   |-- Abstractions/
|   |-- Models/
|   `-- Services/
|-- TaskManagementSystem.Domain/
|   |-- Entities/
|   |-- Enums/
|   `-- Exceptions/
`-- TaskManagementSystem.Infrastructure/
    |-- Authentication/
    |-- Database/
    `-- Repositories/

tests/
|-- TaskManagementSystem.Api.UnitTests/
|   |-- Controllers/
|   |-- ExceptionHandling/
|   `-- Validation/
|-- TaskManagementSystem.Application.UnitTests/
`-- TaskManagementSystem.Infrastructure.UnitTests/

openapi/
`-- openapi.yaml
```

**Structure Decision**: Preserve the existing four-project backend structure. The API project owns DTO definitions, mappings, JSON enum contract behavior, Swagger/OpenAPI alignment, and exception-handler logging. The Application project owns success-path operational logging and warning logs for expected business outcomes when those outcomes are determined before exceptions are thrown. Infrastructure remains unchanged except for dependency wiring required to keep logging provider configuration centralized at application startup.

## Phase Plan

### Phase 0

- Confirm the simplest DTO-only response approach across every existing endpoint and response case without introducing a dedicated mapping framework.
- Confirm the safest low-complexity JSON strategy for string-based `ManagementTaskStatus` parsing and serialization while preserving validation failures for invalid inputs.
- Confirm where informational, warning, and error logs should be emitted so signal stays consistent and duplicated logs are avoided.
- Confirm the smallest Serilog setup that satisfies the requirement: centralized startup configuration with console sink only and continued `ILogger<T>` consumption elsewhere.
- Confirm the TDD slice order for contract mapping, status handling, and logging coverage.

### Phase 1

- Design the affected response DTO inventory and identify any endpoints that still risk exposing domain entities directly.
- Design the API contract updates for string-based `ManagementTaskStatus` values across request DTOs, response DTOs, query DTOs, and summary payloads.
- Design the logging event boundaries for successful operations, expected validation/not-found scenarios, and unexpected exceptions.
- Design the minimal startup/configuration changes required to register Serilog once and keep logging concerns centralized.
- Update the agent context to point to this plan.

### Phase 2

- Implement in strict TDD order:
  1. API and mapping tests for DTO-only responses and string enum serialization/parsing behavior.
  2. Validation tests for invalid status values in body and query-bound request models.
  3. Application tests for informational and warning logs around successful and expected non-success outcomes.
  4. API exception-handling tests for warning/error logging and unchanged public error responses.
  5. OpenAPI contract updates and final alignment checks against the documented string enum surface.
- Keep refactors within each slice after tests pass and before moving to the next behavior.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |
