[$speckit-specify]

Define the specification for the **Application** project.

Create an **Abstractions** folder containing:

* `IManagementTaskRepository`, defining the contract required to support CRUD operations for `ManagementTask`.
* `IManagementTaskService`, defining the contract for the application service responsible for task management operations.

Create a **Services** folder containing a `ManagementTaskService` that implements `IManagementTaskService` and depends only on `IManagementTaskRepository`.

Read operations (`GetById` and `GetAll`) should leverage `IMemoryCache` to improve performance, with appropriate cache invalidation when data changes.

Include a `DependencyInjection.cs` file containing a static extension class with an `AddApplicationDependencies` extension method for `IServiceCollection`. This method should register all dependencies belonging to the Application layer.

The Application project should depend only on the Domain project and its own abstractions, remaining independent from Infrastructure and API concerns.

Keep the specification short, concrete, and easy to review. Avoid unnecessary abstractions or overengineering.
