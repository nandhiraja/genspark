# RDBMS Schema Design — Complete Learning Notes

# 1. What is RDBMS?

RDBMS = Relational Database Management System

Data is stored in:

```text id="5izl4p"
Tables → Rows → Columns
```

Examples:

* PostgreSQL
* MySQL
* Oracle Database

---

# 2. Why Database Design Matters

Good database design helps:

* Reduce duplicate data
* Maintain relationships
* Prevent invalid data
* Improve query performance
* Make scaling easier

Bad design causes:

```text id="saz0az"
Duplicate data
Broken relationships
Slow queries
Difficult updates
Inconsistent records
```

---

# 3. Core Model

Think in:

```text id="ewt3b8"
Entities + Relationships
```

Example:

```text id="vnd8fe"
Customer places Orders
Order contains Products
Product belongs to Category
```

Tables represent entities.

---

# 4. Main Database Components

| Component  | Meaning                    |
| ---------- | -------------------------- |
| Table      | Stores data                |
| Row        | Single record              |
| Column     | Attribute/field            |
| Schema     | Overall database structure |
| Constraint | Rules for data             |
| Key        | Identifies/connects data   |

---

# 5. Table Design

Example:

```sql id="v1jlwm"
CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    customer_name VARCHAR(100),
    email VARCHAR(100)
);
```

---

# 6. Data Types

| Type        | Usage        |
| ----------- | ------------ |
| `INT`       | Numbers      |
| `VARCHAR`   | Short text   |
| `TEXT`      | Large text   |
| `DATE`      | Date only    |
| `TIMESTAMP` | Date + time  |
| `BOOLEAN`   | true/false   |
| `DECIMAL`   | Money values |

Example:

```sql id="6v9m6o"
price DECIMAL(10,2)
```

---

# 7. Keys

Keys are used to:

```text id="9rj7kc"
Identify data
Connect tables
Maintain uniqueness
```

---

# 8. Primary Key (PK)

## Purpose

Uniquely identifies each row.

Example:

```sql id="e93x2f"
customer_id PRIMARY KEY
```

Rules:

* Unique
* Not NULL
* One PK per table

---

# 9. Foreign Key (FK)

## Purpose

Creates relationship between tables.

Example:

```text id="n3n7m0"
orders.customer_id
→ customers.customer_id
```

```sql id="2x9mfd"
FOREIGN KEY (customer_id)
REFERENCES customers(customer_id)
```

---

# 10. Candidate Key

Columns that can uniquely identify rows.

Example:

```text id="sdj0fi"
email
phone_number
aadhaar_number
```

One candidate key becomes Primary Key.

---

# 11. Unique Key

Prevents duplicate values.

```sql id="5xh8zt"
email VARCHAR(100) UNIQUE
```

---

# 12. Composite Key

Multiple columns together form uniqueness.

Example:

```text id="j4s3ym"
order_id + product_id
```

Used in:

```text id="03gbb8"
order_details
```

---

# 13. Super Key

Any column combination that uniquely identifies rows.

Example:

```text id="rjlwm0"
customer_id
customer_id + email
```

---

# 14. Constraints

Used to enforce rules.

| Constraint  | Purpose         |
| ----------- | --------------- |
| PRIMARY KEY | Unique row      |
| FOREIGN KEY | Relationship    |
| NOT NULL    | Required value  |
| UNIQUE      | No duplicates   |
| CHECK       | Restrict values |
| DEFAULT     | Default value   |

---

# 15. NOT NULL

```sql id="49o5hl"
name VARCHAR(100) NOT NULL
```

Value cannot be empty.

---

# 16. CHECK Constraint

Controls allowed values.

```sql id="hnkgfd"
CHECK(status IN ('PAID','PENDING'))
```

---

# 17. DEFAULT Constraint

Provides default value.

```sql id="nj5vr0"
status VARCHAR(20) DEFAULT 'PENDING'
```

---

# 18. Relationships

---

## One-to-One (1:1)

Example:

```text id="4w3m6t"
User ↔ Passport
```

---

## One-to-Many (1:M)

Most common.

Example:

```text id="psjwnu"
One Customer
→ Many Orders
```

---

## Many-to-Many (M:M)

Example:

```text id="es3rmd"
Many Students
↔ Many Courses
```

Needs bridge table.

