# Angular Project Structure

When Angular creates a project, you'll see something like this:

```text
my-app/
│
├── src/
│   │
│   ├── app/
│   │   ├── app.component.ts
│   │   ├── app.component.html
│   │   ├── app.component.css
│   │   └── app.component.spec.ts
│   │
│   ├── assets/
│   ├── index.html
│   ├── main.ts
│   └── styles.css
│
├── package.json
├── angular.json
└── node_modules/
```

---

# How Angular Starts

```text
Browser Opens
      ↓
index.html
      ↓
main.ts
      ↓
App Component
      ↓
Other Components
```

Everything starts from `main.ts`.

---

# main.ts

### Purpose

Entry point of the application.

Angular starts execution from this file.

### Responsibility

* Starts Angular
* Loads the root component
* Initializes the application

### Example

```typescript
bootstrapApplication(AppComponent);
```

Meaning:

```text
Start Angular
Use AppComponent as root
Render the application
```

---

# index.html

### Purpose

Main HTML page loaded by the browser.

### Responsibility

* Contains root tag
* Hosts Angular application
* First page loaded by browser

Example:

```html
<body>
   <app-root></app-root>
</body>
```

Angular finds:

```html
<app-root>
```

and replaces it with the application's UI.

---

# app Folder

### Purpose

Contains application source code.

Most development work happens here.

Example:

```text
app/
│
├── components
├── services
├── models
├── pages
└── shared
```

---

# app.component.ts

### Purpose

Component logic.

### Responsibility

* Variables
* Functions
* API calls
* Business logic
* State management

Example:

```typescript
export class AppComponent {

  title = "Angular App";

  showMessage() {
    alert("Hello");
  }

}
```

Think:

```text
What should happen?
```

is decided here.

---

# app.component.html

### Purpose

User Interface.

### Responsibility

* Display data
* Show buttons
* Forms
* Tables
* Layout structure

Example:

```html
<h1>{{title}}</h1>

<button (click)="showMessage()">
  Click
</button>
```

Think:

```text
What should the user see?
```

is defined here.

---

# app.component.css

### Purpose

Component styling.

### Responsibility

* Colors
* Fonts
* Spacing
* Layout
* Responsive design

Example:

```css
h1{
   color:red;
}
```

Think:

```text
How should it look?
```

is handled here.

---

# Component Relationship

```text
app.component.ts
      ↓
Provides Data & Logic
      ↓
app.component.html
      ↓
Displays Data
      ↓
app.component.css
      ↓
Applies Styling
```

---

# app.component.spec.ts

### Purpose

Testing file.

### Responsibility

* Verify component behavior
* Validate logic
* Automated testing

Example:

```typescript
expect(component).toBeTruthy();
```

Used when testing application functionality.

---

# styles.css

### Purpose

Global stylesheet.

### Responsibility

Styles applied across the entire application.

Example:

```css
body{
   margin:0;
}
```

---

## Difference

### Component CSS

```text
app.component.css
```

Affects only that component.

### Global CSS

```text
styles.css
```

Affects the entire application.

---

# assets Folder

### Purpose

Store static resources.

Example:

```text
assets/
│
├── images
├── icons
├── pdf
└── videos
```

Common files:

```text
logo.png
banner.jpg
guide.pdf
```

Usage:

```html
<img src="assets/logo.png">
```

---

# package.json

### Purpose

Project dependency configuration.

### Responsibility

* Package list
* Project scripts
* Dependency versions

Example:

```json
{
  "dependencies": {
    "@angular/core": "...",
    "@angular/router": "...",
    "rxjs": "..."
  }
}
```

Whenever a package is installed:

```bash
npm install package-name
```

it gets registered here.

---

# node_modules

### Purpose

Stores installed packages.

Contains:

```text
Angular
RxJS
TypeScript
Third-party libraries
```

Generated automatically from `package.json`.

---

# angular.json

### Purpose

Angular project configuration.

### Responsibility

* Build settings
* Asset configuration
* Style configuration
* Environment configuration

Example:

```json
{
  "styles": [
    "src/styles.css"
  ]
}
```

Controls how Angular builds and runs the project.

---

# Common Structure in Real Projects

```text
app/
│
├── components/
│
├── pages/
│
├── services/
│
├── models/
│
├── guards/
│
├── interceptors/
│
├── shared/
│
└── app.routes.ts
```

---

# Components

### Purpose

Reusable UI pieces.

Examples:

```text
Navbar
Footer
Sidebar
Course Card
Product Card
```

---

# Pages

### Purpose

Complete screens.

Examples:

```text
Home Page
Login Page
Dashboard
Profile Page
```

---

# Services

### Purpose

Shared business logic.

### Responsibility

* API calls
* Data sharing
* Authentication
* Reusable operations

Example:

```text
UserService
CourseService
AuthService
```

---

# Models

### Purpose

Represent data structures.

Example:

```typescript
export interface User {
   id:number;
   name:string;
   email:string;
}
```

Used to define the shape of data.

---

# Guards

### Purpose

Control route access.

Example:

```text
User logged in?
      ↓
Yes → Allow
No  → Redirect Login
```

---

# Interceptors

### Purpose

Intercept HTTP requests and responses.

Common uses:

```text
Attach JWT Token
Log Requests
Handle Errors
Modify Headers
```

---

# Shared Folder

### Purpose

Store reusable items.

Examples:

```text
Common Components
Pipes
Directives
Utilities
```

Used across multiple areas of the application.

---

# app.routes.ts

### Purpose

Application navigation.

Example:

```typescript
[
  { path:'home', component:HomeComponent },
  { path:'login', component:LoginComponent }
]
```

Controls:

```text
URL
   ↓
Component/Page
```

---

# Complete Picture

```text
Browser
   ↓
index.html
   ↓
main.ts
   ↓
App Component
   ↓
Routes
   ↓
Pages
   ↓
Components
   ↓
Services
   ↓
API / Database
```

# Quick Reference

| File/Folder           | Responsibility                 |
| --------------------- | ------------------------------ |
| main.ts               | Start Angular application      |
| index.html            | Host page                      |
| app.component.ts      | Logic                          |
| app.component.html    | UI                             |
| app.component.css     | Styling                        |
| app.component.spec.ts | Testing                        |
| styles.css            | Global styles                  |
| assets                | Images, PDFs, icons            |
| services              | API calls & shared logic       |
| models                | Data structures                |
| guards                | Route protection               |
| interceptors          | HTTP request/response handling |
| app.routes.ts         | Navigation                     |
| package.json          | Dependencies                   |
| node_modules          | Installed packages             |
| angular.json          | Project configuration          |

A simple way to remember:

```text
HTML  → What user sees
CSS   → How it looks
TS    → How it works

Service → Shared logic
Model   → Data shape
Guard   → Access control
Interceptor → Request control
Route   → Navigation

main.ts → Start
index.html → Host
```
