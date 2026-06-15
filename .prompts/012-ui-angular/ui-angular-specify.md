[$speckit-specify]

Define the specification for the **UI** project.

The frontend should be responsive, user-friendly, and follow a clean and well-structured architecture. Organize pages, components, services, and state management in a clear and maintainable way.

The implementation should reflect the quality expected from an **experienced UI/UX engineer**, focusing on usability, consistency, accessibility, intuitive navigation, and a polished user experience. The application should look professional while keeping the design simple and uncluttered.

The UI must implement **only the functionality currently available in the backend**. Do not add features, validations, or behaviors that are not already implemented by the existing API.

Implement the following screens:

* **Management Tasks**

  * List ManagementTasks.
  * Create a ManagementTask.
  * Update a ManagementTask.
  * Change the status of a ManagementTask.
  * Delete or archive a ManagementTask, depending on the current backend implementation.
  * Support all filters and operations currently exposed by the API.

* **Login**

  * Authenticate an existing ManagementUser.

* **Sign Up / Create User**

  * Create a new ManagementUser.

* **Users**

  * Display the list of ManagementUsers using the existing endpoint.
  * This screen exists **for testing purposes only** and is not part of the normal application flow.

The UI should consume the existing REST API using the defined DTO contracts and should never depend on Domain entities.

Implement all client-side validations that mirror the backend validations to provide immediate feedback to the user while keeping the backend as the source of truth.

Handle loading states, success messages, validation errors, authorization errors, and unexpected API errors consistently across the application.

Protect all authenticated screens according to the backend authentication behavior, leaving only Login and Sign Up publicly accessible.

Add unit tests covering the implemented components, pages, services, validation logic, and the main user flows, following the same TDD philosophy used throughout the backend.

Keep the implementation simple, clean, professional, and consistent with the existing architecture. Avoid unnecessary abstractions or overengineering.
