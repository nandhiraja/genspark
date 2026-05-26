# LINQ in C# —  Notes
---

# 1. What is LINQ?

LINQ = **Language Integrated Query**

It allows us to query and manipulate data directly inside C# using a clean syntax.

Instead of writing loops again and again:

```csharp
foreach(var item in list)
{
    if(item.Age > 18)
    {
        ...
    }
}
```

We can write:

```csharp
var result = list.Where(x => x.Age > 18);
```

---

# 2. Why LINQ Was Introduced?

Before LINQ:

* Too many loops
* Filtering logic repeated
* Code less readable
* Different query styles for DB/XML/Collections

LINQ provides:

* Common query style
* Less code
* Better readability
* Powerful data operations
* Functional-style programming

---

# 3. LINQ Can Work With

| Source            | Example          |
| ----------------- | ---------------- |
| Collections       | List, Array      |
| Database          | Entity Framework |
| XML               | XML documents    |
| JSON              | Parsed objects   |
| In-memory objects | Objects in RAM   |

---

# 4. Main LINQ Concepts

| Concept           | Purpose             |
| ----------------- | ------------------- |
| Filtering         | Select needed data  |
| Projection        | Transform data      |
| Sorting           | Order data          |
| Grouping          | Group similar data  |
| Aggregation       | Count, Sum, Average |
| Joining           | Combine data        |
| Quantifiers       | Any, All            |
| Element operators | First, Single       |

---

# 5. Two LINQ Syntax Types

---

## A) Query Syntax

Looks like SQL.

```csharp
var result =
    from emp in employees
    where emp.Salary > 50000
    select emp;
```

### Mostly Used For

* Simple readability
* SQL-like writing

---

## B) Method Syntax

Most commonly used in real projects.

```csharp
var result = employees
                .Where(x => x.Salary > 50000)
                .Select(x => x.Name);
```

### Why Mostly Used?

* More flexible
* Easy chaining
* Supports all operators

---

# 6. Important LINQ Methods

---

## Filtering

## Where()

Used to filter data.

```csharp
var result = numbers.Where(x => x > 10);
```

---

# Projection

## Select()

Transforms data.

```csharp
var result = employees.Select(x => x.Name);
```

### Convert object

```csharp
var result = employees.Select(x => new
{
    x.Name,
    x.Salary
});
```

---

# Sorting

## OrderBy()

Ascending order.

```csharp
var result = employees.OrderBy(x => x.Name);
```

---

## OrderByDescending()

```csharp
var result = employees.OrderByDescending(x => x.Salary);
```

---

## ThenBy()

Secondary sorting.

```csharp
var result = employees
                .OrderBy(x => x.Department)
                .ThenBy(x => x.Name);
```

---

# Aggregation

## Count()

```csharp
int total = employees.Count();
```

---

## Sum()

```csharp
int totalSalary = employees.Sum(x => x.Salary);
```

---

## Average()

```csharp
double avg = employees.Average(x => x.Salary);
```

---

## Max() / Min()

```csharp
int max = employees.Max(x => x.Salary);
```

---

# Element Operators

---

## First()

Returns first element.

```csharp
var emp = employees.First();
```

### Problem

Throws exception if empty.

---

## FirstOrDefault()

Safe version.

```csharp
var emp = employees.FirstOrDefault();
```

Returns:

* null
* default value

if no data exists.

---

## Single()

Used when exactly ONE record should exist.

```csharp
var user = users.Single(x => x.Id == 1);
```

### Exception if:

* No record
* Multiple records

---

## SingleOrDefault()

Safe version.

---

# Quantifiers

---

## Any()

Checks existence.

```csharp
bool exists = employees.Any(x => x.Salary > 50000);
```

---

## All()

Checks all satisfy condition.

```csharp
bool allAdults = users.All(x => x.Age >= 18);
```

---

# Grouping

## GroupBy()

```csharp
var groups = employees.GroupBy(x => x.Department);
```

---

# Joining

## Join()

Combine two collections.

```csharp
var result = employees.Join(
    departments,
    emp => emp.DepartmentId,
    dept => dept.Id,
    (emp, dept) => new
    {
        emp.Name,
        dept.DepartmentName
    });
```

---

# 7. Deferred Execution (VERY IMPORTANT)

LINQ usually executes only when data is needed.

```csharp
var result = numbers.Where(x => x > 5);
```

Here query NOT executed yet.

Execution happens when:

```csharp
foreach(var item in result)
{
}
```

OR

```csharp
result.ToList();
```

---

# 8. Immediate Execution

Methods like:

* ToList()
* ToArray()
* Count()
* First()

execute immediately.

```csharp
var data = numbers.Where(x => x > 5).ToList();
```

---

# 9. IEnumerable vs IQueryable

| IEnumerable                  | IQueryable               |
| ---------------------------- | ------------------------ |
| Works in memory              | Works in database        |
| LINQ to Objects              | LINQ to SQL/EF           |
| Faster for local collections | Optimized SQL generation |

---

## Example

### IEnumerable

```csharp
var data = employees.Where(x => x.Age > 18);
```

Filtering happens in RAM.

---

### IQueryable

```csharp
IQueryable<Employee> data = db.Employees;
```

Filtering converted into SQL query.

---

# 10. LINQ Execution Flow

```text
Data Source
    ↓
LINQ Query
    ↓
Execution
    ↓
Result Collection
```

