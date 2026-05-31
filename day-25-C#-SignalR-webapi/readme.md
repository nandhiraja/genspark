# SignalR in ASP.NET Core —  Notes

---

# What is SignalR?

SignalR is a library in ASP.NET Core used for:

```text
Real-time communication
between server and clients
```

---

# What is Real-Time Communication?

Normally:

```text
Client asks for data
Server sends response
```

Example:

```text
Refresh page
↓
Get latest data
```

This is request-response communication.

---

With SignalR:

```text
Server can push data immediately
```

without user refreshing.

---

# Why SignalR is Needed

Some applications need instant updates.

Examples:

* Chat applications
* Live notifications
* Online gaming
* Stock market updates
* Live dashboards
* Ride tracking
* Food delivery tracking

---

# Real Example

WhatsApp:

```text
User A sends message
        ↓
Server receives message
        ↓
Server immediately pushes
message to User B
```

No refresh needed.

---

# Without SignalR

Client continuously asks:

```text
Any new message?
Any new message?
Any new message?
```

Called:

```text
Polling
```

Waste of resources.

---

# With SignalR

```text
Message arrives
      ↓
Server pushes instantly
      ↓
Client receives instantly
```

Much more efficient.

---

# Main Purpose of SignalR

```text
Server → Client communication
in real time
```

---

# Basic Communication Flow

```text
Client Connected
        ↓
Connection Established
        ↓
Server Sends Updates
        ↓
Client Receives Updates
```

---

# How SignalR Works Internally

SignalR automatically chooses best transport.

Possible transports:

```text
WebSockets
Server Sent Events (SSE)
Long Polling
```

---

# Preferred Transport

```text
WebSockets
```

because it provides:

* Full duplex communication
* Fast updates
* Persistent connection

---

# Good News

Developers usually don't manage transports manually.

SignalR handles it automatically.

---

# Core Components of SignalR

---

# 1. Hub

Most important component.

Hub acts like:

```text
Communication center
```

between server and clients.

---

# Example

```csharp
public class ChatHub : Hub
{
}
```

---

# Responsibility of Hub

Handles:

* Sending messages
* Receiving messages
* Broadcasting updates

---

# Simple Mental Model

```text
Controller
     ↓
HTTP Requests

Hub
     ↓
Real-Time Communication
```

---

# 2. Client

Client can be:

* React app
* Angular app
* Mobile app
* ASP.NET MVC app
* Blazor app

---

# 3. Connection

When client joins:

```text
Client
   ↓
SignalR Hub
```

connection is established.

---

# SignalR Flow

```text
Client Connects
      ↓
Hub Created
      ↓
Messages Exchanged
      ↓
Connection Closed
```

---

# Creating a Hub

Example:

```csharp
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
}
```

---

# Sending Message to All Clients

```csharp
public async Task SendMessage(
    string message)
{
    await Clients.All.SendAsync(
        "ReceiveMessage",
        message);
}
```

---

# Understanding

---

# Clients.All

Means:

```text
Send to every connected client
```

---

# SendAsync()

Means:

```text
Send event to clients
```

---

# ReceiveMessage

Event name client listens for.

---

# Common Client Targets

---

# All Clients

```csharp
Clients.All
```

Everyone receives message.

---

# Single Client

```csharp
Clients.Client(connectionId)
```

One specific client.

---

# Group of Clients

```csharp
Clients.Group(groupName)
```

Only group members.

---

# Current Client

```csharp
Clients.Caller
```

Only requesting client.

---

# Others Except Current

```csharp
Clients.Others
```

Everyone except sender.

---

# Groups in SignalR

Very useful feature.

---

# What is Group?

Group means:

```text
Collection of connected clients
```

---

# Example

Chat Rooms:

```text
Java Room
C# Room
Python Room
```

Each room is a group.

---

# Adding User to Group

```csharp
await Groups.AddToGroupAsync(
    Context.ConnectionId,
    "Developers");
```

---

# Sending to Group

```csharp
await Clients.Group("Developers")
    .SendAsync(
        "ReceiveMessage",
        message);
```

