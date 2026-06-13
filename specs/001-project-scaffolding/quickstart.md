# Quickstart: Initial Project Scaffolding

## Prerequisites

- .NET 10 SDK

## Validate the scaffold

```powershell
dotnet restore TaskManagementSystem.slnx
dotnet build TaskManagementSystem.slnx
dotnet test TaskManagementSystem.slnx
```

## Expected result

- The solution restores successfully.
- All backend projects target `.NET 10`.
- Unit test projects are ready with `xUnit`, `FakeItEasy`, and `Shouldly`.
- The API host starts without feature endpoints and acts only as scaffold startup.
