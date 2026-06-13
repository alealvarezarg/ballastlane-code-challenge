# Quickstart: Domain Model

## Prerequisites

- .NET 10 SDK

## Validate the feature

1. Confirm the feature scope is limited to the Domain project and Domain unit tests.
2. Review the entity rules in [data-model.md](./data-model.md).
3. Run the domain unit tests:

```powershell
dotnet test tests/TaskManagementSystem.Domain.UnitTests/TaskManagementSystem.Domain.UnitTests.csproj
```

## Expected outcomes

- `ManagementTask` cannot be created or updated with missing required values.
- `ManagementTask` status changes accept only defined lifecycle values.
- `ManagementUser` cannot be created with missing required values.
- Domain validation failures are reported with domain-specific exceptions.
- No implementation for this feature appears outside the Domain project and its unit tests.
