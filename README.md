# Enhanzer Technical Interview Submission

## 1. Final Architecture Overview

This repository contains a full-stack assignment solution with:

- `backend/` ASP.NET Core 8 Web API
- `frontend/` Angular application
- SQL Server LocalDB compatible EF Core setup
- Login flow backed by the provided external POS API
- Purchase Bill page protected by a frontend route guard
- `Location_Details` persistence refreshed on every successful login

### High-level flow

1. User lands on `/login`
2. Angular submits `email` and `password` to `POST /api/auth/login`
3. Backend calls the external API using the required payload format
4. On successful response, backend clears and reinserts `User_Locations` into `Location_Details`
5. Frontend stores authenticated state in `localStorage`
6. User is routed to `/purchase-bill`
7. Purchase Bill page loads batches from `GET /api/locations`
8. User adds purchase bill rows in-memory and the summary updates live

### Backend layers

- `Controllers/` thin HTTP endpoints
- `Services/` login orchestration, external API integration, location persistence
- `Interfaces/` service contracts
- `Data/` EF Core DbContext and design-time factory
- `Models/Entities/` database entities
- `Models/DTOs/` request and response contracts
- `Helpers/` calculation helper
- `Extensions/` DI and middleware pipeline setup
- `Migrations/` initial migration for `Location_Details`
- `Tests/` xUnit + Moq tests

### Frontend layers

- `features/auth/` login UI and validation
- `features/purchase-bill/` protected bill-entry experience
- `services/` API communication
- `guards/` route protection
- `models/` frontend contracts
- `environments/` backend base URL configuration

## 2. Backend File Tree

```text
backend/
├── Controllers/
│   ├── AuthController.cs
│   └── LocationsController.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── ApplicationDbContextFactory.cs
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   └── WebApplicationExtensions.cs
├── Helpers/
│   └── PurchaseBillCalculationHelper.cs
├── Interfaces/
│   ├── IAuthService.cs
│   ├── IExternalPosApiService.cs
│   └── ILocationService.cs
├── Migrations/
│   ├── 202603190001_InitialCreate.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── DTOs/
│   │   ├── ErrorResponseDto.cs
│   │   ├── ExternalLoginRequestDto.cs
│   │   ├── ExternalLoginResponseDto.cs
│   │   ├── LocationDto.cs
│   │   ├── LoginRequestDto.cs
│   │   ├── LoginResponseDto.cs
│   │   └── UserLocationDto.cs
│   └── Entities/
│       └── LocationDetail.cs
├── Services/
│   ├── AuthService.cs
│   ├── ExternalPosApiService.cs
│   └── LocationService.cs
├── Tests/
│   ├── AuthServiceTests.cs
│   ├── Enhanzer.Api.Tests.csproj
│   ├── LocationServiceTests.cs
│   └── PurchaseBillCalculationHelperTests.cs
├── appsettings.Development.json
├── appsettings.json
├── Enhanzer.Api.csproj
└── Program.cs
```

## 3. Frontend File Tree

```text
frontend/
├── public/
│   └── .gitkeep
├── src/
│   ├── app/
│   │   ├── features/
│   │   │   ├── auth/
│   │   │   │   └── login/
│   │   │   │       ├── login.component.css
│   │   │   │       ├── login.component.html
│   │   │   │       ├── login.component.spec.ts
│   │   │   │       └── login.component.ts
│   │   │   └── purchase-bill/
│   │   │       ├── purchase-bill.component.css
│   │   │       ├── purchase-bill.component.html
│   │   │       ├── purchase-bill.component.spec.ts
│   │   │       └── purchase-bill.component.ts
│   │   ├── guards/
│   │   │   └── auth.guard.ts
│   │   ├── models/
│   │   │   ├── location.model.ts
│   │   │   ├── login-request.model.ts
│   │   │   ├── login-response.model.ts
│   │   │   └── purchase-bill-item.model.ts
│   │   ├── services/
│   │   │   ├── auth.service.ts
│   │   │   └── location.service.ts
│   │   ├── app.component.css
│   │   ├── app.component.html
│   │   ├── app.component.ts
│   │   └── app.routes.ts
│   ├── environments/
│   │   ├── environment.development.ts
│   │   └── environment.ts
│   ├── index.html
│   ├── main.ts
│   └── styles.css
├── angular.json
├── karma.conf.js
├── package.json
├── tsconfig.app.json
├── tsconfig.json
└── tsconfig.spec.json
```

