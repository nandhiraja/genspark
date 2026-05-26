# Delegates in C# —  Notes

---

# What is a Delegate?

A delegate is a special type in C# used to store and execute methods.

It works like:

```text
Variable for Methods
```

Normally:

```csharp id="8ffnws"
int x = 10;
```

stores values.

But delegate stores:

```csharp id="hmd2n9"
SomeMethod
```

reference.

---

# Why Delegates are Used

Delegates are mainly used when we want:

* Pass methods as parameters
* Execute methods dynamically
* Call methods later
* Build flexible systems
* Support callbacks and events

---

# Real Understanding

Imagine:

```text
Button Clicked
   ↓
Call This Method
```

Instead of hardcoding method names,
delegate allows methods to be assigned dynamically.

---

# Delegate Structure

Basic syntax:

```csharp id="3xz7g6"
delegate returnType DelegateName(parameters);
```

Example:

```csharp id="50f3vz"
public delegate void PrintMessage();
```

Here:

| Part         | Meaning       |
| ------------ | ------------- |
| delegate     | Keyword       |
| void         | Return type   |
| PrintMessage | Delegate name |
| ()           | Parameters    |

---

# Creating a Delegate

Delegate creation usually has 4 parts.

---

## 1. Create Delegate Type

```csharp id="ll7d1m"
public delegate void MessageDelegate();
```

This defines:

```text
A delegate that can store methods
having:
→ no parameter
→ no return value
```

---

## 2. Create Matching Method

```csharp id="0c4f3t"
public static void Show()
{
    Console.WriteLine("Hello");
}
```

---

## 3. Store Method in Delegate

```csharp id="vqgdrm"
MessageDelegate del = Show;
```

Now delegate contains method reference.

---

## 4. Invoke Delegate

```csharp id="mjlwm5"
del();
```

Output:

```text id="o6wlom"
Hello
```

---

# Important Rule While Creating Delegates

The method signature MUST match the delegate signature.

---

## Signature Means

* Return type
* Parameter count
* Parameter types

---

## Correct Matching

Delegate:

```csharp id="q2m14m"
public delegate int MathOperation(int a, int b);
```

Matching method:

```csharp id="f0g47r"
public static int Add(int x, int y)
{
    return x + y;
}
```

Works correctly.

---

## Wrong Matching

```csharp id="z3e33y"
public static void Add(int x, int y)
{
}
```

Problem:

```text
Return type not matching
```

---

# Delegate With Parameters

Delegates can also receive values.

---

## Example

Delegate:

```csharp id="urjpr7"
public delegate int Operation(int a, int b);
```

Method:

```csharp id="y0j7sr"
public static int Multiply(int a, int b)
{
    return a * b;
}
```

Usage:

```csharp id="mfxczd"
Operation op = Multiply;

int result = op(2, 3);
```

---

# Delegates with Instance Methods

Delegate can store:

* Static methods
* Object methods

---

## Example

```csharp id="gvlc8w"
public class Test
{
    public void Show()
    {
        Console.WriteLine("Hello");
    }
}
```

Usage:

```csharp id="jz7qun"
Test t = new Test();

MessageDelegate del = t.Show;
```

---

# Multicast Delegates

A delegate can store multiple methods together.

---

## Example

```csharp id="pr3qk5"
public delegate void Notify();
```

Methods:

```csharp id="q0m4iw"
public static void Email()
{
    Console.WriteLine("Email Sent");
}

public static void SMS()
{
    Console.WriteLine("SMS Sent");
}
```

Adding methods:

```csharp id="d6yw5u"
Notify notify = Email;

notify += SMS;
```

Calling:

```csharp id="msj1t6"
notify();
```

Output:

```text id="jlwm3j"
Email Sent
SMS Sent
```

---

# Removing Methods from Delegate

```csharp id="1ml6vf"
notify -= SMS;
```

---

# Delegate Invocation

Delegates can be executed in two ways.

---

## Direct Invocation

```csharp id="kn0g1y"
del();
```

---

## Using Invoke()

```csharp id="s7evl6"
del.Invoke();
```

---

# Null Safe Delegate Calling

Sometimes delegate may be null.

Safe calling:

```csharp id="sr9p3m"
del?.Invoke();
```

---

# Anonymous Methods

Method without name.

---

## Example

```csharp id="rmk8z5"
Operation op = delegate(int a, int b)
{
    return a + b;
};
```

Useful for short temporary logic.

---

# Lambda Expressions

Short version of anonymous methods.

---

## Example

```csharp id="3q4bdw"
Operation op = (a, b) => a + b;
```

Modern C# mostly uses lambda expressions.

---

# Delegates and Lambda Relation

Lambda expressions internally work using delegates.

Example:

```csharp id="igui3d"
numbers.Where(x => x > 5);
```

Here:

```text
x => x > 5
```

is converted into delegate internally.

---

# Built-in Delegates

C# already provides ready-made delegates.

---

# Action Delegate

Used when:

```text
No return value needed
```

---

## Example

```csharp id="z8d8k0"
Action<string> print = msg =>
{
    Console.WriteLine(msg);
};
```

---

# Func Delegate

Used when:

```text
Method returns value
```

Last type represents return type.

---

## Example

```csharp id="5b3b3h"
Func<int, int, int> add =
    (a, b) => a + b;
```

Meaning:

```text
Input  → int, int
Return → int
```

---

# Predicate Delegate

Special delegate that always returns boolean.

---

## Example

```csharp id="r5h71n"
Predicate<int> isEven =
    x => x % 2 == 0;
```

---

# Delegates in Real Applications

Delegates are heavily used in:

* LINQ
* Event handling
* Callbacks
* Async programming
* Sorting/filtering logic
* Middleware pipelines

---

# Delegates and Events

Events internally use delegates.

Flow:

```text
User Action
    ↓
Event Triggered
    ↓
Delegate Calls Methods
```

---

# Important Concepts Behind Delegates

Delegates are connected with:

| Concept   | Usage                |
| --------- | -------------------- |
| Methods   | Store references     |
| Lambda    | Short delegate logic |
| Events    | Event handling       |
| Callbacks | Dynamic execution    |
| LINQ      | Passing logic        |

---

# Internal Working Idea

Internally delegate mainly stores:

```text
Method Address
+
Method Information
```

Then executes that method later.

---

# Simple Mental Model

Think delegate as:

```text
Remote Control for Methods
```

You can:

* Store method
* Pass method
* Execute method later
* Replace method dynamically

---

# Small Complete Example

```csharp id="d7j4c5"
using System;

public delegate int Operation(int a, int b);

class Program
{
    static int Add(int x, int y)
    {
        return x + y;
    }

    static void Main()
    {
        Operation op = Add;

        int result = op(10, 20);

        Console.WriteLine(result);
    }
}
```

---

# Quick Recall

```text
Delegate
 → Stores method references

Used For
 → Dynamic method execution

Rules
 → Signature must match

Supports
 → Static methods
 → Instance methods
 → Multiple methods

Related To
 → Lambda
 → Events
 → LINQ
 → Callbacks
```
