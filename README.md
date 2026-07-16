# Online Railway Reservation System

A RESTful Web API built with ASP.NET Core 10 for managing railway reservations. The system supports passenger booking flows, train management, and PNR tracking with secure JWT-based authentication.

## What It Does

**Authentication**
- User sign-up and sign-in with JWT token generation
- Role-based authorization (Passenger / Admin)

**Passenger Features**
- Search trains by source, destination, or train details
- Book tickets and receive a PNR number
- Check booking status
- Check PNR status for any reservation
- Cancel bookings

**Admin Features**
- Add new trains to the system
- Delete existing trains
- Update Existing trains
- View all Bookings

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/signup` | Register a new user |
| POST | `/api/auth/signin` | Login and receive JWT token |
| GET | `/api/trains/search` | Search trains |
| POST | `/api/booking` | Book a ticket |
| GET | `/api/booking/status` | Check booking status |
| GET | `/api/booking/pnr/{pnr}` | Check PNR status |
| POST | `/api/trains` | Add a new train (Admin) |
| DELETE | `/api/trains/{id}` | Delete a train (Admin) |

## Built With

- **ASP.NET Core 10** — Web API framework
- **Entity Framework Core** — ORM for database operations and migrations
- **SQL Server** — relational database
- **JWT Authentication** — secure token-based auth
- **NUnit** — unit testing
- **Dependency Injection** — built-in ASP.NET Core DI container
- **Async/Await** — asynchronous request handling throughout

## How to Run

1. Clone the repository
2. Update the connection string in `appsettings.json` to point to your SQL Server instance
3. Apply migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the project:
   ```bash
   dotnet run
   ```
5. Open Swagger UI at `https://localhost:{port}/swagger` to explore and test the endpoints

## Testing

NUnit test cases are included covering core booking and authentication logic. To run tests:
```bash
dotnet test
```

## Architecture

- **Controllers** — handle HTTP requests and route to services
- **Services** — business logic layer
- **Repositories** — data access via EF Core
- **Models / DTOs** — entity definitions and data transfer objects
- **Middleware** — custom exception handling and request pipeline

Detailed Low-Level Design (LLD) documentation with architecture and entity diagrams is included in the `/docs` folder.

---
*Project by Akhil Sanjay — Capgemini Training, B.Tech Data Science, Presidency University*