## 4. Tech Stack

### Frontend

- Angular standalone components
- TypeScript
- Angular Router
- Angular Reactive Forms
- Angular HttpClient
- Karma + Jasmine

### Backend

- ASP.NET Core 8 Web API
- C#
- Entity Framework Core
- SQL Server / LocalDB
- Swagger
- CORS
- xUnit + Moq

## 5. Backend Setup

### Configuration

The backend uses:

- External API base URL: `https://ez-staging-api.azurewebsites.net/`
- Connection string: LocalDB in [backend/appsettings.json](/c:/Users/MSII/Desktop/Enhanzer_Technical_Interview_AbbasMuneer/backend/appsettings.json)
- Allowed CORS origin: `http://localhost:4200`

### Database

Table created by migration:

- `Location_Details`
  - `Id`
  - `Location_Code`
  - `Location_Name`
  - `CreatedAt`
  - `UpdatedAt`

### Important backend behavior

- Successful login clears previous `Location_Details`
- Fresh `User_Locations` are inserted after every successful login
- `GET /api/locations` returns persisted batches for the purchase bill page
- External API response parsing is defensive and searches recursively for `User_Locations`

## 6. Frontend Setup

### Screens implemented

- Login page matching the provided minimalist screenshot
- Purchase Bill page matching the enterprise-style layout screenshot

### Important frontend behavior

- Login uses inline validation and disabled submit on invalid state
- Login shows loading text while the request is in progress
- Login errors are surfaced from backend response
- Auth state is stored in `localStorage`
- `/purchase-bill` is protected using a route guard
- Batch dropdown values come from backend locations
- Item suggestions are filtered from the required fruit list
- Margin, total cost, and total selling update live and remain read-only
- Grid rows are stored in frontend memory

## 7. How Login Works

Frontend submits:

```json
{
  "email": "user@example.com",
  "password": "secret"
}
```

Backend transforms the request to:

```json
{
  "API_Action": "GetLoginData",
  "Device_Id": "D001",
  "Sync_Time": "",
  "Company_Code": "user@example.com",
  "API_Body": {
    "Username": "user@example.com",
    "Pw": "secret"
  }
}
```

Then:

1. External API is called
2. `User_Locations` are extracted
3. `Location_Details` is refreshed
4. Success response is returned to Angular
5. Angular stores auth state and routes to Purchase Bill

## 8. How Purchase Bill Works

### Input fields

- Item
- Batch
- Standard Cost
- Standard Price
- Margin
- Qty
- Free Qty
- Discount
- Total Cost
- Total Selling

### Calculations

- `margin = standardPrice - standardCost`
- `subtotalCost = standardCost * qty`
- `totalCost = subtotalCost - (subtotalCost * discount / 100)`
- `totalSelling = standardPrice * qty`

Example:

- Standard Cost = `100`
- Standard Price = `150`
- Qty = `5`
- Discount = `20`
- Total Cost = `400`
- Total Selling = `750`

### Summary panel

Includes:

- Total Items
- Total Qty
- Free Qty
- Gross Total
- Total Discount
- Total Selling
- Net Total
- Static Tax Summary section for screenshot fidelity

## 9. Assumptions

