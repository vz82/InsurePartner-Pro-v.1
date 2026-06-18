# Insurance Company Partner Management System

A comprehensive web application for managing insurance company partners and their policies.

## Features

✅ **Partner Management**
- Create, view, and manage insurance company partners
- Full validation of all partner data
- Unique partner numbers and external codes
- Support for personal and legal entities

✅ **Policy Tracking**
- Add policies with shelf numbers and amounts
- Track multiple policies per partner
- Real-time policy aggregation

✅ **Partner Highlighting**
- Automatically marks partners with >5 policies or >5,000 HRK total amount
- Visual indicators with asterisk (*) before partner names

✅ **Responsive UI**
- Built with Bootstrap 4
- Modal dialogs for detailed information
- Intuitive navigation and forms

## Technology Stack

- **Framework**: ASP.NET Core 8 MVC
- **ORM**: Dapper Micro ORM
- **Database**: PostgreSQL
- **Frontend**: HTML5, JavaScript, Bootstrap 4
- **Language**: C#

## Prerequisites

- .NET 8 SDK
- PostgreSQL 12+
- Visual Studio Code or Visual Studio 2022

## Installation

### 1. Database Setup

```bash
# Create database
psql -U postgres -c "CREATE DATABASE insurance_partners;"

# Run the initial schema script
psql -U postgres -d insurance_partners -f Database/001_InitialSchema.sql
```

### 2. Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=insurance_partners;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 3. Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The application will be available at `https://localhost:5001`

## Project Structure

```
├── Controllers/
│   └── PartnerController.cs       # Partner and Policy management
├── Models/
│   ├── Partner.cs                 # Partner domain model
│   └── Policy.cs                  # Policy domain model
├── Services/
│   ├── IPartnerService.cs         # Partner service interface
│   ├── PartnerService.cs          # Partner service implementation
│   ├── IPolicyService.cs          # Policy service interface
│   └── PolicyService.cs           # Policy service implementation
├── Data/
│   ├── DbConnectionFactory.cs     # Database connection factory
│   └── DbInitializer.cs           # Database initialization
├── Views/
│   ├── Partner/
│   │   ├── Index.cshtml           # Partner list page
│   │   └── Create.cshtml          # Create partner page
│   └── Shared/
│       └── _Layout.cshtml         # Master layout
├── Database/
│   └── 001_InitialSchema.sql      # Database schema
├── wwwroot/                       # Static files (CSS, JS, images)
├── Program.cs                     # Application startup
├── appsettings.json               # Configuration
└── README.md                      # This file
```

## Pages

### 1. Partner List (Home)
- Displays all partners sorted by creation date (newest first)
- Shows: FullName, PartnerNumber, PartnerType, IsForeign, Gender, PolicyCount, TotalAmount, CreatedAt
- Click on a row to view detailed partner information in a modal
- Add New Partner button navigates to creation form
- Add Policy button opens policy entry dialog
- Partners with >5 policies or >5,000 HRK amount marked with * in red

### 2. Create Partner
- Form with all required partner fields
- Client and server-side validation
- Email validation for CreatedByUser
- Partner Number must be exactly 20 digits
- External Code must be unique and 10-20 characters
- Upon successful creation, redirects to list with newly created partner highlighted

### 3. Partner Details (Modal)
- Displays all partner information
- Shows FullName (FirstName + LastName combined)
- Lists all associated policies
- Shows high-risk warning if applicable

## Validation Rules

### Partner Fields
- **FirstName**: 2-255 characters, alphanumeric, required
- **LastName**: 2-255 characters, alphanumeric, required
- **Address**: Alphanumeric, optional
- **PartnerNumber**: Exactly 20 digits, required, unique per partner type
- **CroatianPIN**: Optional
- **PartnerTypeId**: 1 (Personal) or 2 (Legal), required
- **CreatedAtUtc**: Automatically set to current UTC time
- **CreatedByUser**: Valid email address, max 255 characters, required
- **IsForeign**: Boolean, required
- **ExternalCode**: 10-20 alphanumeric characters, unique, required
- **Gender**: M, F, or N only, required

### Policy Fields
- **ShelfNumber**: 10-15 characters, required
- **PolicyAmount**: Decimal > 0, required

## High-Risk Criteria

A partner is marked as high-risk if:
- Number of policies > 5, OR
- Total policy amount > 5,000 HRK

High-risk partners are displayed with:
- Red asterisk (*) before their name in the list
- Warning alert in the details modal

## Database Schema

### Partners Table
```sql
CREATE TABLE partners (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    address VARCHAR(255),
    partner_number CHAR(20) NOT NULL,
    croatian_pin VARCHAR(50),
    partner_type_id INT NOT NULL CHECK (partner_type_id IN (1, 2)),
    created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'UTC',
    created_by_user VARCHAR(255) NOT NULL,
    is_foreign BOOLEAN NOT NULL,
    external_code VARCHAR(20) NOT NULL UNIQUE,
    gender CHAR(1) NOT NULL CHECK (gender IN ('M', 'F', 'N'))
);
```

### Policies Table
```sql
CREATE TABLE policies (
    id SERIAL PRIMARY KEY,
    partner_id INT NOT NULL REFERENCES partners(id) ON DELETE CASCADE,
    shelf_number VARCHAR(15) NOT NULL,
    policy_amount NUMERIC(18, 2) NOT NULL,
    created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'UTC'
);
```

## API Endpoints

### Partner Endpoints
- `GET /Partner/Index` - List all partners
- `GET /Partner/Create` - Show create form
- `POST /Partner/Create` - Create new partner
- `GET /Partner/GetPartnerDetails?id={id}` - Get partner details (AJAX)
- `POST /Partner/AddPolicy` - Add policy to partner (AJAX)

## Security Considerations

- CSRF protection enabled on all POST requests
- Email validation for CreatedByUser field
- Unique constraints on PartnerNumber and ExternalCode
- SQL injection prevention through Dapper parameterized queries
- Input validation on both client and server side

## Error Handling

- Comprehensive validation messages
- User-friendly error alerts
- Server-side exception handling
- Transaction integrity for policy additions

## Future Enhancements

- User authentication and authorization
- Audit logging for all changes
- Advanced filtering and search
- Export to CSV/Excel
- Policy renewal tracking
- Bulk operations
- Reporting dashboard

## License

This project is provided as-is for insurance partner management purposes.
