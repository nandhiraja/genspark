# Multi-Tier Architecture in C# - Notes

---

# What is Multi-Tier Architecture?

Multi-tier architecture means:

```text id="9y4y9l"
Splitting application into separate layers
based on responsibilities
```

Instead of writing everything in one file/class,
application is divided properly.

---

# Why Companies Use Multi-Tier Architecture

Because real applications become large.

Without layers:

```text id="mbb1qz"
UI
Database
Business Logic
Validation
API calls

all mixed together
```

Problems:

* Hard to maintain
* Hard to debug
* Difficult for teams
* Code duplication
* Tight coupling

---

# Main Goal

```text id="tq1r88"
Separation of Responsibilities
```

Each layer handles only its own work.

---

# Common Layers in C# Applications

---

# 1. Presentation Layer

Also called:

```text id="jlwmv1"
UI Layer
```

Handles:

* User interaction
* Input/output
* API endpoints
* Screens/forms

---

## Examples

* ASP.NET Controllers
* React frontend
* MVC Views
* Console UI

---

## Responsibility

```text id="jlwmv2"
Take request from user
Send response back
```

Should NOT contain:

* Database queries
* Heavy business logic

---

# 2. Business Layer

Also called:

```text id="jlwmv3"
Service Layer
BLL
```

Handles:

* Business rules
* Validation
* Processing
* Application logic

---

## Example

```text id="jlwmv4"
Can user withdraw money?
Can order be cancelled?
Can stock be reduced?
```

---

## Responsibility

```text id="jlwmv5"
Main application brain
```

---

# 3. Data Access Layer

Also called:

```text id="jlwmv6"
DAL
Repository Layer
```

Handles:

* Database operations
* CRUD operations
* Queries
* EF Core/SQL interaction

---

## Responsibility

```text id="jlwmv7"
Talk only with database
```

Should NOT contain:

* UI logic
* Business decisions

---

# 4. Database Layer

Stores actual data.

Examples:

* PostgreSQL
* SQL Server
* MySQL

---

# Basic Flow

```text id="jlwmv8"
User
 ↓
Controller/UI
 ↓
Service Layer
 ↓
Repository/DAL
 ↓
Database
```

Response comes back same path.

---

# Real Example — Login System

---

# UI Layer

Gets:

```text id="jlwmv9"
Username
Password
```

Sends request to service.

---

# Service Layer

Checks:

* User exists?
* Password correct?
* Account blocked?

Then calls repository.

---

# Repository Layer

Executes:

```sql id="jlwmwa"
SELECT * FROM Users
```

Returns data.

---

# Why This Separation is Important

---

# Easier Maintenance

Database change affects only DAL.

---

# Easier Testing

Business logic can be tested separately.

---

# Team Collaboration

Frontend/backend/database teams work independently.

---

# Reusability

Same business logic reusable in:

* Web app
* Mobile app
* API

---

# Better Scalability

Application grows cleanly.

---

# Typical Folder Structure in C#

```text id="jlwmwb"
Project
 ├── Controllers
 ├── Services
 ├── Repositories
 ├── Models
 ├── DTOs
 ├── Interfaces
 └── DBContext
```

---

# Layer Responsibilities

| Layer      | Responsibility   |
| ---------- | ---------------- |
| Controller | Request/Response |
| Service    | Business logic   |
| Repository | Database access  |
| Database   | Data storage     |

---

# Important Rule

Each layer should:

```text id="jlwmwc"
Do only its own responsibility
```

Avoid mixing layers.

---

# Bad Example

Controller directly writing SQL query.

Problem:

```text id="jlwmwd"
Tightly coupled
Hard maintenance
```

---

# Good Example

Controller → Service → Repository

Clean separation.

---

# Common Terms in C#

| Term       | Meaning                  |
| ---------- | ------------------------ |
| DTO        | Data transfer object     |
| Service    | Business processing      |
| Repository | Database interaction     |
| Entity     | Database model           |
| DbContext  | EF Core database manager |

---

# Example Structure

---

# Controller

```csharp id="jlwmwe"
public class UserController
{
    private readonly UserService _service;
}
```

Handles request.

---

# Service

```csharp id="jlwmwf"
public class UserService
{
    public void Register()
    {
        // business logic
    }
}
```

---

# Repository

```csharp id="jlwmwg"
public class UserRepository
{
    public void Save()
    {
        // database logic
    }
}
```

---

# Real Applications Using Multi-Tier

* Banking systems
* E-commerce platforms
* ERP systems
* Hospital management
* Payment systems
* CRM software

---

# Common Architecture in Modern C#

```text id="jlwmwh"
Frontend
   ↓
API Controller
   ↓
Service Layer
   ↓
Repository Layer
   ↓
Database
```

---

# Final Understanding

Multi-tier architecture is mainly about:

```text id="jlwmwi"
Clean responsibility separation
```

So applications become:

* Maintainable
* Scalable
* Testable
* Reusable
* Team-friendly

---

# Quick Recall

```text id="jlwmwj"
Presentation Layer
 → User interaction

Business Layer
 → Business logic

Data Access Layer
 → Database operations

Database Layer
 → Stores data

Goal
 → Separation of responsibilities
```
