# 💰 FinTrack – Personal Finance Manager API

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-green)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)
![License](https://img.shields.io/badge/License-MIT-lightgrey)
![Build](https://img.shields.io/badge/Status-Active-brightgreen)

---

## 🧭 Overview

**FinTrack** is a backend-only **Personal Finance Manager API** built with **ASP.NET Core 8**.  
It helps users track income, expenses, and budgets across multiple bank accounts — with automated reporting and a modern, scalable architecture.

This project demonstrates how to build a **production-grade backend** using **Clean Architecture**, **JWT Authentication**, and **Entity Framework Core**.

---

## 🏗️ Architecture

**Clean n-tier architecture** ensures separation of concerns, scalability, and testability.

```
FinTrack.Domain        → Core entities, enums  
FinTrack.Application   → Business logic, interfaces, DTOs, services  
FinTrack.Infrastructure→ Repositories, EF Core, seeders, database  
FinTrack.API           → Controllers, authentication, dependency injection
```

📊 **Architecture Diagram**
![Architecture Diagram](assets/architecture-diagram.png)

---

## 🧾 Entity Relationship Diagram (ERD)

![ERD Diagram](assets/diagram-erd.png)

**Entities:**
- `ApplicationUser` → extends `IdentityUser`
- `BankAccount` → user’s accounts
- `Transaction` → income/expense records
- `Category` → transaction type
- `Budget` → user’s monthly spending limit
- `Report` → generated summaries

**Relationships:**
- 1️⃣ User ↔ * BankAccounts  
- 1️⃣ BankAccount ↔ * Transactions  
- 1️⃣ Category ↔ * Transactions  
- 1️⃣ User ↔ * Budgets

---

## ⚙️ Tech Stack

| Layer | Technology |
|--------|-------------|
| **Framework** | ASP.NET Core 8 |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Identity + JWT |
| **Jobs** | Hangfire (for reports) |
| **Email** | MailKit / SMTP |
| **Docs** | Swagger / OpenAPI |
| **Architecture** | Clean Architecture, Repository Pattern |

---

## 🔐 Authentication & Roles

- **ASP.NET Identity** handles registration, login, password reset, and email confirmation.  
- **JWT** provides secure, stateless authentication.  
- Role-based authorization:
  - 👤 **User** → personal finance tracking  
  - 🧑‍💼 **Admin** → system & category management  

**JWT Payload Example:**
```json
{
  "email": "user@email.com",
  "role": "User",
  "nameid": "guid-of-user",
  "iss": "https://localhost:5246",
  "aud": "https://localhost:5246"
}
```

---

## 🔌 API Endpoints

### 👤 Account
| Method | Endpoint | Description |
|--------|-----------|-------------|
| POST | `/api/account/register` | Register new user |
| GET | `/api/account/confirm-email` | Confirm user email |
| POST | `/api/account/login` | Login and get JWT |
| POST | `/api/account/forgot-password` | Reset password request |
| POST | `/api/account/reset-password` | Complete password reset |
| GET | `/api/account/me` | Get current logged user info |

---

### 🏦 Bank Accounts
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/accounts` | Get all user accounts |
| GET | `/api/accounts/{id}` | Get single account |
| POST | `/api/accounts` | Create new account |
| PUT | `/api/accounts/{id}` | Update account |
| DELETE | `/api/accounts/{id}` | Delete account |

---

### 💸 Transactions
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/transactions` | Get all user transactions |
| GET | `/api/transactions/{id}` | Get single transaction |
| POST | `/api/transactions` | Add new transaction |
| PUT | `/api/transactions/{id}` | Update transaction |
| DELETE | `/api/transactions/{id}` | Delete transaction |
| GET | `/api/transactions/report` | Filter by date/category/account |

---

### 🗂️ Categories (Admin only)
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/categories` | Get all categories |
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |

---

### 🎯 Budgets
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/budgets` | Get user budgets |
| POST | `/api/budgets` | Create budget |
| PUT | `/api/budgets/{id}` | Update budget |
| DELETE | `/api/budgets/{id}` | Delete budget |

---

### 📊 Reports
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/reports/monthly` | Generate monthly report (Hangfire job) |
| GET | `/api/reports/yearly` | Generate yearly summary |

---

## 🧠 Troubleshooting & Lessons Learned

### ❌ Problem:
`User.FindFirstValue(ClaimTypes.NameIdentifier)` returned **null** even though JWT was valid.

### 🔍 Diagnosis:
- Placed breakpoints → saw controller not being hit  
- Verified token decoding via [jwt.io](https://jwt.io)  
- Found that ASP.NET **auto-remaps claim types**, changing `nameid` → `sub`

### ✅ Fix:
Add this before configuring JWT:

```csharp
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
```

Now claims are mapped correctly and authenticated users are resolved properly.

📚 **Lesson learned:**  
Understanding the internal claim mapping in ASP.NET Core’s JWT middleware is critical for debugging user identity issues.

---

## 🚀 Getting Started

### 1️⃣ Clone repo
```bash
git clone https://github.com/<your-username>/FinTrack.git
```

### 2️⃣ Update DB connection string in `appsettings.json`

### 3️⃣ Apply migrations
```bash
dotnet ef database update
```

### 4️⃣ Run the app
```bash
dotnet run
```

### 5️⃣ Open Swagger UI
```
https://localhost:5246/swagger
```

---

## 🧩 Future Improvements

- [ ] Frontend SPA (Blazor or React)  
- [ ] AI-powered expense predictions (ML.NET)  
- [ ] Multi-currency and exchange rate sync  
- [ ] Docker support  
- [ ] Advanced reporting dashboard

---

## 👩‍💻 Author

**Sonja Divac**  
💼 [[LinkedIn Profile](https://www.linkedin.com/in/sonja-divac/)]  

---


