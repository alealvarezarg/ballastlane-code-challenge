[$speckit-specify]

Domain model:

### ManagementTask

Properties:

* Id (Guid)
* Title (string)
* Description (string)
* Status (TaskStatus)
* DueDate (DateTime)
* UserId (Guid)

Behavior:

* Validate that Title is not null or empty.
* Validate that Description is not null or empty.
* Validate that DueDate is valid.
* Expose methods to update its information.
* Expose a method to change its status.
* Prevent the entity from entering an invalid state.

### ManagementUser

Properties:

* Id (Guid)
* Username (string)
* Email (string)
* PasswordHash (string)

Behavior:

* Validate that Username is not null or empty.
* Validate that Email is not null or empty.
* Validate that PasswordHash is not null or empty.
* Prevent the entity from entering an invalid state.

### Relationship

A `ManagementTask` belongs to a single `ManagementUser` through the `UserId` property.

Do not include navigation properties or persistence-specific relationships.

### ManagementTaskStatus

Create an enum with the following values:

* Pending
* InProgress
* Completed

### Domain Exceptions

Create domain exceptions to represent validation errors when an entity is created or modified with invalid data.

### General Guidelines

* Keep entities encapsulated.
* Prefer methods over public setters.
* Keep the implementation simple and focused on business rules.
* Avoid unnecessary abstractions or patterns.
