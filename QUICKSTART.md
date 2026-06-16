# Quick Start

This guide is focused on getting the current implementation running locally as quickly as possible.

## Prerequisites

- .NET SDK `10.0.301` or a compatible .NET 10 SDK
- Node.js version compatible with Angular 22
- npm
- Chrome or Chrome Headless available for frontend tests

Optional but recommended:

- Trust the local ASP.NET Core HTTPS certificate:

```powershell
dotnet dev-certs https --trust
```

## Backend Setup

```powershell
cd api
dotnet restore TaskManagementSystem.slnx
dotnet run --project src/TaskManagementSystem.Api
```

Notes:

- The default HTTPS URL is `https://localhost:7203`
- Swagger is available only when the API runs in `Development`
- On first startup, LiteDB is created and seed data is inserted automatically

## Frontend Setup

Open a second terminal:

```powershell
cd ui
npm install
npm start
```

Notes:

- The Angular dev server runs on `http://localhost:4200`
- The frontend expects the API at `https://localhost:7203`

## Seeded Login

The backend seeds three users on first startup. All seeded users share this password:

```text
1234567890
```

Example seeded user:

```text
alicia.moreno@example.com
```

Other seeded emails:

- `marcus.bennett@example.com`
- `sofia.chen@example.com`

## Build Commands

### Backend

```powershell
cd api
dotnet build TaskManagementSystem.slnx
```

### Frontend

```powershell
cd ui
npm run build
```

## Run Commands

### Backend

```powershell
cd api
dotnet run --project src/TaskManagementSystem.Api
```

### Frontend

```powershell
cd ui
npm start
```

## Test Commands

### Backend

```powershell
cd api
dotnet test tests/TaskManagementSystem.Api.UnitTests/TaskManagementSystem.Api.UnitTests.csproj --no-restore
dotnet test tests/TaskManagementSystem.Application.UnitTests/TaskManagementSystem.Application.UnitTests.csproj --no-restore
dotnet test tests/TaskManagementSystem.Domain.UnitTests/TaskManagementSystem.Domain.UnitTests.csproj --no-restore
dotnet test tests/TaskManagementSystem.Infrastructure.UnitTests/TaskManagementSystem.Infrastructure.UnitTests.csproj --no-restore
```

### Frontend

```powershell
cd ui
npm test
```

## URLs

- Swagger UI: `https://localhost:7203/swagger`
- API base URL: `https://localhost:7203`
- Frontend UI: `http://localhost:4200`

## Common Development Commands

### Restore dependencies

```powershell
cd api
dotnet restore TaskManagementSystem.slnx

cd ../ui
npm install
```

### Rebuild both projects

```powershell
cd api
dotnet build TaskManagementSystem.slnx

cd ../ui
npm run build
```

### Run validated frontend checks

```powershell
cd ui
npm run build
npm test
```

### Run a validated backend check

```powershell
cd api
dotnet test tests/TaskManagementSystem.Api.UnitTests/TaskManagementSystem.Api.UnitTests.csproj --no-restore
```

## Current Scope Notes

- The frontend currently implements login, sign-up, and a protected tasks page.
- The frontend does not currently include a users page.
- The backend exposes more task endpoints than the current UI uses directly, including overdue, due-within, summary, and partial patch endpoints.
