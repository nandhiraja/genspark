# ADO.NET in C# —  Notes

---

# What is ADO.NET?

ADO.NET is a technology in C# used to:

```text id="k9u4o1"
Connect application with database
```

Using ADO.NET we can:

* Connect database
* Read data
* Insert data
* Update data
* Delete data

---

# Why ADO.NET is Needed

Applications need data storage.

Example:

* Users
* Orders
* Products
* Payments
* Employees

All stored inside database.

ADO.NET acts like:

```text id="jlwmx1"
Bridge between C# application and database
```

---

# Real Flow

```text id="jlwmx2"
C# Application
      ↓
ADO.NET
      ↓
Database
```

---

# Databases Commonly Used

| Database   | Provider       |
| ---------- | -------------- |
| SQL Server | SqlClient      |
| PostgreSQL | Npgsql         |
| MySQL      | MySqlConnector |

---

# What ADO.NET Mainly Handles

ADO.NET helps with:

* Opening connection
* Sending SQL queries
* Reading result
* Executing commands
* Managing transactions

---

# Main Components of ADO.NET

---

# 1. Connection

Used to connect database.

Example:

```text id="jlwmx3"
SqlConnection
NpgsqlConnection
```

Responsibility:

```text id="jlwmx4"
Open database connection
```

---

# 2. Command

Used to execute SQL query.

Example:

```text id="jlwmx5"
SqlCommand
NpgsqlCommand
```

Responsibility:

```text id="jlwmx6"
Send SQL to database
```

---

# 3. DataReader

Used to read data row by row.

Example:

```text id="jlwmx7"
SqlDataReader
```

Responsibility:

```text id="jlwmx8"
Fast forward-only data reading
```

---

# 4. DataAdapter

Used to fill data into memory objects.

Mostly used with:

```text id="jlwmx9"
DataSet
DataTable
```

---

# 5. DataSet

Temporary in-memory database.

Can store:

* Multiple tables
* Relations
* Data in RAM

---

# Core ADO.NET Flow

```text id="jlwmya"
Create Connection
      ↓
Open Connection
      ↓
Create Command
      ↓
Execute Query
      ↓
Read Result
      ↓
Close Connection
```

---

# Connection String

Connection string contains database details.

Example:

```csharp id="jlwmyb"
string connectionString =
"Host=localhost;
Port=5432;
Database=TestDB;
Username=postgres;
Password=1234";
```

---

# What Connection String Contains

| Part     | Meaning         |
| -------- | --------------- |
| Host     | Database server |
| Port     | Database port   |
| Database | Database name   |
| Username | DB username     |
| Password | DB password     |

---

# Simple ADO.NET Connection Example

---

# PostgreSQL Example Using Npgsql

```csharp id="jlwmyc"
using Npgsql;

string connectionString =
"Host=localhost;
Port=5432;
Database=TestDB;
Username=postgres;
Password=1234";

using var connection =
    new NpgsqlConnection(connectionString);

connection.Open();

Console.WriteLine("Connected");
```

---

# Why using Used

```text id="jlwmyd"
Automatically closes connection
```

Prevents memory/resource leaks.

---

# Simple SELECT Query

---

## Read Data from Database

```csharp id="jlwmye"
using Npgsql;

string connectionString =
"Host=localhost;
Port=5432;
Database=TestDB;
Username=postgres;
Password=1234";

using var connection =
    new NpgsqlConnection(connectionString);

connection.Open();

string query =
"SELECT id, name FROM users";

using var command =
    new NpgsqlCommand(query, connection);

using var reader =
    command.ExecuteReader();

while(reader.Read())
{
    Console.WriteLine(
        reader["id"]);

    Console.WriteLine(
        reader["name"]);
}
```

---

# Important Methods

| Method            | Purpose              |
| ----------------- | -------------------- |
| Open()            | Open DB connection   |
| ExecuteReader()   | Read data            |
| ExecuteNonQuery() | Insert/Update/Delete |
| ExecuteScalar()   | Single value         |

---

# Insert Data Example

---

## INSERT Query

```csharp id="jlwmyf"
string query =
"INSERT INTO users(name) VALUES('John')";
```

Execution:

```csharp id="jlwmyg"
using var command =
    new NpgsqlCommand(query, connection);

int rows =
    command.ExecuteNonQuery();
```

---

# ExecuteNonQuery()

Used for:

