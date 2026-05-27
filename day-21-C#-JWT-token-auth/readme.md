# Authentication & JWT in ASP.NET Web API —  Notes

---

# What is Authentication?

Authentication means:

```text id="auth1"
Verifying who the user is
```

System checks:

```text id="auth2"
Is this user valid?
```

---

# Real Example

User enters:

* Email
* Password

System verifies from database.

If valid:

```text id="auth3"
User authenticated
```

---

# Authentication vs Authorization

---

# Authentication

```text id="auth4"
Who are you?
```

---

# Authorization

```text id="auth5"
What can you access?
```

---

# Real Example

| Process           | Meaning        |
| ----------------- | -------------- |
| Login             | Authentication |
| Admin page access | Authorization  |

---

# Why Authentication is Needed

Applications contain:

* Personal data
* Payments
* Orders
* Employee information
* Admin operations

Need secure access control.

---

# Common Authentication Methods

| Method                 | Usage                 |
| ---------------------- | --------------------- |
| Session Authentication | Traditional web apps  |
| JWT Authentication     | APIs/mobile/frontend  |
| OAuth                  | Google login          |
| Identity Server        | Enterprise auth       |
| API Keys               | Service communication |

---

# Most Common in Modern APIs

```text id="auth6"
JWT Authentication
```

---

# What is JWT?

JWT means:

```text id="auth7"
JSON Web Token
```

JWT is a:

```text id="auth8"
Digitally signed token
```

used for authentication.

---

# Main Idea of JWT

After successful login:

```text id="auth9"
Server generates token
```

Client stores token.

Client sends token in future requests.

---

# JWT Flow

```text id="auth10"
Login Request
      ↓
Server Validates User
      ↓
JWT Generated
      ↓
Client Stores JWT
      ↓
Client Sends JWT
with Every Request
      ↓
Server Verifies JWT
```

---

# Why JWT Became Popular

Because JWT is:

* Stateless
* Lightweight
* Fast
* Good for APIs
* Good for mobile apps

---

# Stateless Meaning

Server does NOT store login session.

Instead:

```text id="auth11"
JWT itself contains user information
```

---

# Structure of JWT

JWT contains 3 parts.

```text id="auth12"
Header
Payload
Signature
```

Format:

```text id="auth13"
xxxxx.yyyyy.zzzzz
```

---

# 1. Header

Contains:

* Token type
* Algorithm

Example:

```json id="auth14"
{
  "alg": "HS256",
  "typ": "JWT"
}
```

---

# 2. Payload

Contains user-related data.

Called:

```text id="auth15"
Claims
```

Example:

```json id="auth16"
{
  "userId": 1,
  "email": "john@gmail.com",
  "role": "Admin"
}
```

---

# 3. Signature

Used to verify:

```text id="auth17"
Token not modified
```

Created using:

* Secret key
* Header
* Payload

---

# Important JWT Understanding

JWT is:

```text id="auth18"
NOT encrypted by default
```

Payload can be decoded easily.

So NEVER store:

* Password
* Sensitive data
* Credit card details

inside JWT.

---

# What Usually Stored in JWT

Good claims:

* UserId
* Email
* Role
* Permissions

---

# Why JWT Used in APIs

Frontend/mobile app sends JWT in request header.

Server verifies token.

No session storage needed.

---

# Where JWT Sent

Usually inside:

```http id="auth19"
Authorization Header
```

Format:

```http id="auth20"
Authorization: Bearer TOKEN
```

---

# ASP.NET JWT Flow

```text id="auth21"
User Login
     ↓
Validate Credentials
     ↓
Generate JWT
     ↓
Return JWT
     ↓
Frontend Stores Token
     ↓
Frontend Sends Token
     ↓
API Validates JWT
```

---

# JWT Authentication Setup

---

# Required Package

```text id="auth22"
Microsoft.AspNetCore.Authentication.JwtBearer
```

---

# JWT Configuration

Inside:

```csharp id="auth23"
Program.cs
```

---

# Basic JWT Configuration

```csharp id="auth24"
builder.Services
.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "MyApp",

            ValidAudience = "MyApp",

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        "YourSecretKey"))
        };
});
```

---

# What This Configuration Does

Configures:

* Token validation
* Secret key
* Expiration check
* Issuer validation

---

# Middleware Setup

```csharp id="auth25"
app.UseAuthentication();

app.UseAuthorization();
```

---

# Important Rule

Always:

```text id="auth26"
UseAuthentication()
before
UseAuthorization()
```

---

# JWT Token Creation

---

# Example Login Method

```csharp id="auth27"
public string GenerateToken(User user)
{
    var claims = new[]
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.Id.ToString()),

        new Claim(
            ClaimTypes.Email,
            user.Email),

        new Claim(
            ClaimTypes.Role,
            user.Role)
    };

    var key =
        new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                "YourSecretKey"));

    var creds =
        new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

    var token =
        new JwtSecurityToken(
            issuer: "MyApp",
            audience: "MyApp",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

    return new JwtSecurityTokenHandler()
        .WriteToken(token);
}
```

