# ASP.NET Web API —  Notes

---

# What is Web API?

Web API is a way for applications to:

```text id="api1"
Communicate over HTTP
```

using:

* Requests
* Responses
* URLs
* JSON data

---

# Simple Understanding

Web API allows:

```text id="api2"
Frontend ↔ Backend communication
```

---

# Real Example

When frontend clicks:

```text id="api3"
Login
```

request goes to backend API.

Backend:

* Validates user
* Checks database
* Returns response

---

# Real Flow

```text id="api4"
Frontend / Mobile App
          ↓
       Web API
          ↓
Business Logic
          ↓
Database
```

---

# What is ASP.NET Web API?

ASP.NET Web API is:

```text id="api5"
Microsoft framework for building APIs in C#
```

Used to create:

* REST APIs
* Backend services
* Microservices
* Mobile backends

---

# Why Web API is Needed

Modern applications have:

* Web frontend
* Mobile apps
* Third-party integrations
* Multiple clients

All need common backend communication.

Web API provides:

```text id="api6"
Central backend service
```

---

# Example Applications Using APIs

| Application  | API Usage        |
| ------------ | ---------------- |
| Swiggy       | Order APIs       |
| Banking apps | Transaction APIs |
| Instagram    | Feed APIs        |
| E-commerce   | Product APIs     |
| Uber         | Ride APIs        |

---

# What API Actually Does

API mainly:

* Receives request
* Processes request
* Returns response

---

# Example

Request:

```http id="api7"
GET /users/1
```

Response:

```json id="api8"
{
  "id": 1,
  "name": "John"
}
```

---

# HTTP Basics in Web API

Web APIs mainly work using:

```text id="api9"
HTTP Protocol
```

---

# Important HTTP Methods

| Method | Purpose             |
| ------ | ------------------- |
| GET    | Read data           |
| POST   | Create data         |
| PUT    | Update full data    |
| PATCH  | Update partial data |
| DELETE | Remove data         |

---

# Simple Real Examples

---

# GET

```http id="api10"
GET /products
```

Get products.

---

# POST

```http id="api11"
POST /users
```

Create new user.

---

# PUT

```http id="api12"
PUT /users/1
```

Update user.

---

# DELETE

```http id="api13"
DELETE /users/1
```

Delete user.

---

# Main Components in Web API

---

# 1. Controller

Controller handles HTTP requests.

Example:

```csharp id="api14"
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
}
```

---

# Responsibility

```text id="api15"
Receive request
Return response
```

---

# 2. Action Methods

Methods inside controller.

Example:

```csharp id="api16"
[HttpGet]
public IActionResult GetUsers()
{
}
```

---

# 3. Routing

Routing decides:

```text id="api17"
Which URL maps to which method
```

---

# Example

```http id="api18"
api/users
```

goes to:

```csharp id="api19"
GetUsers()
```

---

# 4. Request

Data sent by client.

Can include:

* URL
* Headers
* Body
* Query params

---

# 5. Response

Data returned from API.

Usually:

```text id="api20"
JSON
```

---

# JSON

Most common API data format.

Example:

```json id="api21"
{
  "id": 1,
  "name": "John"
}
```

---

# Why JSON Used

Because:

* Lightweight
* Easy to parse
* Language independent

---

# Basic Web API Flow

```text id="api22"
Client Request
      ↓
Controller
      ↓
Service/Logic
      ↓
Response Returned
```

---

# Example Simple API

---

# Controller Example

```csharp id="api23"
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = new List<string>
        {
            "John",
            "Sam"
        };

        return Ok(users);
    }
}
```

---

# What Happens Here

Request:

```http id="api24"
GET /api/users
```

Response:

```json id="api25"
[
  "John",
  "Sam"
]
```

---

# IActionResult

Used for returning HTTP responses.

Examples:

| Method         | Meaning         |
| -------------- | --------------- |
| Ok()           | Success         |
| BadRequest()   | Invalid request |
| NotFound()     | Data missing    |
| Unauthorized() | Auth failed     |

---

# Status Codes

HTTP responses contain status codes.

---

# Common Status Codes

| Code | Meaning      |
| ---- | ------------ |
| 200  | Success      |
| 201  | Created      |
| 400  | Bad request  |
| 401  | Unauthorized |
| 404  | Not found    |
| 500  | Server error |

