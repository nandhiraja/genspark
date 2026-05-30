# Unit Testing in C# Backend —  Notes

---

# What is Unit Testing?

Unit testing means:

```text id="ut1"
Testing small individual parts of application
```

usually:

* Methods
* Services
* Business logic
* Utility functions

---

# Main Goal of Unit Testing

Check whether:

```text id="ut2"
Code behaves correctly
```

under expected conditions.

---

# What is a "Unit"?

A unit is usually:

```text id="ut3"
Single method or small logic block
```

Example:

```csharp id="ut4"
CalculateTotal()
CreateOrder()
ValidateUser()
```

---

# Why Unit Testing is Needed

Real applications continuously change.

Without tests:

```text id="ut5"
New changes may break old features
```

Unit testing helps detect problems early.

---

# Benefits of Unit Testing

| Benefit              | Meaning                 |
| -------------------- | ----------------------- |
| Early bug detection  | Catch issues quickly    |
| Safer refactoring    | Modify code confidently |
| Better code quality  | Cleaner design          |
| Easier debugging     | Isolate problems        |
| Documentation effect | Shows expected behavior |

---

# Real Company Usage

Unit tests commonly used for:

* Service layer
* Validation logic
* Business rules
* Calculations
* Helper methods

---

# What Usually NOT Tested in Unit Tests

Avoid directly testing:

* Database
* External APIs
* Real file system
* Network calls

because unit tests should stay:

```text id="ut6"
Fast and isolated
```

---

# Important Idea

Unit test should test:

```text id="ut7"
Behavior
```

not internal implementation.

---

# Real Example

Good test:

```text id="ut8"
Does CreateOrder return success?
```

Bad test:

```text id="ut9"
Did method use foreach internally?
```

---

# Common Testing Types

| Type             | Purpose                   |
| ---------------- | ------------------------- |
| Unit Test        | Small isolated logic      |
| Integration Test | Multiple systems together |
| End-to-End Test  | Full application flow     |

---

# Unit Test vs Integration Test

| Unit Test   | Integration Test  |
| ----------- | ----------------- |
| Fast        | Slower            |
| Isolated    | Real dependencies |
| No DB/API   | Uses DB/API       |
| Small logic | Full flow         |

---

# Most Common Unit Testing Frameworks in C#

| Framework | Usage               |
| --------- | ------------------- |
| xUnit     | Most popular        |
| NUnit     | Common              |
| MSTest    | Microsoft framework |

---

# Most Used in Modern ASP.NET

```text id="ut10"
xUnit
```

---

# Common Packages Used

| Package          | Purpose              |
| ---------------- | -------------------- |
| xUnit            | Testing framework    |
| Moq              | Mocking dependencies |
| FluentAssertions | Better assertions    |

---

# What is Assertion?

Assertion means:

```text id="ut11"
Checking expected result
```

---

# Example

```csharp id="ut12"
Assert.Equal(10, result);
```

Meaning:

```text id="ut13"
Expected result should be 10
```

---

# Basic Unit Test Structure

```text id="ut14"
Arrange
Act
Assert
```

Very important pattern.

---

# 1. Arrange

Prepare test data.

---

# 2. Act

Call method being tested.

---

# 3. Assert

Verify expected result.

---

# Example

---

# Service Method

```csharp id="ut15"
public class CalculatorService
{
    public int Add(int a, int b)
    {
        return a + b;
    }
}
```

---

# Unit Test

```csharp id="ut16"
public class CalculatorServiceTests
{
    [Fact]
    public void Add_ShouldReturnCorrectSum()
    {
        // Arrange
        var service = new CalculatorService();

        // Act
        var result = service.Add(5, 3);

        // Assert
        Assert.Equal(8, result);
    }
}
```

---

# Understanding Test Naming

Good naming:

```text id="ut17"
MethodName_Condition_ExpectedResult
```

Example:

```text id="ut18"
Add_ValidNumbers_ReturnsSum
```

---

# Why Naming Important

Helps understand:

* What tested
* Under what condition
* Expected behavior

---

# What Should Be Tested?

---

# Business Rules

Example:

```text id="ut19"
Cannot withdraw more than balance
```

---

# Validation Logic

Example:

```text id="ut20"
Email should not be empty
```

---

# Calculations

Example:

```text id="ut21"
Tax calculation
Discount logic
```

---

# Conditional Flows

Example:

```text id="ut22"
Admin user allowed
Normal user denied
```

---

# Edge Cases

Very important.

Example:

* Null values
* Empty lists
* Negative numbers
* Large values

---

# Exception Handling

Check expected exceptions.

---

# Example

```csharp id="ut23"
Assert.Throws<Exception>(() =>
{
    service.Withdraw(-100);
});
```

---

# Why Edge Cases Important

Real bugs often happen in:

