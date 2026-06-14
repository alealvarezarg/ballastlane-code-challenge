[$speckit-specify]

Extend the existing solution to support **ManagementUser** while following the same architecture, patterns, conventions, and design decisions already used throughout the project.

Implement only the following endpoints:

* Create ManagementUser (public endpoint)
* Login ManagementUser (public endpoint)
* Get ManagementUser by Id (public endpoint, for testing purposes only)

All other endpoints in the application, including every **ManagementTask** endpoint, must require authentication.

Reuse the existing project structure and patterns across the Domain, Application, Infrastructure, and API layers. Keep controllers thin, place business logic in the Application layer, and persistence logic in the Infrastructure layer.

Authentication should integrate naturally with the existing architecture and protect all secured endpoints consistently.

Use DTOs for all API requests and responses, ensuring every DTO ends with the `Dto` suffix.

Apply FluentValidation to all new request DTOs and ensure validation errors continue to use the existing global exception handling pipeline.

When creating a **ManagementTask**, validate that the referenced **ManagementUser** exists in the data store before creating the task. If the user does not exist, the operation must fail with the appropriate business validation error and the task must not be persisted.

Update the OpenAPI specification to include the new endpoints and authentication requirements.

Add or update unit tests for all new functionality, including:

* ManagementUser creation.
* ManagementUser login.
* ManagementUser retrieval.
* Authorization behavior for protected endpoints.
* Authentication requirements for all ManagementTask endpoints.
* Validation that prevents creating a ManagementTask for a non-existing ManagementUser.

Follow the existing TDD approach and testing patterns already established in the solution.
