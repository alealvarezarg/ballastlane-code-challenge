# Quickstart: API Project

## Prerequisites

- .NET 10 SDK
- Existing Application and Infrastructure projects available in the solution

## Validate the feature

1. Confirm the API project contains `Controllers`, `Models`, and validation support for DTOs.
2. Confirm the API registers Application and Infrastructure dependencies in `Program.cs`.
3. Confirm the root contract file exists at `openapi/openapi.yaml`.
4. Start the API project.
5. Open the OpenAPI surface and inspect the `management-tasks` endpoints.
6. Compare the runtime API behavior with `openapi/openapi.yaml`.
7. Exercise the CRUD scenarios with valid and invalid request payloads.

## Suggested validation flow

1. Send `POST /management-tasks` with a valid create payload and confirm a created response with a task DTO body.
2. Send `GET /management-tasks` and confirm the created task appears in the collection response.
3. Send `GET /management-tasks/{id}` with the created identifier and confirm a single task DTO is returned.
4. Send `PUT /management-tasks/{id}` with a valid update payload and confirm the updated task DTO is returned.
5. Send `DELETE /management-tasks/{id}` and confirm the API returns no content.
6. Send an invalid create or update payload and confirm the API returns a consistent client-error response.
7. Send a request for a non-existent task identifier and confirm the API returns a consistent not-found response.

## Expected outcomes

- The API exposes all required `management-tasks` CRUD endpoints.
- Successful responses use DTO models instead of domain entities.
- DTO validation is enforced through FluentValidation with dedicated validators.
- Errors are returned through one consistent response shape.
- OpenAPI documentation is available for endpoint discovery and testing.
- The root `openapi/openapi.yaml` file reflects the current API behavior.
