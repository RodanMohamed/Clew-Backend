# Clew Backend API

`Clew Backend` is a layered ASP.NET Core Web API for an e-commerce jewelry domain. It provides authentication, role-based authorization, catalog management, cart management, order processing, and image upload capabilities.

## Tech Stack

- `.NET 10` (`net10.0`)
- `ASP.NET Core Web API`
- `Entity Framework Core` + `SQL Server`
- `ASP.NET Core Identity`
- `JWT Bearer Authentication`
- `AutoMapper`
- `FluentValidation`
- `Serilog`
- `Scalar` + OpenAPI for API exploration

## Solution Structure

- `API_Project` — Presentation layer (controllers, startup, auth policies, OpenAPI/Scalar)
- `Clew.BLL` — Business logic layer (managers, DTOs, validators, mapping profiles)
- `Clew.DAS` — Data access layer (EF Core context, models, configurations, repositories, unit of work)
- `Clew.Common` — Shared types (general result wrappers, pagination, filtering)

## Core Features

- Authentication and login with JWT
- Role-based access (`AdminOnly`, `UserOnly`)
- Product APIs:
  - list, details, paged/filtering, category filter, keyword search
  - create/update/delete (admin)
  - stock updates (admin)
  - toggle favorite (user)
- Category APIs:
  - list, details, paged search
  - create/update/delete (admin)
  - upload category image (admin)
- Cart APIs (user):
  - get cart, add/update/remove item, clear cart, item count
- Order APIs:
  - place order (user)
  - user order history
  - admin order listing and status update
  - cancel order (user)
- Image upload endpoint (user)
- Initial category/product seed data

## Getting Started

### 1) Prerequisites

- `Visual Studio 2026` or `dotnet SDK 10`
- `SQL Server` instance

### 2) Configure settings

Update `API_Project/appsettings.json`:

- `ConnectionStrings:ClewConnection`
- `JwtSettings` (`Issuer`, `Audience`, `DurationInMinutes`, `SecretKey`)

> Recommended: move secrets from `appsettings.json` to environment variables or user secrets for non-local environments.

### 3) Apply database migrations

From repository root:

```powershell
dotnet ef database update --project Clew.DAS --startup-project API_Project
```

### 4) Run the API

```powershell
dotnet run --project API_Project
```

Default local URLs (from launch profile):

- `https://localhost:7054`
- `http://localhost:5148`

## API Documentation (Scalar)

In `Development`, OpenAPI + Scalar are enabled.

- Scalar UI: `https://localhost:7054/scalar`
- OpenAPI document: `https://localhost:7054/openapi/v1.json`

## Authentication Flow

1. Register user: `POST /api/auth/register`
2. Login: `POST /api/auth/login`
3. Copy JWT token from response
4. In Scalar, click **Authorize** and use:

```text
Bearer <your_token>
```

## Validation and Response Pattern

- Request validation is handled with `FluentValidation`.
- Endpoints return a `GeneralResult<T>` envelope:
  - `success`
  - `message`
  - `data` (when applicable)

## Notes

- Product and category image uploads use multipart form-data.
- Product/category lists support pagination through `PaginationParameters`.
- Products support filter/search semantics for partial matching.

## Logging

`Serilog` writes logs to:

- Console
- `logs/log.txt` (rolling daily)

## Testing APIs
- see :
- 1-Postman testing Endpoints video :https://drive.google.com/file/d/1vh9dWaGQjNfQYs2NpWoy1Y1-M3AYX8od/view?usp=drivesdk
- 2-Scalar testing Endpoints Video:https://drive.google.com/file/d/1shML2A2ZSnisX3f4Jh9a2XDPbJzODpJJ/view?usp=drivesdk

