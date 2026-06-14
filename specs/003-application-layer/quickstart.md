# Quickstart: Application Layer

## Prerequisites

- .NET 10 SDK
- Domain model feature available in `TaskManagementSystem.Domain`

## Validate the feature

1. Confirm the Application project contains `Abstractions`, `Services`, and `DependencyInjection.cs`.
2. Review the contracts and service behavior in [data-model.md](./data-model.md).
3. Run the Application unit tests:

```powershell
dotnet test tests/TaskManagementSystem.Application.UnitTests/TaskManagementSystem.Application.UnitTests.csproj
```

## Expected outcomes

- Task CRUD operations are exposed through `IManagementTaskService`.
- `ManagementTaskService` depends only on `IManagementTaskRepository` and Application-owned collaborators.
- `GetById` and `GetAll` use cache-aware behavior.
- Create, update, and delete operations invalidate stale cached read results.
- Application dependencies are registered through `AddApplicationDependencies`.
- The Application project remains independent from Infrastructure and API concerns.
