# Quickstart: ManagementUser Authentication

## Prerequisites

- .NET 10 SDK
- Existing solution dependencies restored
- LiteDB configuration available through the API project settings
- Authentication configuration values available for local token signing and expiration

## TDD rule for this feature

Every implementation slice starts by adding or updating failing automated tests before production code changes begin. Do not move to the next slice until the current slice is green and any immediate refactor is complete.

## Validation note

If `dotnet test` execution is restricted in the current environment, use build and static review as a temporary sanity check, then run the listed test commands as the next required step before merge.

## Recommended validation flow

1. Run the targeted failing tests for the next slice you plan to implement.
2. Implement the minimum production code needed to make that slice pass.
3. Re-run the targeted tests, then the broader affected test projects.
4. Repeat for the next behavior in this order:
   - Domain management-user invariants and exception behavior
   - Application registration, login, token issuance, and task-owner validation
   - Infrastructure user persistence, uniqueness checks, password hashing, and token helpers
   - API public/protected routes, validation, authorization, and OpenAPI alignment

## Suggested commands

```powershell
$env:DOTNET_CLI_HOME='d:\source\repos\TaskManagementSystem\.dotnet'
dotnet test tests\TaskManagementSystem.Domain.UnitTests\TaskManagementSystem.Domain.UnitTests.csproj
dotnet test tests\TaskManagementSystem.Application.UnitTests\TaskManagementSystem.Application.UnitTests.csproj
dotnet test tests\TaskManagementSystem.Infrastructure.UnitTests\TaskManagementSystem.Infrastructure.UnitTests.csproj
dotnet test tests\TaskManagementSystem.Api.UnitTests\TaskManagementSystem.Api.UnitTests.csproj
```

## End-to-end validation scenarios

1. Create a management user through the public registration endpoint and confirm the response contains only safe user fields.
2. Log in with the registered user credentials and confirm the response returns a bearer token plus user details without exposing any password material.
3. Call a protected ManagementTask endpoint without authentication and confirm the request is rejected.
4. Call the same protected endpoint with the bearer token from login and confirm the request is allowed to proceed.
5. Call the public management-user get-by-id endpoint and confirm the registered user can be retrieved for testing purposes.
6. Attempt login with an invalid password and confirm the API returns the documented authentication failure response.
7. Attempt to create a management task using a non-existing `UserId` while authenticated and confirm the API rejects the request and no task is persisted.
8. Review the generated and maintained OpenAPI descriptions and confirm only the three management-user endpoints are anonymous while protected endpoints declare bearer authentication.

## Expected outcomes

- Every feature slice is implemented from failing tests to passing tests before the next slice starts.
- Passwords are accepted only at the API request boundary, hashed before persistence, and never returned in any response.
- Registration, login, and test-only get-by-id flows behave as documented through DTO-only contracts.
- All ManagementTask endpoints reject unauthenticated requests and continue to work when valid authentication is supplied.
- Task creation fails deterministically when the referenced management user does not exist.
- The runtime API behavior stays aligned with [management-users-auth.openapi.yaml](./contracts/management-users-auth.openapi.yaml) and the root `openapi/openapi.yaml`.
