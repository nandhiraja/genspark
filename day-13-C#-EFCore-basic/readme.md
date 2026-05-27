# EF Core — Basic Notes

---

# What is EF Core?

EF Core means:

```text id="h2a8x1"
Entity Framework Core
```

It is a:

```text id="k3d9v2"
ORM (Object Relational Mapper)
```

used in C# applications.

---

# What is ORM?

ORM helps:

```text id="m7f1p4"
Convert database tables ↔ C# objects
```

Instead of writing SQL manually,
we work using C# classes and objects.

---

# Without EF Core

Using ADO.NET:

```text id="t4r8y6"
Open connection
Write SQL
Execute query
Read data manually
Map values manually
```

More boilerplate code.

---

# With EF Core

We simply write:

```csharp id="p9s2d7"
var users = dbContext.Users.ToList();
```

EF Core internally:

* Creates SQL query
* Executes query
* Maps result into objects

---

# Why EF Core is Used

EF Core helps reduce:

* Manual SQL writing
* Boilerplate code
* Manual mapping
* Database handling complexity

---

# Main Benefits

| Benefit                  | Meaning            |
| ------------------------ | ------------------ |
| Faster development       | Less code          |
| Object-oriented approach | Work with classes  |
| Automatic mapping        | Table ↔ object     |
| LINQ support             | Query using C#     |
| Database abstraction     | Easier maintenance |

---

# Real Flow

```text id="q8u5l3"
C# Classes
     ↓
EF Core
     ↓
SQL Queries
     ↓
Database
```

---

# Main Things EF Core Handles

EF Core mainly manages:

* Database connection
* SQL generation
* CRUD operations
* Object mapping
* Change tracking
* Relationships

---

# Core Idea of EF Core

---

# Table → Class

Database table:

```text id="w1n6k8"
Users
```

becomes:

```csharp id="b5r3m9"
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

---

# Row → Object

Database row:

```text id="g7t4x2"
1 | John
```

becomes:

```csharp id="z2c8v5"
new User
{
    Id = 1,
    Name = "John"
}
```

---

# Main Components in EF Core

---

# 1. Entity

Entity means:

```text id="u4m7n1"
C# class representing database table
```

---

## Example

```csharp id="l8p2q6"
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}
```

Represents:

```text id="d5w9k3"
Products table
```

---

# 2. DbContext

Most important EF Core class.

DbContext acts like:

```text id="n1x4r7"
Main bridge between application and database
```

Handles:

* Database connection
* Query execution
* Entity tracking

---

## Example

```csharp id="f6k2m8"
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
}
```

---

# 3. DbSet

DbSet represents:

```text id="r3v7p1"
Table collection
```

Used for CRUD operations.

---

## Example

```csharp id="j9t5w4"
dbContext.Users
```

Represents:

```text id="h4m8n6"
Users table
```

---

# 4. Provider

Provider connects EF Core with specific database.

Examples:

| Database   | Provider                                |
| ---------- | --------------------------------------- |
| SQL Server | Microsoft.EntityFrameworkCore.SqlServer |
| PostgreSQL | Npgsql.EntityFrameworkCore.PostgreSQL   |

---

# 5. LINQ

EF Core heavily uses LINQ.

Example:

```csharp id="v7n2k5"
var users =
    dbContext.Users
             .Where(x => x.Age > 18)
             .ToList();
```

EF Core converts this into SQL.

---

# Basic EF Core Flow

```text id="x8m3q1"
Create Entity Class
        ↓
Create DbContext
        ↓
Configure Database
        ↓
Use DbSet
        ↓
Perform CRUD Operations
```

---

# Simple EF Core Setup

---

# Install Packages

For PostgreSQL:

```text id="k2v7p9"
Microsoft.EntityFrameworkCore
Npgsql.EntityFrameworkCore.PostgreSQL
Microsoft.EntityFrameworkCore.Tools
```

---

# Create Entity

```csharp id="m6q1w8"
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

---

# Create DbContext

```csharp id="t4n8k2"
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Your Connection String");
    }
}
```

---

# Insert Data

```csharp id="g5m2v7"
using var db = new AppDbContext();

var user = new User
{
    Name = "John"
};

db.Users.Add(user);

db.SaveChanges();
```

