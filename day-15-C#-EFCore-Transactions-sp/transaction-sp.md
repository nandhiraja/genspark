# Transactions & Stored Procedures in EF Core —  Notes

---

# What is a Transaction?

Transaction means:

```text id="tr1"
Multiple database operations treated as ONE unit
```

Either:

```text id="tr2"
All operations succeed
```

OR

```text id="tr3"
Everything rollback
```

No partial save.

---

# Why Transactions are Needed

Real applications often do:

* Multiple inserts
* Multiple updates
* Money transfer
* Stock update
* Order creation

If one operation fails:

```text id="tr4"
Database should remain consistent
```

---

# Real Scenario — Banking

Money transfer:

```text id="tr5"
Deduct from Account A
      +
Add to Account B
```

If server crashes after deducting:

```text id="tr6"
Money lost
```

Transaction prevents this issue.

---

# Transaction Flow

```text id="tr7"
Start Transaction
       ↓
Execute Operations
       ↓
Success?
  ↓           ↓
Yes          No
 ↓            ↓
Commit      Rollback
```

---

# Important Terms

| Term              | Meaning                  |
| ----------------- | ------------------------ |
| Begin Transaction | Start transaction        |
| Commit            | Save changes permanently |
| Rollback          | Undo changes             |

---

# EF Core Transaction Support

EF Core already uses:

```text id="tr8"
Automatic transaction
```

inside:

```csharp id="tr9"
SaveChanges()
```

for single save operation.

---

# Why Manual Transaction Needed

Needed when:

```text id="tr10"
Multiple SaveChanges()
Multiple operations
External operations
```

must behave as single unit.

---

# Simple Transaction Example

---

# Scenario

Create:

* Order
* Payment

Both must succeed together.

---

# Example

```csharp id="tr11"
using var transaction =
    await db.Database.BeginTransactionAsync();

try
{
    db.Orders.Add(order);

    await db.SaveChangesAsync();

    db.Payments.Add(payment);

    await db.SaveChangesAsync();

    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();

    throw;
}
```

---

# Understanding Flow

---

# BeginTransactionAsync()

Starts transaction.

---

# SaveChangesAsync()

Executes SQL queries.

---

# CommitAsync()

Permanently saves changes.

---

# RollbackAsync()

Undo all operations.

---

# Why try-catch Important

If exception occurs:

```text id="tr12"
Rollback protects database consistency
```

---

# Best Practice for Transactions

---

# Keep Transaction Small

Good:

```text id="tr13"
Only DB operations inside transaction
```

Avoid:

* Long loops
* API calls
* File uploads

inside transaction.

---

# Why?

Long transaction causes:

* Table locking
* Slow performance
* Deadlocks

---

# One Business Operation = One Transaction

Example:

```text id="tr14"
Place Order
```

should be single transaction.

---

# Always Use try-catch

Ensures rollback happens properly.

---

# Dispose Transaction Properly

Using:

```csharp id="tr15"
using var transaction
```

helps automatic cleanup.

---

# Common Transaction Use Cases

| Scenario           | Why Transaction Needed |
| ------------------ | ---------------------- |
| Banking transfer   | Data consistency       |
| Order placement    | Multiple inserts       |
| Inventory update   | Prevent mismatch       |
| Payment processing | Atomic operation       |

---

# Transaction Isolation Levels

Controls how transactions interact.

Basic idea:

```text id="tr16"
Prevent dirty/conflicting reads
```

---

# Common Isolation Levels

| Level           | Purpose            |
| --------------- | ------------------ |
| ReadCommitted   | Default safe level |
| Serializable    | Strict locking     |
| ReadUncommitted | Fast but unsafe    |

---

# Usually Used

Most applications use:

```text id="tr17"
ReadCommitted
```

default.

---

# What is Stored Procedure (SP)?

Stored Procedure means:

```text id="tr18"
SQL logic stored inside database
```

Instead of sending large SQL from application.

---

# Why Stored Procedures are Used

Used for:

* Complex SQL
* Performance
* Security
* Reusable DB logic

---

# Real Scenario

Instead of writing:

```sql id="tr19"
INSERT
UPDATE
JOIN
VALIDATION
```

