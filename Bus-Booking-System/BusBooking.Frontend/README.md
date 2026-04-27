# Bus Booking — Frontend

Angular app for the GO-BUS bus booking system. It talks to the backend API at `http://localhost:5011/api` (see `src/app/core/environment.ts`).

---

## What this app does

### Everyone (no login)

- **Login / Register** — create an account or sign in with JWT.

### Traveller (USER)

- **Search buses** — by source, destination, and date; see boarding point info on each result.
- **Pick seats** — seat map, lock seats while you checkout.
- **Payment** — review trip, confirm booking, download a **PDF invoice** after success.
- **Dashboard** — upcoming, past, and cancelled bookings; cancel a booking (in-app confirm, not browser alert).
- **Profile** — view/update name and phone; see role from the server.
- **Apply as operator** — send operator application; see pending / approved status.

### Bus operator (OPERATOR)

- **Dashboard** — booking counts, revenue (total and per bus), bus list, simple notices.
- **Add bus** — choose admin route, times, price, seats, bus number, source/destination **boarding points** from your list.
- **Manage buses** — set bus active or unavailable; request **route change** (needs admin approval); if admin deactivated a bus, **request reactivation** and read admin reasons on the card; **delete bus** when allowed (no bookings).
- **Boarding points** — save addresses per city/area for use when adding buses.

### Admin (ADMIN)

- **Dashboard** — platform stats and overview.
- **Bus requests** — approve or reject new buses from operators.
- **Operator requests** — approve or reject operator applications.
- **Operators** — list all operators; open **operator detail**; deactivate a bus with a reason (in-app prompt).
- **Routes** — add routes; delete a route if no bus uses it.
- **Route change requests** — approve or reject operator route changes.
- **Reactivation requests** — approve or reject bus reactivation after admin deactivation.

### Shared UI

- **Navbar** — links change by role (user / operator / admin).
- **Mail widget** — small inbox in the corner: list messages, open, reply, delete (mock mail API).
- **Dialog** — shared confirm / prompt modals instead of `alert` / `confirm` / `prompt` in main flows.

---

## Run locally

```bash
npm install
npm start
```

Open **http://localhost:4200**. The backend must be running (default **http://localhost:5011**) and CORS must allow this origin.

---

## Build

```bash
npm run build
```

Output goes to `dist/`.

---

## Stack (short)

- Angular 21, standalone components, `HttpClient`, routing with `authGuard` and role checks.
- **jspdf** + **jspdf-autotable** for the payment PDF bill.

For full setup (database, API, secrets), use the main project **README** in the folder above (`Bus-Booking-System/README.md`).
