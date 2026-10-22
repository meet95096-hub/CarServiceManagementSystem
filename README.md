# ApexAuto Dynamics - Car Service Management System

A high-performance **Car Service Management System** built with **ASP.NET Core (C#)**, **Entity Framework Core**, and **Microsoft SQL Server** (with seamless zero-configuration SQLite dev fallback).

---

## 🌟 Key Functional Modules

### 1. 🚗 Vehicle Registration & Technical Dossier
- **Fleet Directory**: Search and filter vehicles by license plate, VIN, make, model, fuel type, and owner contact.
- **Detailed Profile**: Displays complete vehicle specs, current odometer, cumulative service spend, historical repair cards, and past invoices.
- **Full CRUD Support**: Add new vehicles, update specs, or remove entries.

### 2. 📅 Service Appointment Booking & Dispatch
- **Smart Scheduling**: Select registered vehicle, service package, preferred time slot, and assigned hoist bay.
- **Status Workflow**: Manage appointment progression (`Scheduled` → `Confirmed` → `In Service` → `Completed` → `Cancelled`).
- **1-Click Conversion**: Convert any confirmed appointment directly into an active workshop **Repair Job Card** with pre-populated labor items and diagnostic notes.

### 3. 🔧 Live Repair Tracker & Job Cards
- **7-Stage Visual Stepper**:
  1. `Pending Check-In` (Walkaround & mileage logging)
  2. `Inspection & Diagnostic` (OBD-II error codes & visual checks)
  3. `In Progress` (Active mechanical labor)
  4. `Waiting for Parts` (Holding for OEM delivery)
  5. `Quality Check` (Road test & torque audit)
  6. `Ready for Delivery` (Final wash & polish)
  7. `Delivered & Closed`
- **Multi-Point Digital Inspection Checklist**: Interactive inspection switches for engine oil, brake pads, battery load, tire tread, coolant pressure, suspension, and lights.
- **Dynamic Labor & Parts Requisition**: Add and remove mechanical labor tasks (hours & hourly rate) and genuine OEM spare parts with live price recalculation.
- **Customer Live Telemetry Portal** (`/RepairTracking/Live`): Public-facing tracking page where vehicle owners can check their repair status, technician remarks, and estimated pickup time using their license plate.

### 4. 💳 Invoicing, Billing & Payment Settlement
- **Automated Bill Generation**: Automatically computes labor subtotal, parts subtotal, promotional discount %, and state/federal taxes.
- **Payment Processing**: Record partial or full payments via Credit Card, Debit Card, Cash, UPI / Instant Bank Transfer, or Fleet Accounts.
- **Print / PDF Ready**: Dedicated print stylesheet formatted for paper and PDF export featuring corporate headers, line items, and signature lines.

### 5. 🧑‍🔧 Workshop Hoists & Master Technicians
- Real-time monitoring of workshop bays (Bay 1 to 4) with technician status and current hoist occupancy.
- Standardized service packages catalog with estimated hours and labor rates.

---

## 🗄️ Database & SQL Server Configuration

The application is engineered with **Dual-Provider Architecture**:
1. **Microsoft SQL Server**: Production-ready support via `Microsoft.EntityFrameworkCore.SqlServer`.
2. **Zero-Setup SQLite Fallback**: If a local SQL Server instance is not currently active, the application automatically boots into SQLite (`CarService.db`) so developers can test immediately without any database setup.

### Switching to SQL Server:
1. Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=CarServiceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;",
    "SqliteConnection": "Data Source=CarService.db"
  },
  "DatabaseProvider": "SqlServer"
}
```
2. You can also run the provided **`CarServiceDb_SQLServer.sql`** script directly in **SQL Server Management Studio (SSMS)** or **Azure Data Studio** to inspect or execute the full T-SQL DDL schema.

---

## 🚀 Running the Project Locally

```bash
# Clone or navigate to the project directory
cd web.net

# Build the application
dotnet build

# Run the web server
dotnet run --launch-profile http
```

Once started, open your browser to:
**`http://localhost:5243`**
"# CarServiceManagementSystem" 
"# CarServiceManagementSystem" 
"# CarServiceManagementSystem" 
