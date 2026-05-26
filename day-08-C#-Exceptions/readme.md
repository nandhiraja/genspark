# Exception Handling in C# — Notes

---

# What is an Exception?

An exception is:

```text id="1g20a8"
An unexpected error occurring during program execution
```

When exception happens:

* Program flow breaks
* Application may crash
* Execution stops if not handled

---

# Real Examples

| Situation          | Exception                |
| ------------------ | ------------------------ |
| Divide by zero     | DivideByZeroException    |
| Access null object | NullReferenceException   |
| File not found     | FileNotFoundException    |
| Wrong array index  | IndexOutOfRangeException |

---

# Why Exception Handling is Needed

Without exception handling:

```text id="c9qax0"
Application crashes
```

With exception handling:

```text id="bthq6u"
Application handles error safely
```

Benefits:

* Prevent crashes
* Show proper error message
* Continue execution safely
* Improve debugging
* Better user experience

---

# Exception Handling Flow

```text id="td3e5v"
Exception Occurs
       ↓
Try Block Detects
       ↓
Catch Block Handles
       ↓
Finally Block Executes
```

---

# Main Keywords in Exception Handling

| Keyword | Purpose                  |
| ------- | ------------------------ |
| try     | Risky code               |
| catch   | Handle exception         |
| finally | Always execute           |
| throw   | Manually throw exception |

---

# try Block

Used to place risky code.

---

## Example

```csharp id="1c22gf"
try
{
    int x = 10 / 0;
}
```

If error occurs inside `try`,
control moves to `catch`.

---

# catch Block

Used to handle exceptions.

---

## Example

