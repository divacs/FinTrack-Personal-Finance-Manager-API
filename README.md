# FinTrack - Personal Finance Manager API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-blue)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-green)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)
![Tests](https://img.shields.io/badge/Tests-xUnit-orange)

FinTrack is a backend API for personal finance management, built with ASP.NET Core 8, Entity Framework Core, SQL Server, ASP.NET Identity, JWT authentication, MailKit and Hangfire.

The project is designed as a practical portfolio backend: it demonstrates layered architecture, authentication and authorization, ownership-based data access, background jobs, email flows, migrations, validation, and focused automated tests.

## What This Project Demonstrates

This project is suitable as a medior .NET backend portfolio project because it goes beyond basic CRUD and covers common real-world backend concerns:

- User registration, login, email confirmation and password reset
- JWT-based authentication with role-based authorization
- Ownership checks so users can access only their own financial data
- Admin/Manager access patterns for wider operational access
- Bank accounts, transactions, categories, budgets and reports
- Background report jobs with Hangfire
- SMTP email integration with certificate validation
- Fail-fast configuration validation for JWT and SMTP settings
- EF Core migrations and relationship configuration
- Defensive API validation for invalid foreign keys and invalid report dates
- Tests for report generation and background job behavior

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 8 |
| API | ASP.NET Core Web API |
| Authentication | ASP.NET Identity + JWT Bearer |
| Authorization | Role-based authorization |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Background jobs | Hangfire |
| Email | MailKit / SMTP |
| Documentation | Swagger / OpenAPI |
| Testing | xUnit, Moq, FluentAssertions, SQLite in-memory |

## Solution Structure

```text
FinTrack.Domain
  Core domain entities such as ApplicationUser, BankAccount, Transaction, Category, Budget and Report.

FinTrack.Application
  Interfaces, application services, email/token abstractions and background job logic.

FinTrack.Infrastructure
  EF Core DbContext, repositories, migrations and database persistence.

FinTrack.API
  Controllers, dependency injection, authentication setup, Swagger, Hangfire setup and startup validation.

FinTrack.Tests
  Unit/integration-style tests for report calculation and background report jobs.
```

## Main Features

### Account Management

- Register a new user
- Confirm email
- Login and receive JWT
- Request password reset
- Reset password
- Get current authenticated user

### Finance Management

- Manage bank accounts
- Create, update, list and delete transactions
- Categorize income and expenses
- Manage budgets per category
- Generate monthly and yearly financial reports

### Background Processing

Hangfire is used for scheduled report jobs:

- Monthly report email job
- Yearly report email job
- Job logs stored through `ReportJobLogs`
- Hangfire dashboard protected by Admin-only authorization

### Security And Data Safety

The project includes several protections that are important in production-style APIs:

- JWT issuer, audience and signing key validation at startup
- SMTP configuration validation at startup
- SMTP TLS certificate validation enabled
- Default demo/admin/manager seed users are limited to Development
- Transaction ownership checks to prevent IDOR vulnerabilities
- Category deletion is blocked when budgets depend on it
- Invalid foreign keys are validated before database writes
- Report date inputs are validated before `DateTime` creation
- Report generation no longer creates duplicate database records on every GET request

## API Overview

Base route examples use `/api/...`.

### Account

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/account/register` | Register user and send confirmation email |
| GET | `/api/account/confirm-email` | Confirm email address |
| POST | `/api/account/login` | Login and return JWT |
| POST | `/api/account/forgot-password` | Send password reset email |
| POST | `/api/account/reset-password` | Reset password |
| GET | `/api/account/me` | Get current user |

### Bank Accounts

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/bankaccounts` | List accounts |
| GET | `/api/bankaccounts/{id}` | Get account by id |
| POST | `/api/bankaccounts` | Create account |
| PUT | `/api/bankaccounts/{id}` | Update account |
| DELETE | `/api/bankaccounts/{id}` | Delete account |

### Transactions

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/transactions` | List transactions |
| GET | `/api/transactions/{id}` | Get transaction by id |
| POST | `/api/transactions` | Create transaction |
| PUT | `/api/transactions/{id}` | Update transaction |
| DELETE | `/api/transactions/{id}` | Delete transaction |
| POST | `/api/transactions/report` | Filter transactions for reporting |

### Categories

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/categories` | List categories |
| POST | `/api/categories` | Create category, Admin only |
| PUT | `/api/categories/{id}` | Update category, Admin only |
| DELETE | `/api/categories/{id}` | Delete category, Admin only |

### Budgets

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/budgets` | List current user's budgets |
| POST | `/api/budgets` | Create budget |
| PUT | `/api/budgets/{id}` | Update budget |
| DELETE | `/api/budgets/{id}` | Delete budget |

### Reports

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/report/monthly?year=2025&month=5` | Generate monthly report |
| GET | `/api/report/yearly?year=2025` | Generate yearly report |

## Configuration

Sensitive values are intentionally not stored in `appsettings.json`.

The application validates required configuration at startup and fails fast if any required value is missing, weak or left as a placeholder.

Required settings:

```text
ConnectionStrings__DefaultConnection
JWT__Issuer
JWT__Audience
JWT__SigningKey
EmailSettings__From
EmailSettings__SmtpServer
EmailSettings__Port
EmailSettings__Username
EmailSettings__Password
```

`JWT__SigningKey` must be at least 32 characters long.

Example local setup with user-secrets:

```bash
dotnet user-secrets init --project FinTrack.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=FinTrackDb;Trusted_Connection=True;TrustServerCertificate=True;" --project FinTrack.API
dotnet user-secrets set "JWT:Issuer" "FinTrackApi" --project FinTrack.API
dotnet user-secrets set "JWT:Audience" "FinTrackClient" --project FinTrack.API
dotnet user-secrets set "JWT:SigningKey" "replace-with-a-secure-32-character-minimum-key" --project FinTrack.API
dotnet user-secrets set "EmailSettings:From" "noreply@example.com" --project FinTrack.API
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.example.com" --project FinTrack.API
dotnet user-secrets set "EmailSettings:Port" "587" --project FinTrack.API
dotnet user-secrets set "EmailSettings:Username" "smtp-user" --project FinTrack.API
dotnet user-secrets set "EmailSettings:Password" "smtp-password" --project FinTrack.API
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- EF Core CLI tools

Install EF tools if needed:

```bash
dotnet tool install --global dotnet-ef
```

### Restore And Build

```bash
dotnet restore
dotnet build --no-restore
```

### Apply Migrations

```bash
dotnet ef database update --project FinTrack.Infrastructure --startup-project FinTrack.API
```

### Run API

```bash
dotnet run --project FinTrack.API
```

Swagger is available in Development:

```text
https://localhost:<port>/swagger
```

## Testing

Run tests:

```bash
dotnet test
```

Current test coverage focuses on:

- Monthly report calculation
- Report controller behavior
- Monthly background report job success flow
- Background job failure logging when email sending fails

## Design Notes

### Ownership-Aware Data Access

Financial data is user-owned. Regular users are restricted to their own accounts and transactions. Admin and Manager roles keep broader access where the existing authorization pattern allows it.

### GET Reports Without Persistence Side Effects

Report endpoints calculate and return report data without inserting a new row into `Reports` on every request. This keeps GET behavior predictable and avoids uncontrolled table growth.

### Safe Category Deletion

Categories cannot be deleted while budgets reference them. The API returns a controlled conflict response instead of deleting dependent budget data.

### Startup Configuration Validation

JWT and SMTP settings are checked when the app starts. This avoids delayed runtime failures and prevents weak/default JWT secrets from being used accidentally.

## Possible Future Improvements

- Add Docker Compose for API + SQL Server
- Add refresh tokens
- Add audit logs for admin actions
- Add more endpoint-level integration tests
- Add pagination and sorting for large transaction lists
- Add currency conversion support
- Add CI pipeline with build, test and vulnerability scanning

## Author

Sonja Divac  
LinkedIn: [linkedin.com/in/sonja-divac](https://www.linkedin.com/in/sonja-divac/)