---

# Example

```csharp id="api26"
return NotFound();
```

returns:

```text id="api27"
404 Not Found
```

---

# API Request Types

---

# Route Parameter

```http id="api28"
GET /users/1
```

---

# Query Parameter

```http id="api29"
GET /users?page=1
```

---

# Request Body

Sent in POST/PUT.

Example:

```json id="api30"
{
  "name": "John"
}
```

---

# Model Binding

ASP.NET automatically converts JSON into C# object.

---

# Example

```csharp id="api31"
public IActionResult CreateUser(User user)
{
}
```

JSON automatically mapped.

---

# Dependency Injection (DI)

Very important in Web API.

Used for:

```text id="api32"
Injecting services into controllers
```

---

# Example

```csharp id="api33"
private readonly IUserService _service;

public UsersController(IUserService service)
{
    _service = service;
}
```

---

# Why DI Important

Helps:

* Loose coupling
* Easier testing
* Cleaner architecture

---

# Middleware

Middleware handles request pipeline.

Examples:

* Authentication
* Logging
* Error handling
* CORS

---

# Request Pipeline

```text id="api34"
Request
  ↓
Middleware
  ↓
Controller
  ↓
Response
```

---

# REST API

Most ASP.NET APIs follow:

```text id="api35"
REST architecture
```

---

# REST Basics

REST means:

```text id="api36"
Resources accessed using HTTP methods
```

---

# REST Example

| Operation    | Endpoint        |
| ------------ | --------------- |
| Get users    | GET /users      |
| Get one user | GET /users/1    |
| Create user  | POST /users     |
| Delete user  | DELETE /users/1 |

---

# Web API vs MVC

| Web API                  | MVC                           |
| ------------------------ | ----------------------------- |
| Returns JSON/data        | Returns Views/UI              |
| Backend service          | Full web pages                |
| Used for frontend/mobile | Used for server-rendered apps |

---

# Alternatives to ASP.NET Web API

---

# Node.js + Express

JavaScript backend.

---

# Spring Boot

Java backend framework.

---

# Django REST

Python backend.

---

# FastAPI

Modern Python API framework.

---

# Why ASP.NET Web API Popular

Because it provides:

* High performance
* Strong typing
* Enterprise support
* Built-in DI
* Security features
* Cross-platform support

---

# Common Real Use Cases

---

# Mobile Backend

Android/iOS apps call APIs.

---

# Frontend Backend Separation

React/Angular frontend uses APIs.

---

# Authentication Systems

Login/register APIs.

---

# E-commerce Systems

Orders/products/payment APIs.

---

# Microservices

Multiple small APIs communicating.

---

# Common Architecture

```text id="api37"
Client
  ↓
Controller
  ↓
Service
  ↓
Repository
  ↓
Database
```

---

# Authentication in APIs

Usually done using:

* JWT
* OAuth
* Cookies

---

# API Testing Tools

Commonly used:

| Tool    | Purpose                   |
| ------- | ------------------------- |
| Postman | API testing               |
| Swagger | API documentation/testing |

---

# Swagger

Swagger automatically shows:

* API endpoints
* Request format
* Response format

---

# Why Swagger Useful

Helps developers:

```text id="api38"
Understand and test APIs easily
```

---

# Important Beginner Concepts

---

# Stateless Nature

Each request independent.

Server does not automatically remember previous request.

---

# APIs Mostly Return Data

Not UI.

Usually JSON response.

---

# Controller Should Stay Thin

Avoid heavy business logic inside controller.

Use service layer.

---

# Final 

Web API mainly helps:

```text id="api39"
Applications communicate over HTTP
```

using:

* Endpoints
* Requests
* Responses
* JSON data

It acts as:

```text id="api40"
Backend communication layer
```

for modern applications.

---

#  Recall

```text id="api41"
Web API
 → Backend communication system

Main Components
 → Controller
 → Routing
 → Request
 → Response
 → Middleware

HTTP Methods
 → GET
 → POST
 → PUT
 → DELETE

Response Format
 → JSON

Common Usage
 → Frontend communication
 → Mobile backend
 → REST APIs
 → Microservices
```
