# GO-BUS — Bus Booking System

Full-stack bus booking platform with **travellers**, **bus operators**, and **admins**. Users search approved buses, pick seats, pay (mock flow), and get a **PDF invoice**. Operators manage buses, boarding points, route-change requests, and reactivation after admin deactivation. Admins approve operators and buses, manage routes, and handle request queues.

---

## Tech stack

| Layer | Technology |
|--------|------------|
| **API** | ASP.NET Core 8, JWT auth, Swagger |
| **Database** | PostgreSQL + Entity Framework Core + Npgsql |
| **Web app** | Angular 21 (standalone components), RxJS |
| **PDF** | jsPDF + jspdf-autotable (client-side invoice) |

---

## Repository layout

```
Bus-Booking-System/
├── BusBooking.Backend/     # REST API (Program.cs, Controllers, Models, Migrations)
└── BusBooking.Frontend/    # Angular SPA (src/app: core, modules, shared)
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (LTS recommended; project pins npm in `package.json`)
- [PostgreSQL](https://www.postgresql.org/download/) running locally (or a reachable instance)

---

## Configuration (backend)

Edit `BusBooking.Backend/appsettings.json` (or use user secrets / environment variables in production):

| Section | Purpose |
|---------|---------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `JwtSettings` | Signing key, issuer, audience, token lifetime |
| `EmailSettings` | SMTP for real email (optional for local dev) |

**Defaults in repo:** connection string uses `Host=localhost;Port=5432;Database=BusBookingDb;Username=postgres;Password=root` — change this to match your machine.

**Security:** replace placeholder JWT secret and SMTP credentials before any shared or production deployment. Never commit real secrets.

---

## Database setup

1. Create an empty database (e.g. `BusBookingDb`) in PostgreSQL.
2. Point `DefaultConnection` at that database.
3. From the backend project folder, apply EF migrations:

```bash
cd BusBooking.Backend
dotnet ef database update
```

If the `dotnet ef` tool is missing:

```bash
dotnet tool install --global dotnet-ef
```

---

## Run the API

```bash
cd BusBooking.Backend
dotnet run
```

- **HTTP API:** `http://localhost:5011` (see `Properties/launchSettings.json`)
- **Swagger UI:** `http://localhost:5011/swagger` (Development environment)

API routes are under `/api/...` (controllers use `[Route("api/[controller]")]` pattern; the Angular app calls `http://localhost:5011/api` as the base URL).

---

## Run the web app

```bash
cd BusBooking.Frontend
npm install
npm start
```

- **App URL:** `http://localhost:4200` (Angular dev server default)
- **API base URL:** `BusBooking.Frontend/src/app/core/environment.ts` → `apiUrl: 'http://localhost:5011/api'`

The backend **CORS** policy allows `http://localhost:4200` with credentials. If you change the frontend origin or port, update `Program.cs` (`AllowFrontend` policy) accordingly.

---

## Default admin account (seeded)

On first startup, if no user exists with this email, the API seeds an admin:

| Field | Value |
|--------|--------|
| **Email** | `admin@ridebus.com` |
| **Password** | `admin123` |

Change this password after first login in a real environment.

---

## Roles and routing

| Role | Typical paths |
|------|----------------|
| **USER** | Search → seats → payment → dashboard, profile, apply for operator |
| **OPERATOR** | Dashboard, add bus, manage buses, boarding points |
| **ADMIN** | Dashboard, bus/operator approvals, operators list & detail, routes, route-change requests, reactivation requests |

Guards enforce JWT role claims. After an admin **approves** an operator, the profile role can change; the app may require **sign out and sign in again** so the new role is in the token.

---

## Main features (by area)

### Traveller (user)

- Search buses by route and date; see operator **boarding point** labels and addresses on cards.
- Seat map, temporary **seat lock**, checkout and booking confirmation.
- **Dashboard:** upcoming / past / cancelled bookings; cancel with in-app confirmation (not browser `alert`).
- **Profile** and **apply to become an operator** (pending until admin approves).
- **Payment success:** download a styled **PDF** bill (jsPDF).

### Operator

- Dashboard: bookings, revenue (overall and bus-level), bus list, notices.
- **Add bus:** route from admin catalog, times, price, seats, **source/destination boarding points** from operator-defined list.
- **Boarding points:** per city/area addresses linked to route cities.
- **Manage buses:** status (available vs unavailable), **route change request** (admin approval), **reactivation request** if admin deactivated the bus; see admin deactivation / rejection reasons on cards.
- **Delete bus:** allowed only when the bus has **no booking history** (API returns 400 otherwise).

### Admin

- Dashboard metrics and polish layout.
- Approve/reject **new buses** and **operator applications**; deactivate buses (with reason); manage **routes** (add/delete when no buses attached).
- Queues: **route change** and **bus reactivation** requests; navigation badges for pending counts.
- Operator list and **operator detail** (by operator id).

### Mail (in-app + optional SMTP)

- **Mock inbox** API stores messages users see in the floating **mail widget**; replies can still attempt **real SMTP** (failures are handled so the inbox flow does not always 500).
- Delete and mark-read from the widget where supported.

---

## Useful API groups (high level)

- **Auth** — register, login, JWT, `/Auth/me` for profile/role.
- **User** — search buses, locations, bus detail, seats-related data used by booking UI.
- **Booking** — lock/unlock seats, confirm, cancel.
- **Operator** — apply, status, cities, boarding points, buses, revenue, route change, reactivation, delete bus.
- **Admin** — dashboard, routes, operators, approvals, deactivation, route-change and reactivation queues.
- **Mail** — inbox, read, reply, delete (mock persistence).

Exact paths are easiest to explore in **Swagger** while the API is running.

---

## Troubleshooting

| Symptom | What to check |
|---------|----------------|
| **500 on API** after pulling code | Run `dotnet ef database update`; restart the API so migrations and code match. |
| **401 / wrong role** after approval | Log out and log in again to refresh the JWT. |
| **CORS errors** | Frontend must be on the origin allowed in `Program.cs` (default `http://localhost:4200`). |
| **Add bus 500** | Often **DateTime** must be UTC for PostgreSQL; ensure client sends sensible times and backend is current. |
| **SMTP / reply errors** | Configure `EmailSettings` or rely on mock inbox; failed SMTP should not always block saving the mock reply. |

---

## Scripts (frontend)

| Command | Description |
|---------|-------------|
| `npm start` | Dev server (`ng serve`) |
| `npm run build` | Production build |
| `npm test` | Unit tests (Karma/Jasmine if configured) |

---

## License / contribution

This project is for **learning / internship** use unless you add an explicit license. Adjust this section for your team or course requirements.

---

**Summary:** Start PostgreSQL → configure `appsettings.json` → `dotnet ef database update` → `dotnet run` in **BusBooking.Backend** → `npm install` && `npm start` in **BusBooking.Frontend** → open `http://localhost:4200` and sign in (or register a user; use seeded admin for admin features).
