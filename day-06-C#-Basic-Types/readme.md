# C# Basics Notes

## What is C#?

C# is a modern, object-oriented programming language developed by Microsoft.

It is mainly used to build:

* Desktop Applications
* Web Applications
* APIs & Backend Systems
* Games
* Mobile Applications
* Cloud Applications

C# runs mainly on the .NET platform.

---

# Why C# Was Created

Before C#, developers used languages like:

* C
* C++
* Java

These languages had some problems:

| Problem                      | Result                 |
| ---------------------------- | ---------------------- |
| Complex memory management    | Hard to develop safely |
| Platform dependency          | Difficult deployment   |
| Large codebases became messy | Maintenance problems   |
| Security issues              | Runtime crashes        |

C# was designed to solve these problems by providing:

* Simpler syntax
* Automatic memory management
* Strong type safety
* Better developer productivity
* Easy application development

---

# Mental Model of C#

Think of C# like this:

```text
C# = Human-Friendly Language
.NET = Engine that runs the program
CLR = Runtime environment
```

Flow:

```text
C# Code
   ↓
Compiler
   ↓
Intermediate Language (IL)
   ↓
CLR (.NET Runtime)
   ↓
Machine Code
   ↓
Program Executes
```

---

# Important Components in C#

## 1. Compiler

The C# compiler converts human-readable C# code into Intermediate Language (IL).

Compiler used:

```text
csc.exe
```

### Example

```csharp
Console.WriteLine("Hello");
```

Compiler converts this into IL code.

---

# 2. CLR (Common Language Runtime)

CLR is the runtime engine of .NET.

It handles:

* Memory Management
* Garbage Collection
* Security
* Exception Handling
* Thread Management

Think of CLR like:

```text
Operating System for .NET Applications
```

---

# 3. Garbage Collection

In languages like C/C++:

```text
Developer manually frees memory
```

In C#:

```text
Garbage Collector automatically cleans unused memory
```

This reduces:

* Memory leaks
* Crashes
* Pointer issues

---

# 4. .NET

.NET provides:

* Libraries
* Runtime
* Tools
* Frameworks

Used for building:

| Area     | Usage             |
| -------- | ----------------- |
| ASP.NET  | Web apps          |
| WinForms | Desktop apps      |
| WPF      | Modern desktop UI |
| MAUI     | Mobile/Desktop    |
| Unity    | Game development  |

---

# Features of C#

## Object-Oriented

Supports:

* Class
* Object
* Inheritance
* Polymorphism
* Encapsulation
* Abstraction

---

## Type Safe

C# checks data types before execution.

Example:

```csharp
int x = "Hello"; // Error
```

This prevents many runtime bugs.

---

## Cross Platform

Modern .NET allows C# applications to run on:

* Windows
* Linux
* macOS

---

## Rich Library Support

C# has built-in libraries for:

* File handling
* Networking
* Database access
* JSON handling
* Web APIs
* Multithreading

---

# Structure of a Basic C# Program

```csharp
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello World");
    }
}
```

---

# Line-by-Line Explanation

## using System;

Imports built-in functionalities.

---

## class Program

Defines a class.

Everything in C# lives inside classes.

---

## static void Main()

Main method = Entry point of program.

Program execution starts here.

---

## Console.WriteLine()

Prints output to console.

---

# Compilation Process in C#

## Step 1 — Write Source Code

```text
Program.cs
```

---

## Step 2 — Compile

Compiler converts:

```text
C# → IL Code
```

Produces:

```text
.exe or .dll
```

---

## Step 3 — CLR Executes

CLR converts IL into machine code using:

```text
JIT Compiler (Just-In-Time Compiler)
```

---

# Important Areas Where C# is Used

| Area                | Example       |
| ------------------- | ------------- |
| Backend Development | ASP.NET Core  |
| Game Development    | Unity         |
| Desktop Apps        | WPF, WinForms |
| Mobile Apps         | MAUI          |
| Cloud Apps          | Azure         |
| Enterprise Software | ERP, Banking  |

---

# Data Types in C#

## What is a Data Type?

Data type defines:

```text
What kind of data a variable can store
```

Example:

```csharp
int age = 20;
```

`int` tells C#:

```text
Store integer values
```

---

# Types of Data Types

## 1. Value Types

Stores actual value directly.

Examples:

```csharp
int
float
double
char
bool
```

---

## 2. Reference Types

Stores memory address/reference.

Examples:

```csharp
string
array
class
object
```

---

# Common Data Types

| Type   | Example | Size     |
| ------ | ------- | -------- |
| int    | 10      | 4 bytes  |
| long   | 100000L | 8 bytes  |
| float  | 10.5f   | 4 bytes  |
| double | 10.55   | 8 bytes  |
| char   | 'A'     | 2 bytes  |
| bool   | true    | 1 byte   |
| string | "Hello" | Variable |

---

# Variable Declaration

```csharp
int age = 25;
string name = "John";
bool isActive = true;
```

---

# Type Conversion in C#

## Why Type Conversion Exists

Sometimes one type must be converted into another.

Example:

```text
User input comes as string
But calculation needs int
```

---

# Types of Conversion

## 1. Implicit Conversion

Automatic conversion.

Small → Large

Example:

```csharp
int x = 10;
double y = x;
```

Why safe?

```text
int fits inside double
```

---

## 2. Explicit Conversion (Casting)

Manual conversion.

Large → Small

Example:

```csharp
double x = 10.5;
int y = (int)x;
```

Output:

```text
10
```

Decimal part removed.

---

# Type Conversion Methods

## Convert Class

```csharp
string num = "100";

int x = Convert.ToInt32(num);
```

---

## Parse Method

```csharp
string num = "50";

int x = int.Parse(num);
```

---

## TryParse Method

Safest method.

```csharp
string value = "abc";

bool result = int.TryParse(value, out int number);
```

If conversion fails:

```text
No crash occurs
```

Very important in real applications.

---

# Value Type vs Reference Type

## Value Type

```csharp
int a = 10;
int b = a;

b = 20;
```

`a` remains:

```text
10
```

Because copy is created.

---

## Reference Type

```csharp
string s1 = "Hello";
string s2 = s1;
```

Reference behavior differs internally.

---

# Important Keywords in C#

| Keyword | Purpose          |
| ------- | ---------------- |
| class   | Create class     |
| static  | Shared member    |
| void    | No return        |
| new     | Create object    |
| public  | Access modifier  |
| private | Restrict access  |
| using   | Import namespace |

---

# Common Beginner Mistakes

## Forgetting Semicolon

```csharp
int x = 10
```

Error.

---

## Wrong Type Assignment

```csharp
int x = "Hello";
```

---

## Confusing float and double

```csharp
float x = 10.5; // Error
```

Correct:

```csharp
float x = 10.5f;
```

---

# Real Production Perspective

Small programs look simple.

But large enterprise C# applications involve:

* Multiple projects
* Dependency Injection
* APIs
* Databases
* Async Programming
* Logging
* Security
* Caching
* Distributed Systems

This is why C# is heavily used in:

* Banking
* Enterprise Software
* Cloud Systems
* Backend APIs

---