inside C# repeatedly,

store inside database once.

---

# Example Stored Procedure

PostgreSQL example:

```sql id="tr20"
CREATE OR REPLACE PROCEDURE add_user(
    p_name TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO users(name)
    VALUES(p_name);
END;
$$;
```

---

# What This Does

Creates procedure:

```text id="tr21"
add_user
```

which inserts user.

---

# Calling SP from EF Core

---

# Execute Procedure

```csharp id="tr22"
await db.Database.ExecuteSqlRawAsync(
    "CALL add_user({0})",
    "John");
```

---

# Why Use ExecuteSqlRaw

Used for:

```text id="tr23"
Raw SQL execution
```

---

# Stored Procedure Returning Data

Example:

```sql id="tr24"
CREATE OR REPLACE FUNCTION
get_users()
RETURNS TABLE(id INT, name TEXT)
AS $$
BEGIN
    RETURN QUERY
    SELECT id, name FROM users;
END;
$$ LANGUAGE plpgsql;
```

---

# Reading SP Data

```csharp id="tr25"
var users =
    await db.Users
        .FromSqlRaw(
            "SELECT * FROM get_users()")
        .ToListAsync();
```

---

# Important EF Core Raw SQL Methods

| Method                 | Purpose                  |
| ---------------------- | ------------------------ |
| ExecuteSqlRaw          | Insert/update/delete     |
| FromSqlRaw             | Read data                |
| ExecuteSqlInterpolated | Safer parameter handling |

---

# Parameterized Stored Procedure Call

Good practice:

```csharp id="tr26"
await db.Database.ExecuteSqlInterpolatedAsync(
    $"CALL add_user({"John"})");
```

---

# Why Parameterization Important

Prevents:

```text id="tr27"
SQL Injection
```

---

# When Companies Use Stored Procedures

Usually used for:

* Heavy reports
* Financial calculations
* Bulk operations
* Legacy enterprise systems
* Performance-critical queries

---

# When NOT to Use Too Much SP

Avoid placing:

```text id="tr28"
Entire business logic
```

inside database.

Modern systems usually keep:

```text id="tr29"
Business logic in application layer
```

---

# EF Core vs Stored Procedure

| EF Core LINQ      | Stored Procedure                  |
| ----------------- | --------------------------------- |
| Easier coding     | Better for complex SQL            |
| More maintainable | More DB-centric                   |
| Good readability  | Better for some performance cases |

---

# Recommended Modern Approach

Usually:

```text id="tr30"
Simple CRUD → EF Core LINQ

Complex reports/heavy SQL
→ Stored Procedures
```

---

# Best Practice for Stored Procedures

---

# Keep SP Focused

One procedure = one responsibility.

---

# Use Proper Naming

Good:

```text id="tr31"
sp_CreateOrder
sp_GetUserSummary
```

---

# Always Parameterize

Avoid string concatenation.

---

# Keep Business Logic Mostly in Service Layer

Avoid moving entire application logic into database.

---

# Use Transactions with SP When Needed

Critical operations should still use transaction.

---

# Real Architecture Flow

```text id="tr32"
Controller
   ↓
Service
   ↓
Repository
   ↓
EF Core
   ↓
Stored Procedure / SQL
   ↓
Database
```

---

# Example Combined Flow

---

# Place Order

Inside transaction:

```text id="tr33"
Create Order
Update Inventory
Insert Payment
Call Invoice SP
```

All succeed together.

---

# Final Understanding

---

# Transaction

Helps maintain:

```text id="tr34"
Data consistency
```

during multiple operations.

---

# Stored Procedure

Helps move:

```text id="tr35"
Complex SQL logic
```

into database.

---

# Quick Recall

```text id="tr36"
Transaction
 → Multiple DB operations as one unit

Main Operations
 → Commit
 → Rollback

EF Core Methods
 → BeginTransaction()
 → Commit()
 → Rollback()

Stored Procedure
 → SQL logic stored in DB

EF Core SP Methods
 → ExecuteSqlRaw()
 → FromSqlRaw()

Best Practice
 → Keep transaction small
 → Parameterize queries
 → Use SP for complex SQL
```
