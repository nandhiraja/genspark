# Fluent API in EF Core —  Notes

---

# What is Fluent API?

Fluent API is used to:

```text id="fa1"
Configure database behavior using C# code
```

inside EF Core.

Used mainly for:

* Relationships
* Constraints
* Keys
* Table mapping
* Column mapping
* Validation rules

---

# Why Fluent API is Needed

EF Core can automatically guess many things.

But real applications need:

* Precise control
* Custom relationships
* Database rules
* Naming control
* Advanced mapping

Fluent API gives:

```text id="fa2"
Full control over database mapping
```

---

# Where Fluent API is Written

Inside:

```csharp id="fa3"
OnModelCreating()
```

method of DbContext.

---

# Basic Structure

```csharp id="fa4"
protected override void OnModelCreating(
    ModelBuilder modelBuilder)
{
}
```

---

# What is ModelBuilder?

`ModelBuilder` is used to:

```text id="fa5"
Configure entities and relationships
```

---

# Main Usage Areas of Fluent API

| Purpose        | Example            |
| -------------- | ------------------ |
| Table mapping  | Custom table names |
| Primary keys   | Define keys        |
| Relationships  | One-to-many        |
| Constraints    | Required fields    |
| Column config  | Length/type        |
| Default values | Created date       |
| Composite keys | Multiple PKs       |

---

# Why Companies Prefer Fluent API

Because:

```text id="fa6"
All database configuration stays centralized
```

instead of spreading attributes everywhere.

Better for:

* Large projects
* Team projects
* Complex relationships

---

# Data Annotation vs Fluent API

---

# Data Annotation

Uses attributes.

```csharp id="fa7"
[Required]
[StringLength(50)]
```

Good for:

```text id="fa8"
Simple configurations
```

---

# Fluent API

Uses code configuration.

```csharp id="fa9"
modelBuilder.Entity<User>()
    .Property(x => x.Name)
    .HasMaxLength(50)
    .IsRequired();
```

Good for:

```text id="fa10"
Advanced configurations
```

---

# Important Rule

If both exist:

```text id="fa11"
Fluent API overrides Data Annotations
```

---

# Entity Configuration Flow

```text id="fa12"
Entity Class
      ↓
DbContext
      ↓
OnModelCreating()
      ↓
Fluent API Configuration
      ↓
Migration
      ↓
Database Schema
```

---

# Basic Entity Example

```csharp id="fa13"
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

---

# Table Configuration

---

# Change Table Name

```csharp id="fa14"
modelBuilder.Entity<User>()
    .ToTable("tbl_users");
```

---

# Why Use This?

Useful when:

* Existing database naming
* Company naming standards
* Legacy systems

---

# Primary Key Configuration

---

# Simple Primary Key

```csharp id="fa15"
modelBuilder.Entity<User>()
    .HasKey(x => x.Id);
```

---

# Composite Key

Multiple columns as key.

```csharp id="fa16"
modelBuilder.Entity<OrderItem>()
    .HasKey(x =>
        new { x.OrderId, x.ProductId });
```

---

# When Composite Key Used

Usually in:

* Mapping tables
* Many-to-many tables
* Junction tables

---

# Property Configuration

---

# Required Column

```csharp id="fa17"
modelBuilder.Entity<User>()
    .Property(x => x.Name)
    .IsRequired();
```

---

# Max Length

```csharp id="fa18"
modelBuilder.Entity<User>()
    .Property(x => x.Name)
    .HasMaxLength(100);
```

---

# Column Name

```csharp id="fa19"
modelBuilder.Entity<User>()
    .Property(x => x.Name)
    .HasColumnName("user_name");
```

---

# Column Type

```csharp id="fa20"
modelBuilder.Entity<Product>()
    .Property(x => x.Price)
    .HasColumnType("decimal(18,2)");
```

---

# Default Value

```csharp id="fa21"
modelBuilder.Entity<User>()
    .Property(x => x.CreatedAt)
    .HasDefaultValueSql("NOW()");
```

---

# Ignore Property

Property not stored in DB.

```csharp id="fa22"
modelBuilder.Entity<User>()
    .Ignore(x => x.TempValue);
```

---

# Relationship Configuration

Most important Fluent API usage.

---

# Types of Relationships

| Type         | Example            |
| ------------ | ------------------ |
| One-to-One   | User ↔ Profile     |
| One-to-Many  | Customer ↔ Orders  |
| Many-to-Many | Students ↔ Courses |

---

# 1. One-to-Many Relationship

---

# Real Scenario

```text id="fa23"
One Customer
can have many Orders
```

---

# Entity Example

---

## Customer

```csharp id="fa24"
public class Customer
{
    public int Id { get; set; }

    public List<Order> Orders { get; set; }
}
```

---

## Order

```csharp id="fa25"
public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; }
}
```

---

# Fluent API Mapping

```csharp id="fa26"
modelBuilder.Entity<Order>()
    .HasOne(x => x.Customer)
    .WithMany(x => x.Orders)
    .HasForeignKey(x => x.CustomerId);
