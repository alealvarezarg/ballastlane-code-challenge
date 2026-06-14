[$speckit-specify]

Define the specification for the **API** project.

Create a **Controllers** folder containing a `ManagementTaskController` responsible for exposing the CRUD operations for `ManagementTask`.

The controller should depend only on `IManagementTaskService` and should not contain business logic.

Expose the following endpoints:

* GET /management-tasks
* GET /management-tasks/{id}
* POST /management-tasks
* PUT /management-tasks/{id}
* DELETE /management-tasks/{id}

Use the appropriate HTTP status codes and response types for each operation.

Create a **Models** folder containing all request and response DTOs. Every model in this folder must have the **`Dto`** suffix (e.g., `CreateManagementTaskRequestDto`, `UpdateManagementTaskRequestDto`, `ManagementTaskResponseDto`) to clearly distinguish API models from Domain entities.

Do not expose Domain entities directly through the API.

Configure the project to register the Application and Infrastructure dependencies using their respective extension methods.

Enable Swagger/OpenAPI for testing and documentation.

Add global exception handling to return consistent error responses.

Keep controllers thin and focused only on translating HTTP requests into application service calls.

Keep the specification short, concrete, and easy to review. Avoid unnecessary abstractions or overengineering.