---

# ConnectionId

Every connected client gets:

```text
Unique Connection ID
```

Example:

```text
123abc456
```

Used for targeting specific users.

---

# Hub Context

Sometimes we need SignalR outside Hub.

Example:

* Service layer
* Background jobs
* Scheduled tasks

Use:

```csharp
IHubContext<NotificationHub>
```

---

# Example

```csharp
public class NotificationService
{
    private readonly
    IHubContext<NotificationHub>
    _hubContext;
}
```

---

# Why HubContext Useful

Example:

```text
Order created
      ↓
Service layer
      ↓
Push notification
```

without calling Hub directly.

---

# Client Events

Client listens for server messages.

Example:

```text
ReceiveMessage
NewNotification
OrderUpdated
```

---

# Real Applications of SignalR

---

# Chat System

```text
User A
    ↓
Server
    ↓
User B
```

Instant messaging.

---

# Notifications

Example:

```text
New Order
Payment Success
New Message
```

Instant updates.

---

# Live Dashboard

Example:

```text
Active Users
Sales Count
Revenue
```

Updates automatically.

---

# Tracking Systems

Examples:

* Delivery tracking
* Cab tracking
* GPS monitoring

---

# Online Multiplayer Games

Player movements instantly shared.

---

# SignalR Authentication

SignalR supports:

* JWT
* Cookies
* ASP.NET Identity

---

# Common Setup

API uses JWT.

SignalR connection also validates JWT.

---

# Security Considerations

Always:

* Authenticate users
* Authorize access
* Validate incoming data

---

# Scaling SignalR

Single server works easily.

Large applications may need:

```text
Redis Backplane
Azure SignalR Service
```

for multiple servers.

---

# Why Scaling Needed

Example:

```text
Server A
Server B
Server C
```

Users connected to different servers.

Messages must reach everyone.

---

# SignalR vs Web API

| Web API          | SignalR               |
| ---------------- | --------------------- |
| Request-Response | Real-Time             |
| Client asks      | Server pushes         |
| Stateless        | Persistent connection |
| CRUD operations  | Live updates          |

---

# When to Use Web API

Good for:

* Create user
* Get products
* Place order
* CRUD operations

---

# When to Use SignalR

Good for:

* Notifications
* Chat
* Live updates
* Tracking systems

---

# Common Architecture

```text
Frontend
     ↓
Web API
     ↓
Business Logic
     ↓
Database

          +
          ↓

SignalR Hub
     ↓
Real-Time Updates
```

---

# Example Real Flow

Food Delivery App:

```text
Driver Location Changes
        ↓
Database Updated
        ↓
SignalR Sends Update
        ↓
Customer Map Updates
```

No refresh needed.

---

# Best Practices

---

# Keep Hubs Thin

Good:

```text
Hub
 ↓
Call Service Layer
```

Avoid putting business logic in Hub.

---

# Use Groups

For targeted communication.

---

# Authenticate Users

Never trust anonymous clients.

---

# Send Small Messages

Avoid huge payloads.

---

# Handle Disconnects

Users may lose internet.

Always handle reconnection.

---

# Common Methods in Hub

| Method                | Purpose             |
| --------------------- | ------------------- |
| SendAsync()           | Send message        |
| OnConnectedAsync()    | Client connected    |
| OnDisconnectedAsync() | Client disconnected |

---

#  Recall

```text
SignalR
 → Real-time communication library

Main Component
 → Hub

Purpose
 → Server pushes updates instantly

Common Uses
 → Chat
 → Notifications
 → Tracking
 → Dashboards

Targets
 → All
 → Caller
 → Others
 → Client
 → Group

Important Concepts
 → Hub
 → ConnectionId
 → Groups
 → HubContext

Use SignalR
 → Real-time updates

Use Web API
 → Normal CRUD operations
```

---

# Final Understanding

SignalR is essentially:

```text
A real-time communication layer
for ASP.NET applications
```

It allows:

```text
Server
   ↓
Immediately notify clients
```

without requiring page refreshes or repeated API requests.
