# Structural Model: Initial Project Scaffolding

## Solution

- **TaskManagementSystem.slnx**: Root container for the backend and test projects.

## Source Projects

- **TaskManagementSystem.Domain**: Domain types and core abstractions.
- **TaskManagementSystem.Application**: Use-case orchestration and application contracts.
- **TaskManagementSystem.Infrastructure**: External integrations and persistence implementations.
- **TaskManagementSystem.Api**: Runtime host and composition root.

## Test Projects

- **TaskManagementSystem.Domain.UnitTests**: Unit tests for domain behavior.
- **TaskManagementSystem.Application.UnitTests**: Unit tests for application behavior.

## Dependency Rules

- Application -> Domain
- Infrastructure -> Application, Domain
- Api -> Application, Infrastructure
- Domain.UnitTests -> Domain
- Application.UnitTests -> Application, Domain