---

# Read Data

```csharp id="q1t8n4"
var users = db.Users.ToList();
```

---

# Update Data

```csharp id="b7k3m5"
var user = db.Users.First();

user.Name = "Sam";

db.SaveChanges();
```

---

# Delete Data

```csharp id="z4m9q2"
var user = db.Users.First();

db.Users.Remove(user);

db.SaveChanges();
```

---

# Why SaveChanges() Important

EF Core tracks changes internally.

```text id="v1n6m8"
SaveChanges()
```

actually sends SQL query to database.

Without it:

```text id="x3p7q4"
Changes stay only in memory
```

---

# Change Tracking

EF Core automatically tracks:

* Added objects
* Modified objects
* Deleted objects

Then generates proper SQL.

---

# Query Example

```csharp id="h8q2m1"
var users =
    db.Users
      .Where(x => x.Name == "John")
      .ToList();
```

Generated SQL internally:

```sql id="p4m7k6"
SELECT * FROM Users
WHERE Name = 'John'
```

---

# Main Approaches in EF Core

---

# 1. Code First

Most commonly used.

Flow:

```text id="n7m3v2"
Create C# classes
      ↓
EF Core creates database
```

---

# 2. Database First

Flow:

```text id="j2k8m5"
Existing database
      ↓
Generate C# classes
```

---

# Most Commonly Used in Modern Projects

```text id="t5q9m1"
Code First
```

because developers control models from code.

---

# Migrations

Migration means:

```text id="r8m2k4"
Tracking database schema changes
```

Example:

* Added new column
* Added table
* Changed datatype

EF Core migration generates SQL automatically.

---

# Migration Commands

---

# Create Migration

```bash id="z6n3p7"
Add-Migration InitialCreate
```

---

# Apply Migration

```bash id="y1m8v4"
Update-Database
```

---

# Relationships in EF Core

EF Core supports:

| Relationship | Example             |
| ------------ | ------------------- |
| One-to-One   | User ↔ Profile      |
| One-to-Many  | Category ↔ Products |
| Many-to-Many | Student ↔ Courses   |

---

# Navigation Properties

Used for relationships.

---

## Example

```csharp id="m4p9k2"
public class Order
{
    public int Id { get; set; }

    public Customer Customer { get; set; }
}
```

---

# Fluent API

Used for advanced configuration.

Done inside:

```csharp id="k7n2v5"
OnModelCreating()
```

Used for:

* Relationships
* Constraints
* Table names
* Keys

---

# Data Annotations

Simple configuration using attributes.

Example:

```csharp id="v2m8q1"
[Required]
[StringLength(50)]
```

---

# EF Core in Real Projects

Usually used inside:

```text id="q6n1m3"
Repository Layer
Service Layer
```

Architecture flow:

```text id="r4k8m7"
Controller
   ↓
Service
   ↓
Repository
   ↓
EF Core
   ↓
Database
```

---

# Why Companies Prefer EF Core

Because it improves:

* Productivity
* Maintainability
* Development speed
* Readability

Especially for:

* APIs
* Enterprise applications
* Business systems

---

# Important Things to Know

---

## EF Core Does NOT Remove SQL

It only generates SQL automatically.

Understanding SQL still important.

---

## DbContext Should Be Short-Lived

Usually:

```text id="n9v3k5"
One request → one DbContext
```

---

## LINQ Queries Become SQL

Bad LINQ can create bad SQL.

Performance still matters.

---

## SaveChanges() Hits Database

Each SaveChanges call may execute queries.

Avoid unnecessary calls.

---

# Final 

EF Core mainly helps:

```text id="w7m2q8"
Work with database
using C# objects instead of manual SQL
```

It provides:

* ORM mapping
* LINQ querying
* CRUD operations
* Change tracking
* Relationship handling
* Migration system

---

# Quick Recall

```text id="p2n8m4"
EF Core
 → ORM for C#

Main Components
 → Entity
 → DbContext
 → DbSet
 → Provider
 → LINQ

Main Features
 → CRUD
 → Migrations
 → Relationships
 → Change Tracking

Most Used Approach
 → Code First
```