---

# 11. Common LINQ Chain Example

```csharp
var result = employees
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    x.Salary
                })
                .ToList();
```

---

# 12. Advantages of LINQ

* Less code
* Readable
* Powerful filtering
* Easy transformation
* Strongly typed
* IntelliSense support
* Reusable queries

---

# 13. Disadvantages

* Can become complex
* Debugging harder sometimes
* Poorly written LINQ can affect performance
* Multiple enumerations issue

---

# 14. What are Extension Methods?

Extension methods allow adding methods to existing classes without modifying them.

Example:

```csharp
string name = "hello";

name.ToUpper();
```

`ToUpper()` is an extension method.

---

# 15. Why Extension Methods Matter in LINQ?

ALL LINQ methods are actually extension methods.

Example:

```csharp
numbers.Where(...)
```

`Where()` is an extension method of `IEnumerable<T>`.

---

# 16. Extension Method Rules

Must be:

* Inside static class
* Static method
* First parameter uses `this`

---

# 17. Create Custom Extension Method

---

## Example 1 — Simple Extension Method

```csharp
public static class StringExtensions
{
    public static string AddWelcome(this string name)
    {
        return "Welcome " + name;
    }
}
```

Usage:

```csharp
string result = "John".AddWelcome();

Console.WriteLine(result);
```

Output:

```text
Welcome John
```

---

# 18. Create Custom LINQ Method

---

## Example — Even Numbers Filter

```csharp
public static class CustomLinq
{
    public static IEnumerable<int> FilterEven(
        this IEnumerable<int> numbers)
    {
        foreach(var num in numbers)
        {
            if(num % 2 == 0)
            {
                yield return num;
            }
        }
    }
}
```

Usage:

```csharp
var nums = new List<int> {1,2,3,4,5,6};

var result = nums.FilterEven();

foreach(var item in result)
{
    Console.WriteLine(item);
}
```

---

# 19. Why Use yield return?

`yield return` returns values one by one lazily.

Benefits:

* Memory efficient
* Deferred execution
* LINQ-like behavior

---

# 20. Custom Generic LINQ Method

---

## Example — Custom Where

```csharp
public static class CustomLinq
{
    public static IEnumerable<T> MyWhere<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        foreach(var item in source)
        {
            if(predicate(item))
            {
                yield return item;
            }
        }
    }
}
```

Usage:

```csharp
var result = nums.MyWhere(x => x > 3);
```

---

# 21. Important Concepts Behind LINQ

LINQ internally heavily uses:

| Concept            | Why Needed           |
| ------------------ | -------------------- |
| Delegates          | Pass logic           |
| Lambda expressions | Short functions      |
| Generics           | Reusable methods     |
| Extension methods  | Add methods          |
| IEnumerable        | Collection traversal |
| yield return       | Lazy execution       |

---

# 22. LINQ Core Flow Internally

```text
Collection
   ↓
Extension Method
   ↓
Delegate/Lambda Passed
   ↓
Iterate Collection
   ↓
Return Filtered Data
```

---

# 23. Real Project Usage

LINQ heavily used in:

* Entity Framework
* ASP.NET Core
* APIs
* Data filtering
* DTO mapping
* Reporting
* Searching
* Pagination

---

# 24. Most Important LINQ Methods 

Priority order:

1. Where
2. Select
3. OrderBy
4. GroupBy
5. Any
6. FirstOrDefault
7. SelectMany
8. Join
9. Distinct
10. ToDictionary

---

# 25. Common Doubts

---

## Difference Between Select and SelectMany?

| Select                    | SelectMany          |
| ------------------------- | ------------------- |
| Returns nested collection | Flattens collection |

---

## First vs Single?

| First               | Single                |
| ------------------- | --------------------- |
| Returns first match | Expects exactly one   |
| Multiple allowed    | Multiple throws error |

---

## Deferred Execution?

Query executes only when enumerated.

---

## Why LINQ Uses Extension Methods?

To add query capability without modifying collection classes.

---

# 26. LINQ Learning Roadmap

```text
Basics
 ↓
Lambda Expressions
 ↓
Extension Methods
 ↓
IEnumerable
 ↓
Basic LINQ
 ↓
Advanced LINQ
 ↓
IQueryable
 ↓
LINQ with EF Core
 ↓
Expression Trees
```

---

# 27.  Summary

```text
LINQ
 → Query data in C#

Main Parts
 → Where
 → Select
 → OrderBy
 → GroupBy
 → Join
 → Aggregate

Syntax
 → Query Syntax
 → Method Syntax

Important Base Concepts
 → Delegates
 → Lambda
 → Generics
 → Extension Methods
 → IEnumerable

Extension Method Rules
 → static class
 → static method
 → this keyword

Custom LINQ
 → Extension method on IEnumerable<T>

yield return
 → Lazy execution
 → One by one return
```

---

# 28.  Custom LINQ Example

```csharp
using System;
using System.Collections.Generic;

public static class CustomLinq
{
    public static IEnumerable<int> GreaterThanFive(
        this IEnumerable<int> numbers)
    {
        foreach(var num in numbers)
        {
            if(num > 5)
            {
                yield return num;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var nums = new List<int>
        {
            1,2,3,6,7,8
        };

        var result = nums.GreaterThanFive();

        foreach(var item in result)
        {
            Console.WriteLine(item);
        }
    }
}
```