* INSERT
* UPDATE
* DELETE

Returns:

```text id="jlwmyh"
Affected row count
```

---

# Update Example

```csharp id="jlwmyi"
string query =
"UPDATE users
 SET name='Sam'
 WHERE id=1";
```

---

# Delete Example

```csharp id="jlwmyj"
string query =
"DELETE FROM users
 WHERE id=1";
```

---

# ExecuteScalar()

Used when query returns single value.

---

## Example

```csharp id="jlwmyk"
string query =
"SELECT COUNT(*) FROM users";

using var command =
    new NpgsqlCommand(query, connection);

int total =
    Convert.ToInt32(
        command.ExecuteScalar());
```

---

# SQL Injection Problem

Bad approach:

```csharp id="jlwmyl"
string query =
"SELECT * FROM users
 WHERE name='" + name + "'";
```

Danger:

```text id="jlwmym"
SQL Injection Attack
```

---

# Parameterized Query

Safe approach.

---

## Example

```csharp id="jlwmyn"
string query =
"SELECT * FROM users
 WHERE name=@name";

using var command =
    new NpgsqlCommand(query, connection);

command.Parameters.AddWithValue(
    "@name",
    name);
```

---

# Why Parameterized Queries Important

Prevents:

* SQL injection
* Query breaking
* Security issues

---

# DataReader Characteristics

DataReader is:

| Feature      | Meaning               |
| ------------ | --------------------- |
| Fast         | High performance      |
| Read-only    | Cannot modify         |
| Forward-only | One direction reading |

---

# Connection Handling Rule

Always:

```text id="jlwmyo"
Open late
Close early
```

Avoid keeping connections open long time.

---

# Common ADO.NET Classes

| Purpose | SQL Server | PostgreSQL |
|---------|------------|---|
| Connection | SqlConnection | NpgsqlConnection |
| Command | SqlCommand | NpgsqlCommand |
| Reader | SqlDataReader | NpgsqlDataReader |

---



# Basic CRUD Flow


### Create

```text id="jlwmyp"
INSERT
```

---

### Read

```text id="jlwmyq"
SELECT
```

---

### Update

```text id="jlwmyr"
UPDATE
```

---

### Delete

```text id="jlwmys"
DELETE
```


---

# Example Flow

```text id="rgctxyt"
Controller
   ↓
Service
   ↓
Repository
   ↓
ADO.NET
   ↓
Database
```

---

# Real Applications Using ADO.NET

* ERP systems
* Banking applications
* Billing software
* Legacy enterprise systems
* High-performance systems

---

# ADO.NET vs EF Core

| ADO.NET          | EF Core            |
| ---------------- | ------------------ |
| Manual SQL       | ORM                |
| More control     | Faster development |
| High performance | Easier coding      |
| More code        | Less boilerplate   |

---

# Why Learn ADO.NET Even Today

Because it teaches:

* Database fundamentals
* SQL execution flow
* Connection management
* Transactions
* Performance understanding

Even EF Core internally depends on database providers.

---

## Common Problems Beginners Make

---

### Forgetting to Close Connection

Causes resource leaks.

---

### Writing Raw SQL Everywhere

Creates maintainability issues.

---

### Not Using Parameters

Creates security vulnerabilities.

---

### Mixing Business Logic with SQL

Makes code messy.

---

## Simple Repository Example

```csharp id="jlwmyu"
public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    public void AddUser(string name)
    {
        using var connection =
            new NpgsqlConnection(
                _connectionString);

        connection.Open();

        string query =
        "INSERT INTO users(name)
         VALUES(@name)";

        using var command =
            new NpgsqlCommand(
                query,
                connection);

        command.Parameters.AddWithValue(
            "@name",
            name);

        command.ExecuteNonQuery();
    }
}
```

---

# Final

ADO.NET mainly helps:

```text id="jlwmyv"
C# Application
       ↔
Database Communication
```

It provides:

* Connection management
* Query execution
* Data reading
* Transaction handling

---

#  Recall

```text id="jlwmyw"
ADO.NET
 → Database connectivity technology

Main Components
 → Connection
 → Command
 → DataReader
 → DataAdapter
 → DataSet

Important Methods
 → Open()
 → ExecuteReader()
 → ExecuteNonQuery()
 → ExecuteScalar()

Best Practice
 → Use parameterized queries
 → Close connection properly
```
