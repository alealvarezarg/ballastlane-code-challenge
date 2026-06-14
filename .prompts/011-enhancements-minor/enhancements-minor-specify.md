[$speckit-specify]

Define the specification for the following API improvements.

Do not expose Domain entities directly from any endpoint.

Create explicit response DTOs for each API response case, using the `Dto` suffix. Controllers should return DTOs only, not Domain entities.

Update all mappings so Domain entities are converted to response DTOs before being returned to the client.

For `ManagementStatus`, API responses must serialize the enum as a readable string, such as `Pending`, `InProgress`, or `Completed`, instead of returning numeric values.

API requests that include `ManagementStatus` should also accept string values like `Pending`, `InProgress`, or `Completed`, and internally convert them to the enum type.

Invalid status values must return the existing validation error response.

Add application logging using the existing .NET logging abstractions.

Include informational logs for successful main operations, warning logs for expected validation or not-found scenarios, and error logs for unexpected exceptions.

Keep the changes simple, consistent with the current architecture, and covered by unit tests where applicable.