```text id="ut24"
Unexpected situations
```

---

# Mocking in Unit Testing

Very important in backend testing.

---

# What is Mocking?

Mocking means:

```text id="ut25"
Creating fake dependency objects
```

---

# Why Mocking Needed

Real services may depend on:

* Database
* API calls
* Email services

Unit tests should avoid real external systems.

---

# Example Dependency

```csharp id="ut26"
public class UserService
{
    private readonly IUserRepository _repo;
}
```

Repository should be mocked.

---

# What is Moq?

`Moq` is popular mocking library in C#.

Used to create fake objects.

---

# Example

```csharp id="ut27"
var mockRepo =
    new Mock<IUserRepository>();
```

---

# Mock Behavior Setup

```csharp id="ut28"
mockRepo.Setup(x =>
    x.GetUserById(1))
.Returns(new User
{
    Id = 1,
    Name = "John"
});
```

---

# Why Mocking Important

Keeps tests:

* Fast
* Predictable
* Isolated

---

# Example Real Service Test

---

# Service

```csharp id="ut29"
public class UserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public User GetUser(int id)
    {
        return _repo.GetUserById(id);
    }
}
```

---

# Unit Test

```csharp id="ut30"
public class UserServiceTests
{
    [Fact]
    public void GetUser_ReturnsUser()
    {
        // Arrange
        var mockRepo =
            new Mock<IUserRepository>();

        mockRepo.Setup(x =>
            x.GetUserById(1))
        .Returns(new User
        {
            Id = 1,
            Name = "John"
        });

        var service =
            new UserService(mockRepo.Object);

        // Act
        var result = service.GetUser(1);

        // Assert
        Assert.Equal("John", result.Name);
    }
}
```

---

# Why Interfaces Important in Testing

Interfaces allow:

```text id="ut31"
Dependency replacement
```

Very important for mocking.

---

# Unit Testing in Layered Architecture

Usually tests focus on:

```text id="ut32"
Service Layer
```

because business logic mostly exists there.

---

# Example Architecture

```text id="ut33"
Controller
   ↓
Service  ← Mostly unit tested
   ↓
Repository
   ↓
Database
```

---

# Controllers Usually Tested Less

Because controllers mostly:

* Receive request
* Call service
* Return response

Main logic belongs in service layer.

---

# Good Unit Test Characteristics

---

# Fast

Should execute quickly.

---

# Independent

Should not depend on other tests.

---

# Repeatable

Should always produce same result.

---

# Isolated

Should not use real DB/API.

---

# Clear

Easy to understand.

---

# Common Things to Test

| Test Type    | Example        |
| ------------ | -------------- |
| Success case | Valid login    |
| Failure case | Wrong password |
| Validation   | Empty email    |
| Edge case    | Null input     |
| Exception    | Invalid amount |

---

# What NOT to Over-Test

Avoid testing:

* EF Core internal behavior
* Framework functionality
* Simple getters/setters

Focus on:

```text id="ut34"
Business behavior
```

---

# Common Mistakes

---

# Testing Database in Unit Test

Makes tests slow and unstable.

---

# Huge Complex Tests

One test should focus on one behavior.

---

# Too Many Assertions

Hard to understand failures.

---

# Testing Internal Implementation

Tests become fragile.

---

# Not Using Mocking

Creates dependency problems.

---

# Best Practices

---

# Use Arrange-Act-Assert

Keeps tests clean.

---

# Use Meaningful Names

Improves readability.

---

# Mock External Dependencies

Keep tests isolated.

---

# Test Business Rules Carefully

Most important area.

---

# Cover Failure Cases Too

Not only success scenarios.

---

# Keep Tests Simple

Easy maintenance matters.

---

# Real Backend Areas Commonly Tested

| Area              | Test Example      |
| ----------------- | ----------------- |
| Authentication    | Invalid login     |
| Payment logic     | Balance check     |
| Order service     | Stock validation  |
| User registration | Duplicate email   |
| Discount logic    | Coupon validation |

---

# Unit Testing Purpose in Real Companies

Mainly helps:

```text id="ut35"
Prevent breaking existing functionality
```

during:

* Feature additions
* Refactoring
* Team development

---

# Final 

Unit testing mainly means:

```text id="ut36"
Testing small pieces of logic independently
```

to ensure application behaves correctly.

Most important focus:

```text id="ut37"
Business logic correctness
```

---

#  Recall

```text id="ut38"
Unit Test
 → Test small isolated logic

Main Pattern
 → Arrange
 → Act
 → Assert

Common Tools
 → xUnit
 → Moq
 → FluentAssertions

What to Test
 → Business rules
 → Validation
 → Edge cases
 → Exceptions

Avoid
 → Real DB
 → Real APIs
 → External dependencies

Goal
 → Reliable maintainable backend
```
