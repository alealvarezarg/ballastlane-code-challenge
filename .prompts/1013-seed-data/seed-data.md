Analyze the current `ManagementTask` and `ManagementUser` domain models and implement a seed data mechanism for the application.

The seed process should execute automatically during application startup.

If the database does not exist, create it and populate it with sample data. If the database already exists, do not recreate or reseed it.

Generate the following initial data:

* **3 ManagementUsers** with realistic and distinct information.
* All users should use the same password: **`1234567890`**, stored using the existing password hashing mechanism.
* **25 ManagementTasks** assigned to the first user.
* **11 ManagementTasks** assigned to the second user.
* **5 ManagementTasks** assigned to the third user.

The generated tasks should contain varied and realistic data, including different titles, descriptions, due dates, and a balanced distribution across the available `ManagementStatus` values.

The generated data should be deterministic enough for testing and demonstration purposes while still appearing realistic.

Keep the implementation simple and centralized, with all database initialization and seed logic encapsulated in a dedicated component executed during application startup.
