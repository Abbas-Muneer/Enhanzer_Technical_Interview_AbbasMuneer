# Enhanzer Full Stack Assignment Submission

## Project Summary

This submission implements the Mini Purchase Management Module using:

- Angular
- TypeScript
- Reactive Forms
- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server compatible schema and connection configuration

The application includes:

- pre-seeded master data
- purchase bill transaction processing
- multi-line add/edit/delete
- real-time calculations
- save and edit purchase bills
- offline queue with sync support
- audit trail
- PDF export

## Implemented Requirements

### Task 1

- No login flow
- Seeded locations and items

### Task 2

- `GET /api/items`
- `GET /api/locations`

### Task 3

- item autocomplete
- batch dropdown from locations
- cost, price, quantity, discount
- live totals
- summary panel
- multi-line add, edit, delete

### Task 4

- `POST /api/purchase-bill`
- header + line persistence

### Task 5

- PDF export
- edit purchase bill
- offline save using `localStorage`
- sync handling with duplicate prevention using `offlineClientId`
- audit logging for create and update

### Task 6

Backend structure includes:

- `Controllers`
- `Services`
- `Repositories`
- `Entities`
- `DTOs`
- `Data`

Frontend structure includes:

- `core`
- `shared`
- `modules/purchase`
- `services`
- `models`

### Task 7

- ERP-style purchase layout
- header/details/summary tab treatment
- right-side summary panel
- loading and error states

### Task 8

- strong typing
- reusable services/utilities
- clean naming
- separated backend layers

## Backend Endpoints

- `GET /api/items`
- `GET /api/locations`
- `GET /api/purchase-bill`
- `GET /api/purchase-bill/{id}`
- `POST /api/purchase-bill`
- `PUT /api/purchase-bill/{id}`
- `GET /api/audit-logs`

## Database Script

Database script is included here:

- [DatabaseScript.sql](/c:/Users/MSII/Desktop/Enhanzer_Technical_Interview_AbbasMuneer/backend/DatabaseScript.sql)

It creates:

- `Locations`
- `Items`
- `PurchaseBillHeaders`
- `PurchaseBillLines`
- `Audit_Logs`

and seeds:

- `LOC001 - Warehouse A`
- `LOC002 - Warehouse B`
- `LOC003 - Main Store`
- `Mango, Apple, Banana, Orange, Grapes, Kiwi, Strawberry`

## SQL Server Configuration

Default backend connection string is set to SQL Server LocalDB in:

- [appsettings.json](/c:/Users/MSII/Desktop/Enhanzer_Technical_Interview_AbbasMuneer/backend/appsettings.json)

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EnhanzerPurchaseBillDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

If your machine uses SQL Server Express instead, replace it with:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=EnhanzerPurchaseBillDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

## Run Backend

```powershell
cd backend
dotnet restore
dotnet run
```

## Run Frontend

```powershell
cd frontend
npm install
npm start
```

Open:

- `http://localhost:4200`

## Notes

- The codebase still supports SQLite through the provider-selection logic, but the default submission configuration is now SQL Server aligned.
- Backend build passes.
- Backend tests pass.
- Frontend production build passes.

## Demo Flow

1. Start backend
2. Start frontend
3. Create a purchase bill
4. Save it
5. Load and edit a saved bill
6. Export PDF
7. Save one offline
8. Come back online and sync
9. Show audit logs
