# Notes: Policy-Based Authorization in ASP.NET Core (C#)

## 1. Introduction to Authorization
Authorization is the process of determining what a verified user is allowed to do within an application. While **Authentication** verifies *who* a user is (via login credentials, tokens, etc.), **Authorization** determines *what permissions* they possess.

In older legacy systems, access control was predominantly managed through **Role-Based Authorization** (e.g., `[Authorize(Roles = "Admin")]`). While simple, role-based checks tightly couple controller logic to external structural hierarchies. If a role name changes or responsibilities shift, developers must change attributes throughout the source code.

**Policy-Based Authorization** decouples this logic. It acts as an abstraction layer where the controller checks for a named **Policy** (e.g., `[Authorize(Policy = "ElevatedAccess")]`), and the underlying business rules dictating what constitutes "Elevated Access" are managed centrally.

---

## 2. Architectural Components
Policy-Based Authorization relies on three decoupled components working in tandem:

```
[ Controller / Endpoint ] 
           │
           ▼
   [ Named Policy ] ───► Comprises ───► [ IAuthorizationRequirement ]
                                                   │
                                                   ▼
                                       [ AuthorizationHandler<T> ]
```

### IAuthorizationRequirement
A requirement is a minimalist, structural data contract representing a specific rule or parameter that must be validated. It implements the empty marker interface `IAuthorizationRequirement`. Requirements should ideally contain no business logic; they only hold data parameters required for evaluation (such as a minimum age limit, a target department name, or a spending threshold).

### AuthorizationHandler<T>
The handler contains the actual execution logic. It inherits from `AuthorizationHandler<TRequirement>`, where `TRequirement` is the specific requirement class it evaluates. The handler overrides `HandleRequirementAsync`, inspecting the incoming security principal (the user's claims) and the context parameters to determine if the criteria are met. If met, it explicitly marks the requirement as satisfied.

### The Policy
The policy is a named configuration registered globally at application startup. It serves as an aggregation boundary that maps a descriptive string key (e.g., `"MustBeITStaff"`) to one or many underlying requirements.

---

## 3. Comprehensive Code Implementation

Below is a complete, production-ready blueprint illustrating the separation of concerns.

### Step 1: Define the Requirement
```csharp
using Microsoft.AspNetCore.Authorization;

namespace SecurityArchitecture.Authorization
{
    // A simple, immutable data structure defining the requirement payload
    public class DepartmentRequirement : IAuthorizationRequirement
    {
        public string RequiredDepartment { get; }

        public DepartmentRequirement(string department)
        {
            RequiredDepartment = department;
        }
    }
}
```

### Step 2: Implement the Authorization Handler
```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SecurityArchitecture.Authorization
{
    public class DepartmentHandler : AuthorizationHandler<DepartmentRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            DepartmentRequirement requirement)
        {
            // 1. Inspect the Security Principal for the target claim
            var departmentClaim = context.User.FindFirst(c => c.Type == "Department")?.Value;

            // 2. Evaluate business logic criteria
            if (departmentClaim != null && 
                string.Equals(departmentClaim, requirement.RequiredDepartment, System.StringComparison.OrdinalIgnoreCase))
            {
                // 3. Mark the requirement as explicitly successful
                context.Succeed(requirement);
            }

            // Authorization handlers are asynchronous; return a completed task thread execution
            return Task.CompletedTask;
        }
    }
}
```

### Step 3: Registering within the Dependency Injection Container
```csharp
using Microsoft.AspNetCore.Authorization;
using SecurityArchitecture.Authorization;

var builder = WebApplication.CreateBuilder(args);

// 1. Add application controllers to the container
builder.Services.AddControllers();

// 2. Register the Authorization Handler within the DI framework
// Handlers can be Singleton if stateless, or Scoped if they query a database context
builder.Services.AddSingleton<IAuthorizationHandler, DepartmentHandler>();

// 3. Configure and bind Policies to Requirements
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ITSpaceOnly", policy =>
        policy.AddRequirements(new DepartmentRequirement("IT")));
        
    options.AddPolicy("HRSpaceOnly", policy =>
        policy.AddRequirements(new DepartmentRequirement("HR")));
});

var app = builder.Build();

app.UseRouting();

// Authentication MUST always run before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Step 4: Securing Endpoints via Controllers
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecurityArchitecture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NetworkInfrastructureController : ControllerBase
    {
        // This endpoint is fully protected by the policy abstraction layer
        [HttpGet("switch-config")]
        [Authorize(Policy = "ITSpaceOnly")]
        public IActionResult GetSwitchConfiguration()
        {
            return Ok(new { Config = "VLAN 10, Port 2 Enabled", Status = "Secure" });
        }
    }
}
```

---

## 4. Multi-Handler Execution Mechanics
A fundamental architecture pattern in ASP.NET Core is the capability to register **multiple handlers** for a single requirement or across an entire application ecosystem. 

```
                    ┌──► EmployeeBadgeHandler  ──► [Succeed?] ──┐
                    │                                           ├──► Evaluated as OR
[Policy Check] ────┼──► GuestPassHandler       ──► [Succeed?] ──┤
                    │                                           │
                    └──► RemoteVendorHandler    ──► [Succeed?] ──┘
```

When evaluating a policy, ASP.NET Core fetches all classes registered under `IAuthorizationHandler` from the DI framework. It filters handlers matching the requirement types configured inside the targeted policy.

### Logical Evaluation Rules
* **AND Evaluation (Multiple Requirements inside one Policy):** If a single named policy contains multiple distinct requirements (e.g., `policy.AddRequirements(new AgeRequirement(21), new CountryRequirement("US"))`), **ALL** requirements must be successfully validated by their respective handlers. If even one fails, access is denied.
* **OR Evaluation (Multiple Handlers for a single Requirement):** If you register multiple independent handlers processing the *exact same* requirement class, ASP.NET Core treats them as an **OR** condition. The system iterates through every registered handler. If **any single handler** calls `context.Succeed(requirement)`, the requirement is passed. This allows for diverse access pathways (e.g., allowing entry if a user is an internal employee via `EmployeeBadgeHandler` **OR** a registered contractor via `ContractorPassHandler`).

### Explicit Failures vs Passive Failures
Inside a handler, you have access to two distinct termination methodologies:
1. `context.Succeed(requirement)`: Explicitly flags that the current handler has cleared the security hurdle.
2. `context.Fail()`: Explicitly blocks authorization immediately. If any handler invokes `context.Fail()`, the entire request is blocked, overriding all other successful handlers. 

If a handler discovers that a user does not meet its criteria, it should simply do nothing and return `Task.CompletedTask`. This allows secondary handlers processing the same requirement a chance to succeed.

---

## 5. Summary Cheat Sheet

| Concept | Definition / Role | Registration Lifetime |
| :--- | :--- | :--- |
| **Policy** | String identifier mapping to a checklist of requirements. | `Program.cs` options pipeline. |
| **Requirement** | Parameterized data model declaring structural rules. | Passed into Policy config. |
| **Handler** | Active class executing the conditional validation logic. | Added directly to DI container (`AddSingleton`/`AddScoped`). |
| **`context.Succeed()`**| Method indicating a current requirement has been validated. | Called inside the handler block. |
| **`context.Fail()`**| Method forcing immediate rejection, halting all alternate handlers. | Called for explicit security blacklisting. |