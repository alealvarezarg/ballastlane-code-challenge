[$speckit-plan]

Use **FluentValidation** to validate all API DTOs. Create one validator per DTO and keep the validation logic outside the controllers.

Additionally, create an **`openapi`** folder at the root of the solution containing the OpenAPI/Swagger specification for the API.

The OpenAPI definition should be maintained as a separate **YAML** file (for example, `openapi.yaml`) and should fully describe the exposed endpoints, request models, response models, and HTTP status codes.

The API implementation and the OpenAPI specification should remain synchronized so that the YAML document accurately reflects the current behavior of the API.
