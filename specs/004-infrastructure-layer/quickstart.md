# Quickstart: Infrastructure Layer

## Prerequisites

- .NET 10 SDK
- Application and Domain features available in their respective projects

## Validate the feature

1. Confirm the Infrastructure project contains `Database`, `Repositories`, and `DependencyInjection.cs`.
2. Review the persistence structure in [data-model.md](./data-model.md).
3. Run the Infrastructure tests:

```powershell
dotnet test tests/TaskManagementSystem.Infrastructure.UnitTests/TaskManagementSystem.Infrastructure.UnitTests.csproj
```

## Expected outcomes

- `ManagementTaskRepository` implements the full `IManagementTaskRepository` contract.
- `ManagementTask` is persisted through LiteDB collections in an embedded database file.
- LiteDB connection setup is owned by the `Database` folder.
- Infrastructure dependencies are registered through `AddInfrastructureDependencies`.
- The Infrastructure project remains limited to persistence responsibilities and does not contain business logic.