```csharp id="2zfq1r"
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

`ex` contains error information.

---

# Complete Example

```csharp id="zh8x7j"
try
{
    int x = 10 / 0;
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

Output:

```text id="jlwmoc"
Attempted to divide by zero.
```

---

# finally Block

Code inside `finally` always executes.

Used for:

* Closing files
* Closing database connection
* Cleanup work

---

## Example

```csharp id="8rclhf"
finally
{
    Console.WriteLine("Completed");
}
```

Even if exception occurs,
`finally` executes.

---

# Full Structure

```csharp id="rwxxt6"
try
{
    // risky code
}
catch(Exception ex)
{
    // handle error
}
finally
{
    // cleanup
}
```

---

# Multiple catch Blocks

Different exceptions can be handled separately.

---

## Example

```csharp id="mjwgh7"
try
{
    int[] arr = {1,2};

    Console.WriteLine(arr[5]);
}
catch(DivideByZeroException ex)
{
    Console.WriteLine("Divide Error");
}
catch(IndexOutOfRangeException ex)
{
    Console.WriteLine("Index Error");
}
catch(Exception ex)
{
    Console.WriteLine("General Error");
}
```

---

# Important Rule for Multiple catch

```text id="gn5xfi"
Specific exception first
General exception last
```

Correct:

```text id="4v9zgo"
DivideByZeroException
       ↓
Exception
```

Wrong order causes unreachable code.

---

# Exception Hierarchy

All exceptions inherit from:

```text id="2k6zpq"
System.Exception
```

Flow:

```text id="do0wcl"
Object
  ↓
Exception
  ↓
Specific Exceptions
```

---

# Common Exceptions

| Exception                 | Reason             |
| ------------------------- | ------------------ |
| NullReferenceException    | Null object access |
| DivideByZeroException     | Divide by zero     |
| IndexOutOfRangeException  | Invalid index      |
| FormatException           | Wrong format       |
| InvalidOperationException | Invalid action     |

---

# throw Keyword

Used to manually create exceptions.

---

## Example

```csharp id="e6s9gk"
throw new Exception("Something went wrong");
```

---

# Why throw is Used

Used when:

* Business rule fails
* Validation fails
* Invalid data found
* Custom error handling needed

---

# Example of Validation

```csharp id="w3tdnp"
int age = -1;

if(age < 0)
{
    throw new Exception("Invalid Age");
}
```

---

# Rethrowing Exception

Used to pass same exception upward.

---

## Example

```csharp id="i42fko"
catch(Exception)
{
    throw;
}
```

Used when current layer cannot fully handle it.

---

# Custom Exceptions

Custom exception means:

```text id="6e7cl4"
Creating our own exception class
```

Used for business-specific errors.

---

# Why Custom Exceptions are Needed

Built-in exceptions are generic.

But applications may need meaningful errors like:

```text id="z3l9xt"
InvalidSalaryException
UserNotFoundException
PaymentFailedException
```

This improves:

* Readability
* Error understanding
* Business rule handling

---

# Creating Custom Exception

Custom exception is created using inheritance.

---

# Basic Structure

```csharp id="e7m4pq"
public class MyException : Exception
{
}
```

---

# Simple Custom Exception Example

```csharp id="s0q4sz"
public class InvalidAgeException : Exception
{
    public InvalidAgeException(string message)
        : base(message)
    {
    }
}
```

---

# Using Custom Exception

```csharp id="ljlwmv"
int age = -5;

if(age < 0)
{
    throw new InvalidAgeException(
        "Age cannot be negative"
    );
}
```

---

# Output

```text id="6dhh4g"
Age cannot be negative
```

---

# Important Rule for Custom Exceptions

Custom exception classes should:

* Inherit from `Exception`
* Be meaningful names
* End with `Exception`
* Pass message to base class

---

# Recommended Custom Exception Structure

```csharp id="xsr3sr"
public class CustomException : Exception
{
    public CustomException()
    {
    }

    public CustomException(string message)
        : base(message)
    {
    }

    public CustomException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}
```

---

# Why Multiple Constructors?

Provides flexibility.

Supports:

* Default error
* Custom message
* Nested exceptions

---

# Inner Exception

Used when one exception causes another exception.

---

## Example

```csharp id="6dr6oy"
catch(Exception ex)
{
    throw new Exception(
        "Database Failed",
        ex
    );
}
```

Here:

```text id="b5nqsv"
Original exception preserved
```

---

# Generic Rules for Creating Exceptions

---

# Naming Rules

Use meaningful names.

Correct:

```text id="2s0b8r"
InvalidOrderException
UserBlockedException
```

Avoid:

```text id="djlwmj"
MyError
ProblemException
```

---

# Inheritance Rule

Always inherit from:

```text id="j2n5j6"
Exception
```

or specific exception type.

---

# Message Rule

Always provide useful messages.

Good:

```text id="m1jlwm"
"Salary cannot be negative"
```

Bad:

```text id="q6g2z8"
"Error happened"
```

---

# Usage Rule

Use custom exceptions only for:

* Business rules
* Domain validations
* Meaningful application errors

Avoid creating too many unnecessary exceptions.

---

# Catching Exceptions

Always catch only what you can handle.

Avoid:

```csharp id="yjlwm6"
catch(Exception)
{
}
```

without handling.

---

# Logging Rule

Exceptions should usually be:

* Logged
* Tracked
* Reported properly

Especially in real applications.

---

# finally Block Usage

Use finally for cleanup operations.

Examples:

* Database closing
* File closing
* Stream cleanup

---

# Exception Propagation

If not handled:

```text id="u8jlwm"
Exception moves upward in call stack
```

Flow:

```text id="zjlwm8"
Method A
   ↓
Method B
   ↓
Method C
   ↓
Exception Thrown
```

---

# Stack Trace

Exception contains:

* Error message
* Method details
* Line information

Useful for debugging.

---

# Exception Object Important Properties

| Property       | Purpose            |
| -------------- | ------------------ |
| Message        | Error description  |
| StackTrace     | Error location     |
| InnerException | Original exception |
| Source         | Application/module |

---

# Simple Real Application Example

---

## Banking Validation

```csharp id="jlwm1z"
if(balance < withdrawAmount)
{
    throw new InsufficientBalanceException(
        "Insufficient balance"
    );
}
```

---

# Best Practices

---

# Use Specific Exceptions

Good:

```csharp id="xjlwm2"
catch(FileNotFoundException ex)
```

Better than:

```csharp id="rjlwm3"
catch(Exception ex)
```

---

# Do Not Hide Exceptions

Bad:

```csharp id="9jlwm4"
catch(Exception)
{
}
```

This hides real problems.

---

# Keep Messages Clear

Messages should help developers understand issue quickly.

---

# Use Custom Exceptions Carefully

Only create custom exceptions for meaningful business cases.

---

# Internal Understanding

Exception handling mainly helps:

```text id="mjlwm5"
Detect Errors
      +
Handle Errors Safely
      +
Prevent Application Crash
```

---

# Small Complete Example

```csharp id="2jlwm6"
using System;

public class InvalidAgeException : Exception
{
    public InvalidAgeException(string message)
        : base(message)
    {
    }
}

class Program
{
    static void Main()
    {
        try
        {
            int age = -10;

            if(age < 0)
            {
                throw new InvalidAgeException(
                    "Age cannot be negative"
                );
            }
        }
        catch(InvalidAgeException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.WriteLine("Completed");
        }
    }
}
```

---

# Quick Recall

```text id="8jlwm7"
Exception
 → Runtime error

Main Keywords
 → try
 → catch
 → finally
 → throw

Custom Exception
 → Create own exception class

Rules
 → Inherit Exception
 → Meaningful name
 → Useful message

finally
 → Cleanup code

throw
 → Manually raise exception
```
