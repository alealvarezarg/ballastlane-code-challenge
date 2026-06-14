[$speckit-specify]

Define the specification for the **ManagementTask API enhancements**.

Extend the existing API to support the following functional requirements:

* Filter ManagementTasks by status.
* Filter ManagementTasks by user.
* Support combining multiple filters in a single request.
* Support pagination using `page` and `pageSize`.
* Support sorting by `title`, `status`, and `dueDate` in ascending or descending order.
* Support text search over `title` and `description`.
* Expose a dedicated endpoint to update only the status of a ManagementTask.
* Support retrieving overdue ManagementTasks.
* Support retrieving ManagementTasks due within a configurable number of days.
* Prevent deletion when the current status does not allow it.
* Validate allowed status transitions.
* Default the status to `Pending` when omitted during creation.
* Support partial updates through PATCH.
* Support retrieving ManagementTasks filtered by both user and status.
* Expose a summary endpoint returning the number of ManagementTasks grouped by status.
* Prevent duplicate creation through an idempotency mechanism.
* Support soft delete by archiving ManagementTasks instead of permanently deleting them.
* Prevent modification of restricted fields when the ManagementTask is in a non-editable status.
* Support retrieving only active (non-archived) ManagementTasks.

The implementation should also satisfy the following non-functional requirements:

* Invalid requests must return HTTP 400 with a consistent error response.
* Known exceptions must be handled globally and mapped to consistent HTTP responses.
* Every error response must include a `traceId`.
* All endpoints must return deterministic HTTP status codes.
* Controllers must remain thin and contain no business logic.
* Domain entities must never be exposed directly; only DTOs may be returned.
* All request DTOs must be validated using FluentValidation.
* Route identifiers and payload identifiers must be validated for consistency.
* Read operations should leverage in-memory caching where applicable, with proper cache invalidation after modifications.
* Clean Architecture boundaries must be preserved between Domain, Application, Infrastructure, and API.
* Repository implementations must remain isolated within the Infrastructure layer.
* The solution should remain simple, readable, and free of unnecessary abstractions.

Keep the specification concise, practical, and focused on behavior. Prefer clear acceptance criteria over lengthy explanations.