---

# Important JWT Parts Here

---

# Claims

User information inside token.

---

# Secret Key

Used for signing token.

Must remain private.

---

# Expiration

Defines token lifetime.

---

# SigningCredentials

Defines signing algorithm.

---

# Login API Example

```csharp id="auth28"
[HttpPost("login")]
public IActionResult Login(LoginDto dto)
{
    // validate user

    var token = GenerateToken(user);

    return Ok(token);
}
```

---

# Protecting APIs

Use:

```csharp id="auth29"
[Authorize]
```

---

# Example

```csharp id="auth30"
[Authorize]
[HttpGet]
public IActionResult GetProfile()
{
    return Ok();
}
```

Now only authenticated users can access.

---

# Role-Based Authorization

Example:

```csharp id="auth31"
[Authorize(Roles = "Admin")]
```

Only admin can access.

---

# Accessing Logged-In User Info

```csharp id="auth32"
User.Claims
```

---

# Example

```csharp id="auth33"
var userId =
    User.FindFirst(
        ClaimTypes.NameIdentifier)?.Value;
```

---

# JWT Expiration

JWT should always expire.

Example:

```text id="auth34"
15 mins
1 hour
1 day
```

depending on application.

---

# Why Expiration Important

If token stolen:

```text id="auth35"
Limited damage
```

---

# What is Refresh Token?

Refresh token helps:

```text id="auth36"
Generate new access token
without login again
```

---

# Why Refresh Token Needed

Short JWT expiration improves security.

But user should not login repeatedly.

Refresh token solves this.

---

# Flow

```text id="auth37"
Access Token Expired
        ↓
Use Refresh Token
        ↓
Generate New JWT
```

---

# Access Token vs Refresh Token

| Access Token      | Refresh Token          |
| ----------------- | ---------------------- |
| Short life        | Long life              |
| Used in API calls | Used for token renewal |
| Sent frequently   | Sent rarely            |

---

# Where to Store JWT

---

# Frontend Options

| Storage         | Recommendation         |
| --------------- | ---------------------- |
| HttpOnly Cookie | Most secure            |
| LocalStorage    | Common but less secure |
| SessionStorage  | Temporary storage      |

---

# Best Practice

Prefer:

```text id="auth38"
HttpOnly Secure Cookies
```

for better security.

---

# Why LocalStorage Risky

Vulnerable to:

```text id="auth39"
XSS attacks
```

---

# JWT Best Practices

---

# Keep Payload Small

JWT sent in every request.

Avoid large data.

---

# Never Store Sensitive Data

Do NOT store:

* Password
* Bank details
* Secrets

---

# Use Expiration

Always set token expiry.

---

# Use HTTPS

JWT should never travel in plain HTTP.

---

# Use Strong Secret Key

Weak keys create security risk.

---

# Validate Everything

Always validate:

* Signature
* Expiration
* Issuer
* Audience

---

# Use Refresh Tokens

For long login sessions.

---

# Common JWT Use Cases

| Scenario             | Usage |
| -------------------- | ----- |
| React frontend + API | JWT   |
| Mobile apps          | JWT   |
| Microservices        | JWT   |
| SPA applications     | JWT   |

---

# Alternatives to JWT

---

# Session Authentication

Traditional server-side sessions.

Good for:

```text id="auth40"
MVC applications
```

---

# OAuth

Used for:

```text id="auth41"
Google login
Facebook login
```

---

# Identity Server / OpenId Connect

Enterprise authentication systems.

Used in:

* Large organizations
* SSO systems

---

# API Keys

Used for:

```text id="auth42"
System-to-system communication
```

---

# JWT vs Session Authentication

| JWT           | Session                |
| ------------- | ---------------------- |
| Stateless     | Server stores session  |
| Good for APIs | Good for MVC           |
| Scalable      | Simpler for small apps |
| Token-based   | Cookie/session-based   |

---

# Common Real Architecture

```text id="auth43"
Frontend
   ↓
Login API
   ↓
JWT Generated
   ↓
Frontend Stores Token
   ↓
Protected APIs Use JWT
```

---

# Final Understanding

Authentication helps:

```text id="auth44"
Verify user identity
```

JWT helps:

```text id="auth45"
Securely authenticate API requests
without server-side session storage
```

Modern APIs commonly use JWT because it is:

* Stateless
* Scalable
* API-friendly
* Frontend-friendly

---

# Quick Recall

```text id="auth46"
Authentication
 → Verify user

Authorization
 → Verify access permission

JWT
 → JSON Web Token

JWT Parts
 → Header
 → Payload
 → Signature

JWT Flow
 → Login
 → Generate Token
 → Send Token
 → Validate Token

Best Practices
 → Use expiration
 → Use HTTPS
 → Keep payload small
 → Never store sensitive data
 → Use refresh token
```