Example:

```text id="6pj5kz"
student_courses
```

---

# 19. Junction / Bridge Table

Used for Many-to-Many relationships.

Example:

```text id="0h3t9i"
order_details
```

Contains:

```text id="zyw55r"
order_id
product_id
quantity
```

---

# 20. Normalization

## Purpose

Reduce duplicate data.

---

# 21. First Normal Form (1NF)

Rules:

* No multiple values in one column
* Atomic values only

❌ Bad

```text id="5uql1m"
skills = Java, Python
```

✅ Good

```text id="e8mecw"
One value per field
```

---

# 22. Second Normal Form (2NF)

Rules:

* Must be in 1NF
* No partial dependency

Meaning:

```text id="50mjlwm"
Columns must depend on full primary key
```

---

# 23. Third Normal Form (3NF)

Rules:

* Must be in 2NF
* No transitive dependency

❌ Bad

```text id="w25dhj"
student_id
department_id
department_name
```

✅ Better

```text id="ru7uvr"
Separate department table
```

---

# 24. BCNF

Advanced normalization.

Every determinant must be candidate key.

Mostly used in:

```text id="vjlwmx"
Complex enterprise systems
```

---

# 25. Denormalization

Sometimes duplicate data intentionally added for:

```text id="jlwmrn"
Performance optimization
```

Used in:

* Analytics systems
* Reporting systems
* Large-scale applications

Tradeoff:

```text id="cv1i2g"
More speed
Less normalization
```

---

# 26. Indexes

## Purpose

Improve query speed.

Without index:

```text id="cjlwm7"
Full table scan
```

With index:

```text id="x5t3v8"
Fast lookup
```

Example:

```sql id="jlwmx9"
CREATE INDEX idx_customer_email
ON customers(email);
```

---

# 27. Schema Design Process

## Real Thinking Flow

```text id="jlwm4v"
1. Identify entities
2. Identify relationships
3. Create tables
4. Add keys
5. Add constraints
6. Normalize
7. Optimize queries
```

---

# 28. Naming Conventions

Use:

```text id="jlwm0y"
snake_case
```

Examples:

```text id="mjlwm7"
customer_id
order_date
product_name
```

---

# 29. Common Production Tables

| Table      | Purpose          |
| ---------- | ---------------- |
| users      | User data        |
| products   | Product info     |
| orders     | Order records    |
| payments   | Payment info     |
| categories | Product grouping |

---

# 30. Cascading

## ON DELETE CASCADE

If parent row deleted:

```text id="jlwm3r"
Child rows auto-delete
```

Example:

```sql id="jlwm2q"
FOREIGN KEY (customer_id)
REFERENCES customers(customer_id)
ON DELETE CASCADE
```

---

# 31. Transactions

Used for safe operations.

Example:

```text id="jlwmq4"
Money transfer
```

Operations:

```text id="jlwm8f"
BEGIN
COMMIT
ROLLBACK
```

---

# 32. ACID Properties

| Property    | Meaning                |
| ----------- | ---------------------- |
| Atomicity   | All or nothing         |
| Consistency | Valid data             |
| Isolation   | Transactions separated |
| Durability  | Data persists          |

---

# 33. ER Diagram

ER = Entity Relationship Diagram

Visual representation of:

```text id="jlwmz1"
Tables
Keys
Relationships
```

---

# 34. Thinking

## Beginner 

```text id="jlwmr2"
Just create table
```

## Production 

```text id="jlwmk7"
Can this scale?
Will joins become slow?
Is data duplicated?
Are indexes needed?
```

---

# 35. Common Mistakes

❌ No foreign keys
❌ Duplicate data everywhere
❌ Wrong relationships
❌ Using VARCHAR for everything
❌ No indexes
❌ No constraints

---



```text id="jlwmf6"
Tables store entities
Keys identify data
Foreign keys connect data
Constraints protect data
Normalization cleans structure
Indexes improve speed
Transactions protect consistency
```

---

# 37. Topics Covered

✅ Tables
✅ Rows & Columns
✅ Data Types
✅ Primary Key
✅ Foreign Key
✅ Candidate Key
✅ Composite Key
✅ Constraints
✅ Relationships
✅ Normalization
✅ Indexes
✅ Transactions
✅ ACID
✅ ER Diagrams
✅ Cascading
