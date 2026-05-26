# OOPs in C# —  Notes

---

# What is OOP?

OOP (Object-Oriented Programming) is a way of designing software by organizing code around:

```text id="q5m4fx"
Objects
Data
Behaviors
Responsibilities
```

Instead of writing everything in one place,
OOP helps split application into manageable components.

---

# Why Companies Use OOP

In real applications:

* Multiple developers work together
* Features continuously grow
* Code changes frequently
* Systems become large

Without OOP:

```text id="w4r3pf"
Code becomes difficult to maintain
```

OOP helps build:

* Reusable systems
* Maintainable applications
* Scalable architecture
* Flexible business logic

---

# Main Pillars of OOP

```text id="6uoqxy"
1. Encapsulation
2. Abstraction
3. Inheritance
4. Polymorphism
```

These pillars solve different software problems.

---

# 1. Encapsulation

---

# What is Encapsulation?

Encapsulation means:

```text id="i6xjlwm"
Protecting and controlling object data
```

Object should not expose everything directly.

Instead:

```text id="jlwms1"
Data access controlled through methods/properties
```

---

# Real Company Scenario

## Banking System

Imagine bank account object.

Bad approach:

```csharp id="jlwmr2"
account.Balance = -100000;
```

Any developer can directly modify balance.

This creates:

* Invalid data
* Security issues
* Business rule violations

---

# Encapsulation Solution

```csharp id="jlwmr3"
public class BankAccount
{
    private decimal _balance;

    public void Deposit(decimal amount)
    {
        if(amount > 0)
        {
            _balance += amount;
        }
    }

    public decimal GetBalance()
    {
        return _balance;
    }
}
```

---

# Why Encapsulation Used Here

Because:

```text id="jlwmr4"
Balance should not be modified freely
```

Only valid operations allowed.

---

# Outcome of Encapsulation

| Problem              | Solution          |
| -------------------- | ----------------- |
| Invalid data updates | Controlled access |
| Data corruption      | Validation        |
| Direct manipulation  | Protected state   |

---

# Real Applications Using Encapsulation

* Payment systems
* Banking applications
* Inventory systems
* Employee salary systems
* Authentication modules

---

# Common Encapsulation Tools

| Feature    | Usage             |
| ---------- | ----------------- |
| private    | Hide data         |
| properties | Controlled access |
| methods    | Validation logic  |

---

# 2. Abstraction

---

# What is Abstraction?

Abstraction means:

```text id="jlwmr5"
Showing only necessary details
Hiding internal complexity
```

User uses feature without knowing internal implementation.

---

# Real Company Scenario

## Payment Gateway

User clicks:

```text id="jlwmr6"
Pay Now
```

Internally system may:

* Validate card
* Connect bank API
* Verify OTP
* Process transaction
* Save logs
* Send notifications

But frontend developer simply uses:

```csharp id="jlwmr7"
paymentService.Pay();
```

Complexity hidden.

---

# Abstraction Example

```csharp id="jlwmr8"
public interface IPaymentService
{
    void Pay(decimal amount);
}
```

Implementation:

```csharp id="jlwmr9"
public class RazorpayService : IPaymentService
{
    public void Pay(decimal amount)
    {
        // complex payment logic
    }
}
```

---

# Why Abstraction Used Here

Because:

```text id="jlwms0"
Consumers should focus on usage
not internal processing
```

---

# Outcome of Abstraction

| Problem                | Solution                   |
| ---------------------- | -------------------------- |
| Complex logic exposure | Hide implementation        |
| Tight dependency       | Interface-based usage      |
| Hard maintenance       | Separated responsibilities |

---

# Real Applications Using Abstraction

* Payment gateways
* Email services
* Cloud storage systems
* Authentication providers
* External API integrations

---

# Common Abstraction Tools

| Feature        | Usage               |
| -------------- | ------------------- |
| interface      | Contract definition |
| abstract class | Partial abstraction |
| service layer  | Hide business logic |

---

# 3. Inheritance

---

# What is Inheritance?

Inheritance means:

```text id="jlwms2"
Reusing existing functionality
through parent-child relationship
```

---

# Real Company Scenario

## Employee Management System

All employees may have:

* Id
* Name
* Email
* Login
* Attendance

Instead of repeating in every class:

```text id="jlwms3"
Reuse common logic
```

---

# Example

Base class:

```csharp id="jlwms4"
public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; }

    public void MarkAttendance()
    {
    }
}
```

Derived class:

```csharp id="jlwms5"
public class Developer : Employee
{
    public string ProgrammingLanguage { get; set; }
}
```

Another derived class:

```csharp id="jlwms6"
public class HR : Employee
{
    public int RecruitmentCount { get; set; }
}
```

---

# Why Inheritance Used Here

Because:

```text id="jlwms7"
Common functionality reused
instead of duplicated
```

---

# Outcome of Inheritance