- Purchase bill rows are stored in frontend memory only
- Batch dropdown is populated from saved `Location_Details`
- Discount is percentage-based
- Purchase Bill route is protected on the frontend
- `User_Locations` are refreshed on each successful login
- Summary panel includes visual sections to match the screenshot even though purchase rows are not backend-persisted
- External API response shape may vary, so the parser uses defensive extraction

## 10. Exact Commands To Run Backend

From the repository root:

```powershell
cd backend
dotnet restore
dotnet build
dotnet run
```

Swagger will be available at the local URL printed by ASP.NET Core, typically:

```text
https://localhost:7037/swagger
```

## 11. Exact Commands To Run Frontend

From the repository root:

```powershell
cd frontend
npm install
npm start
```

Angular dev server:

```text
http://localhost:4200
```

## 12. Exact Migration Commands

From the repository root:

```powershell
cd backend
dotnet ef database update
```

If a new migration is ever needed:

```powershell
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## 13. Test Commands

### Backend tests

```powershell
cd backend\Tests
dotnet test
```

### Frontend tests

```powershell
cd frontend
npm test
```

## 14. Verification Status

Verified locally in this workspace:

- Backend build succeeded
- Backend tests passed: `4/4`
- Frontend production build succeeded
- Frontend tests executed successfully: `5/5`

Note:

- The Angular Karma process printed a ChromeHeadless teardown warning after reporting all tests as passed. The actual test suite completed successfully.

## 15. Demo Flow Steps

1. Start backend
2. Start frontend
3. Open `http://localhost:4200`
4. Attempt invalid login to show validation and backend error handling
5. Perform successful login
6. Navigate automatically to Purchase Bill
7. Show that batches are loaded from persisted locations
8. Add multiple rows with different items and discounts
9. Show live recalculation and updated summary totals
10. Refresh the page while authenticated to confirm route access remains protected by stored auth state

## 16. Final Checklist Mapping

### Task 1: Login Page

- Centered minimal card: implemented
- Large `Hi` heading: implemented
- Minimal underlined inputs with icons: implemented
- Password visibility icon: implemented
- Gradient full-width login button: implemented
- Required field validation: implemented
- Email format validation: implemented
- Inline validation messaging: implemented
- Loading state during login: implemented
- Backend error display: implemented
- External API payload mapping: implemented
- Successful login stores auth state: implemented
- Successful login persists locations: implemented
- Navigation blocked on failed login: implemented

### Task 2: Purchase Bill Page

- Protected route after login: implemented
- Blue top header bar: implemented
- Secondary tab row with `Details` active: implemented
- Left content panel and right summary panel: implemented
- Inner tab row with `Items` active: implemented
- Required form fields: implemented
- Item autocomplete list: implemented
- Batch dropdown from `Location_Details`: implemented
- Live margin/total calculations: implemented
- Read-only computed fields: implemented
- Add-row behavior: implemented
- Grid/table with required columns: implemented
- Summary totals update: implemented
- Extra financial summary values: implemented
- Responsive behavior while preserving desktop fidelity: implemented

### Backend Requirements

- ASP.NET Core Web API structure: implemented
- EF Core DbContext: implemented
- SQL Server LocalDB compatible config: implemented
- Swagger: implemented
- CORS: implemented
- Auth service layer: implemented
- External API service layer: implemented
- Location service layer: implemented
- `/api/auth/login`: implemented
- `/api/locations`: implemented
- Defensive external API parsing: implemented
- Migration for `Location_Details`: implemented
- Async/await and DI-based HttpClient: implemented

### Frontend Requirements

- Angular app structure: implemented
- Reactive forms: implemented
- Angular Router: implemented
- HttpClient services: implemented
- Route guard: implemented
- Environment config: implemented
- Component tests: implemented

### Testing Requirements

- AuthService success test: implemented
- AuthService failure test: implemented
- Location persistence test: implemented
- Calculation helper test: implemented
- Login validation test: implemented
- Purchase bill calculation test: implemented
- Add-row behavior test: implemented
- Summary totals test: implemented
