[$speckit-specify]

Define the specification for the **Infrastructure** project.

The Infrastructure project should implement the abstractions defined in the **Application** project, specifically `IManagementTaskRepository`.

Use **LiteDB** as the persistence mechanism, storing `ManagementTask` documents in a local embedded database file. The repository should provide the complete CRUD implementation using LiteDB collections.

Organize the project with a clear structure, including folders such as:

* Database
* Repositories

The **Database** folder should contain the classes responsible for configuring and exposing the LiteDB database connection.

The **Repositories** folder should contain `ManagementTaskRepository`, implementing all operations defined by `IManagementTaskRepository`.

The Infrastructure project should depend on the Application and Domain projects but should not introduce business logic. Its only responsibility is persistence.

Include a `DependencyInjection.cs` file containing a static extension class with an `AddInfrastructureDependencies` extension method for `IServiceCollection`. This method should register the LiteDB database and all repository implementations required by the Infrastructure layer.

Keep the specification short, concrete, and easy to review. Avoid unnecessary abstractions or overengineering.
