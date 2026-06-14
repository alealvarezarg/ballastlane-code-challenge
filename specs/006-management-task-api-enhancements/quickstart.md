# Quickstart: ManagementTask API Enhancements

## Prerequisites

- .NET 10 SDK
- Existing solution dependencies restored
- LiteDB configuration available through the API project settings

## TDD rule for this feature

Every implementation slice starts by adding or updating failing automated tests before production code changes begin. Do not move to the next slice until the current slice is green and any immediate refactor is complete.

## Validation note

If `dotnet test` execution is restricted in the current environment, use build and static review as a temporary sanity check, then run the listed test commands as the next required step before merge.

## Recommended validation flow

1. Run the targeted failing tests for the next slice you plan to implement.
2. Implement the minimum production code needed to make that slice pass.
3. Re-run the targeted tests, then the broader affected test projects.
4. Repeat for the next behavior in this order:
   - Domain lifecycle and archive rules
   - Application query, summary, idempotency, and cache behavior
   - Infrastructure query/idempotency persistence
   - API contracts, validation, error mapping, and OpenAPI alignment

## Suggested commands

```powershell
$env:DOTNET_CLI_HOME='d:\source\repos\TaskManagementSystem\.dotnet'
dotnet test tests\TaskManagementSystem.Domain.UnitTests\TaskManagementSystem.Domain.UnitTests.csproj
dotnet test tests\TaskManagementSystem.Application.UnitTests\TaskManagementSystem.Application.UnitTests.csproj
dotnet test tests\TaskManagementSystem.Infrastructure.UnitTests\TaskManagementSystem.Infrastructure.UnitTests.csproj
dotnet test tests\TaskManagementSystem.Api.UnitTests\TaskManagementSystem.Api.UnitTests.csproj
```

## End-to-end validation scenarios

1. Create a task without a status and confirm the response status is `Pending`.
2. Repeat the same create request with the same `Idempotency-Key` and confirm no duplicate task is created.
3. Request `GET /management-tasks` with combined `status`, `userId`, `search`, `sortBy`, `sortDirection`, `page`, and `pageSize` values and confirm the filtered page is deterministic.
4. Request `GET /management-tasks/overdue` and `GET /management-tasks/due-within?days=7` and confirm only matching tasks are returned.
5. Request `GET /management-tasks/summary` and confirm counts are grouped by status.
6. Send `PATCH /management-tasks/{id}` with a subset of mutable fields and confirm unchanged fields are preserved.
7. Send `PATCH /management-tasks/{id}/status` with an allowed transition and then with a disallowed transition, confirming deterministic success and client-error responses.
8. Attempt to update or delete a completed task and confirm the API rejects the request.
9. Delete an eligible task and confirm the task is archived and excluded from the default active list.
10. Send invalid query, body, or route/body identifier combinations and confirm HTTP 400 responses use the standard error shape with `traceId`.

## Expected outcomes

- Every feature slice is implemented from failing tests to passing tests before the next slice starts.
- Combined filters, sorting, search, pagination, overdue, due-within, and summary reads behave as documented.
- Status transitions, non-editable rules, soft delete, and idempotent create behavior are enforced consistently.
- Successful responses use DTOs only, and error responses are deterministic and always include `traceId`.
- Read caches are invalidated after create, update, patch, status change, and archive operations.
- The runtime API behavior stays aligned with [management-tasks.openapi.yaml](./contracts/management-tasks.openapi.yaml) and the root `openapi/openapi.yaml`.
