# Quickstart: API DTO and Logging Improvements

## Prerequisites

- .NET 10 SDK
- Existing solution dependencies restored
- Local API startup configuration available
- Console available to observe Serilog output during manual verification

## TDD rule for this feature

Every implementation slice starts by adding or updating failing automated tests before production code changes begin. Do not move to the next slice until the current slice is green and any immediate refactor is complete.

## Recommended implementation order

1. Add or update failing API tests that verify endpoints return DTOs only and that status values serialize as strings.
2. Add failing validation tests for invalid string status values in request bodies and query parameters.
3. Add failing logging tests for successful operations and handled expected failures.
4. Add failing exception-handling tests for warning-versus-error severity behavior and unchanged error payloads.
5. Implement the minimum production changes to make each slice pass before moving to the next one.

## Suggested commands

```powershell
$env:DOTNET_CLI_HOME='d:\source\repos\TaskManagementSystem\.dotnet'
dotnet test tests\TaskManagementSystem.Api.UnitTests\TaskManagementSystem.Api.UnitTests.csproj --no-restore
dotnet test tests\TaskManagementSystem.Application.UnitTests\TaskManagementSystem.Application.UnitTests.csproj --no-restore
dotnet test tests\TaskManagementSystem.Infrastructure.UnitTests\TaskManagementSystem.Infrastructure.UnitTests.csproj --no-restore
```

## Manual verification flow

1. Start the API and confirm Serilog writes startup and request-related logs to the console only.
2. Call a successful `ManagementUser` or `ManagementTask` endpoint and confirm the response payload contains only DTO fields.
3. Confirm every returned `status` value in task-related responses appears as `Pending`, `InProgress`, or `Completed`.
4. Send a valid task write request with a string `status` value and confirm the operation succeeds.
5. Send an invalid task write or query request with an unknown `status` value and confirm the API returns the existing validation error shape.
6. Trigger an expected handled failure, such as a not-found task lookup or invalid credentials, and confirm a warning-level log is emitted.
7. Trigger an unexpected exception in a controlled test path and confirm an error-level log is emitted without changing the public error contract.
8. Review the feature contract file and root OpenAPI document to confirm all affected status schemas are documented as string enums.

## Expected outcomes

- No endpoint returns a domain entity directly.
- Every affected success payload is a named `*Dto`.
- All task-related status values are exchanged as readable strings consistently.
- Invalid status input still produces the established validation error response.
- Serilog is configured centrally and writes only to the console.
- Operational logs distinguish success, expected warnings, and unexpected errors at the intended severity.

## Validation record

- 2026-06-14: `dotnet test tests\TaskManagementSystem.Application.UnitTests\TaskManagementSystem.Application.UnitTests.csproj --no-restore` passed.
- 2026-06-14: `dotnet test tests\TaskManagementSystem.Api.UnitTests\TaskManagementSystem.Api.UnitTests.csproj --no-restore` passed.
- 2026-06-14: `dotnet test tests\TaskManagementSystem.Infrastructure.UnitTests\TaskManagementSystem.Infrastructure.UnitTests.csproj --no-restore` passed.
