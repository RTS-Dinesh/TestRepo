# RentApp API

A .NET 8 Web API for a **rent-only** marketplace (like OLX, but for renting). Users can list items in categories worldwide and rent from each other.

## Features

- **Categories** – Browse and manage rent categories (Electronics, Vehicles, Property, Tools, Clothing, Sports, Events, Books, etc.).
- **Users** – Register and manage users (listers and renters).
- **Listings** – Create and search listings with price per day/week/month, location, and category.
- **Rentals** – Request and manage rentals (start/end date, total amount, status: Pending, Confirmed, InProgress, Completed, Cancelled).

## Run the API

```bash
cd RentApp.Api
dotnet run
```

- **API base:** `https://localhost:7xxx` (see console for port).
- **Swagger UI:** `https://localhost:7xxx/swagger`.

By default the app uses **In-Memory** storage (no database setup). To use **SQL Server**, set `ConnectionStrings:UseInMemory` to `false` in `appsettings.json` and set `DefaultConnection`, then run:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## API Endpoints

| Resource   | Endpoints |
|-----------|-----------|
| **Categories** | `GET/POST /api/categories`, `GET/PUT/DELETE /api/categories/{id}`, `GET /api/categories/{id}/listings` |
| **Users**      | `GET/POST /api/users`, `GET/PUT/DELETE /api/users/{id}` |
| **Listings**   | `GET/POST /api/listings`, `GET/PUT/DELETE /api/listings/{id}` (GET supports `categoryId`, `city`, `country`, `maxPricePerDay`, `isAvailableOnly`) |
| **Rentals**    | `GET/POST /api/rentals`, `GET /api/rentals/{id}`, `PATCH /api/rentals/{id}/status` (GET supports `listingId`, `renterId`) |

## Example flow

1. **Get categories:** `GET /api/categories` (seeded when using In-Memory).
2. **Create a user (owner):** `POST /api/users` with `{ "name": "Jane", "email": "jane@example.com", "city": "Lahore", "country": "Pakistan" }`.
3. **Create a listing:** `POST /api/listings` with title, description, `categoryId`, `ownerId`, `pricePerDay`, etc.
4. **Create another user (renter):** `POST /api/users` with renter details.
5. **Create a rental:** `POST /api/rentals` with `listingId`, `renterId`, `startDate`, `endDate`; total is computed from listing’s daily rate.
6. **Update rental status:** `PATCH /api/rentals/{id}/status` with `{ "status": 1 }` (e.g. Confirmed).

## Tech stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 (SQL Server or In-Memory)
- Swagger/OpenAPI