| Problem               | Solution             |
| --------------------- | -------------------- |
| Duplicate code        | Reuse common logic   |
| Difficult maintenance | Centralized changes  |
| Repeated structures   | Shared base behavior |

---

# Real Applications Using Inheritance

* User role systems
* Notification systems
* Base API controllers
* Shared domain models
* Logging frameworks

---

# Important Note in Real Projects

Inheritance should be used only when:

```text id="jlwms8"
True IS-A relationship exists
```

Example:

```text id="jlwms9"
Developer IS-A Employee
```

Bad example:

```text id="jlwmt0"
Database IS-A Logger
```

No relationship.

---

# 4. Polymorphism

---

# What is Polymorphism?

Polymorphism means:

```text id="jlwmt1"
One interface
Different implementations
```

Same method behaves differently based on object.

---

# Real Company Scenario

## Notification System

Application may send notifications through:

* Email
* SMS
* Push notification
* WhatsApp

But application wants:

```text id="jlwmt2"
Common notification handling
```

---

# Example

Interface:

```csharp id="jlwmt3"
public interface INotificationService
{
    void Send(string message);
}
```

Email implementation:

```csharp id="jlwmt4"
public class EmailService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine("Email Sent");
    }
}
```

SMS implementation:

```csharp id="jlwmt5"
public class SmsService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine("SMS Sent");
    }
}
```

Usage:

```csharp id="jlwmt6"
INotificationService service =
    new EmailService();

service.Send("Hello");
```

---

# Why Polymorphism Used Here

Because system wants:

```text id="jlwmt7"
Flexible interchangeable implementations
```

---

# Outcome of Polymorphism

| Problem                   | Solution               |
| ------------------------- | ---------------------- |
| Hardcoded implementations | Flexible behavior      |
| Difficult extension       | Easy replacement       |
| Tight coupling            | Loosely coupled system |

---

# Real Applications Using Polymorphism

* Payment providers
* Logging systems
* Notification systems
* Authentication providers
* Cloud services

---

# Types of Polymorphism

---

# Compile-Time Polymorphism

Method overloading.

```csharp id="jlwmt8"
Calculate(int a, int b)

Calculate(decimal a, decimal b)
```

Same method name,
different parameters.

---

# Runtime Polymorphism

Method overriding.

```csharp id="jlwmt9"
virtual
override
```

Behavior decided at runtime.

---

# OOP in Real Enterprise Architecture

---

# Typical Layered Structure

```text id="jlwmu0"
Controller
    ↓
Service
    ↓
Repository
    ↓
Database
```

OOP helps separate responsibilities.

---

# Example Responsibility Split

| Layer      | Responsibility      |
| ---------- | ------------------- |
| Controller | Request handling    |
| Service    | Business logic      |
| Repository | Database access     |
| Model      | Data representation |

---

# Why OOP Matters in Teams

Without OOP:

```text id="jlwmu1"
Everything tightly connected
```

With OOP:

```text id="jlwmu2"
Independent modular components
```

Benefits:

* Easier testing
* Easier maintenance
* Better scalability
* Parallel development possible

---

# Real System Example

## E-Commerce Application

OOP used in:

| Module             | OOP Usage                  |
| ------------------ | -------------------------- |
| Payment            | Abstraction + Polymorphism |
| Product Management | Encapsulation              |
| User Roles         | Inheritance                |
| Notification       | Polymorphism               |
| Security           | Encapsulation              |

---

# Relationship Between OOP Concepts

---

# Encapsulation

Protects data.

---

# Abstraction

Hides complexity.

---

# Inheritance

Reuses functionality.

---

# Polymorphism

Provides flexibility.

---

# Combined Together

They help build:

```text id="jlwmu3"
Clean
Maintainable
Scalable
Reusable
Enterprise-level applications
```

---

# Common Mistakes in Real Projects

---

# Overusing Inheritance

Sometimes composition is better.

Bad deep inheritance chains create complexity.

---

# Exposing Everything Public

Breaks encapsulation.

---

# Huge Classes

One class handling everything.

Violates responsibility separation.

---

# Tight Coupling

Directly depending on concrete classes.

Prefer interfaces.

---

# Good OOP Design Usually Focuses On

```text id="jlwmu4"
Low Coupling
High Cohesion
Clear Responsibilities
Flexible Design
```

---

# Small Real Example Combining Pillars

```csharp id="jlwmu5"
public interface IPaymentService
{
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentService
{
    public void Pay(decimal amount)
    {
        Console.WriteLine("Card Payment");
    }
}

public class PaymentProcessor
{
    private readonly IPaymentService _paymentService;

    public PaymentProcessor(
        IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public void Process(decimal amount)
    {
        _paymentService.Pay(amount);
    }
}
```

---

# OOP Concepts Used Here

| Concept              | Usage                      |
| -------------------- | -------------------------- |
| Abstraction          | IPaymentService            |
| Polymorphism         | Multiple payment types     |
| Encapsulation        | PaymentProcessor internals |
| Dependency Injection | Loose coupling             |

---