```

---

# Understanding This

---

# HasOne()

Current entity has one parent.

```text id="fa27"
Order has one Customer
```

---

# WithMany()

Parent has many children.

```text id="fa28"
Customer has many Orders
```

---

# HasForeignKey()

Defines foreign key column.

```text id="fa29"
CustomerId
```

---

# When Use One-to-Many

Used in:

* User → Orders
* Category → Products
* Department → Employees

---

# 2. One-to-One Relationship

---

# Real Scenario

```text id="fa30"
One User
has one Profile
```

---

# Example

---

## User

```csharp id="fa31"
public class User
{
    public int Id { get; set; }

    public Profile Profile { get; set; }
}
```

---

## Profile

```csharp id="fa32"
public class Profile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }
}
```

---

# Fluent API

```csharp id="fa33"
modelBuilder.Entity<User>()
    .HasOne(x => x.Profile)
    .WithOne(x => x.User)
    .HasForeignKey<Profile>(x => x.UserId);
```

---

# Understanding

---

# HasOne()

User has one profile.

---

# WithOne()

Profile belongs to one user.

---

# HasForeignKey<T>()

Defines FK owner table.

---

# When Use One-to-One

Used in:

* User ↔ Profile
* Employee ↔ Passport
* Product ↔ InventoryDetail

---

# 3. Many-to-Many Relationship

---

# Real Scenario

```text id="fa34"
One Student
can join many Courses

One Course
can contain many Students
```

---

# Entity Example

---

## Student

```csharp id="fa35"
public class Student
{
    public int Id { get; set; }

    public List<Course> Courses { get; set; }
}
```

---

## Course

```csharp id="fa36"
public class Course
{
    public int Id { get; set; }

    public List<Student> Students { get; set; }
}
```

---

# Fluent API

```csharp id="fa37"
modelBuilder.Entity<Student>()
    .HasMany(x => x.Courses)
    .WithMany(x => x.Students);
```

---

# EF Core Internally Creates

```text id="fa38"
Junction Table
```

like:

```text id="fa39"
StudentCourses
```

---

# When Many-to-Many Used

Used in:

* Students ↔ Courses
* Users ↔ Roles
* Products ↔ Tags

---

# Cascade Delete

---

# What is Cascade Delete?

When parent deleted:

```text id="fa40"
Child records automatically deleted
```

---

# Example

```csharp id="fa41"
modelBuilder.Entity<Order>()
    .HasOne(x => x.Customer)
    .WithMany(x => x.Orders)
    .OnDelete(DeleteBehavior.Cascade);
```

---

# Delete Behaviors Available

| Option   | Meaning             |
| -------- | ------------------- |
| Cascade  | Delete children     |
| Restrict | Prevent delete      |
| SetNull  | FK becomes null     |
| NoAction | No automatic action |

---

# When to Use Carefully

Cascade delete dangerous in:

* Financial systems
* Audit systems

because data may disappear accidentally.

---

# Navigation Properties

Used to move between related entities.

Example:

```csharp id="fa42"
order.Customer
```

or

```csharp id="fa43"
customer.Orders
```

---

# Foreign Key

Foreign key connects tables.

Example:

```csharp id="fa44"
CustomerId
```

---

# Shadow Properties

EF Core can create hidden properties internally.

Used when FK property not explicitly defined.

---

# Index Configuration

Used for faster searching.

---

# Example

```csharp id="fa45"
modelBuilder.Entity<User>()
    .HasIndex(x => x.Email)
    .IsUnique();
```

---

# Why Use Index?

Improves:

* Search performance
* Query speed

---

# Unique Constraint

```csharp id="fa46"
.IsUnique()
```

Prevents duplicate values.

Useful for:

* Email
* Username
* Phone number

---

# Value Conversion

Used when object type differs from DB type.

---

# Example

Enum conversion.

```csharp id="fa47"
.Property(x => x.Status)
.HasConversion<string>();
```

---

# Separate Configuration Classes

Large projects usually avoid huge OnModelCreating.

Instead use:

```text id="fa48"
EntityTypeConfiguration
```

---

# Example

```csharp id="fa49"
public class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
    }
}
```

---

# Why Separate Configurations?

Better for:

* Clean architecture
* Team projects
* Large systems

---

# Applying Configurations

```csharp id="fa50"
modelBuilder.ApplyConfigurationsFromAssembly(
    Assembly.GetExecutingAssembly());
```

---

# Real Project Usage

Fluent API heavily used for:

* Enterprise applications
* ERP systems
* Banking systems
* E-commerce platforms
* Complex business systems

---

# Common Beginner Mistakes

---

# Missing Navigation Properties

Relationship becomes confusing.

---

# Wrong FK Mapping

Creates migration errors.

---

# Circular Cascade Delete

May fail migration.

---

# Huge OnModelCreating

Hard to maintain.

---

# Final Understanding

Fluent API mainly helps:

```text id="fa51"
Control how C# classes map to database
```

Especially for:

* Relationships
* Constraints
* Keys
* Database rules
* Advanced configurations

---

# Quick Recall

```text id="fa52"
Fluent API
 → Advanced EF Core configuration

Written Inside
 → OnModelCreating()

Main Usage
 → Relationships
 → Constraints
 → Keys
 → Table mapping

Relationship Methods
 → HasOne()
 → WithMany()
 → WithOne()
 → HasMany()
 → HasForeignKey()

Relationship Types
 → One-to-One
 → One-to-Many
 → Many-to-Many
```
