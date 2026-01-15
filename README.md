
# Customer Campaign Service

Backend interview assignment that demonstrates a complete campaign and rewards flow with authentication, role-based authorization, CSV import, and external SOAP validation.

The focus is on realistic backend logic, clean API structure, and correct security boundaries.

---

## Prerequisites

Before running the application, make sure you have:

- .NET 8 SDK  
- SQL Server LocalDB (included with Visual Studio)
- Internet connection (used for SOAP customer validation)

---

## Configuration

Open the file:

CustomerCampaignService.Api/appsettings.json

Verify the following configuration (ConnectionString to your localdb):

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\Local;Database=CustomerCampaign;Trusted_Connection=True;"
}
}

Notes:
- Database schema is created automatically on startup via EF Core migrations

---

## Running the application

If startup succeeds, the API will be available at:

http://localhost:5183

Swagger UI:

http://localhost:5183/swagger

---

## Seeded users (authentication)

The application seeds users automatically on startup.

Agent user:
- Username: agent1
- Password: Agent123!

Admin user:
- Username: admin
- Password: Admin123!

---

## Login

POST /api/auth/login

Request body example:

{
  "username": "agent1",
  "password": "Agent123!"
}

The response contains an accessToken.

In Swagger:
- Click Authorize
- Paste the token value (without the Bearer prefix, Swagger adds it automatically)

---

## Roles and permissions

Agent:
- View campaigns
- Create rewards
- Cancel own rewards
- Correct own rewards

Admin:
- Import purchases via CSV
- View campaign results

Role restrictions are strictly enforced.

---

## Recommended evaluation flow

1. Login as Agent
2. Authorize in Swagger
3. List campaigns  
   GET /api/campaigns
4. Create rewards  
   POST /api/campaigns/{campaignCode}/rewards
5. Login as Admin
6. Import purchases CSV  
   POST /api/campaigns/{campaignCode}/purchases/import
7. View results  
   GET /api/campaigns/{campaignCode}/results

---

## Campaign and reward identifiers

- Campaigns use an integer code as the external identifier
- Seeded campaign:
  CampaignCode = 100
- Reward creation responses return a rewardCode used for cancel and correct operations

---

## CSV import format

Required columns (case-insensitive):

CustomerId,TransactionId,PurchaseDate,Amount

A sample file is provided at:

CustomerCampaignService.Api/SampleData/purchases-sample.csv

Invalid CSV format will cause the import to fail intentionally.

---

## SOAP customer validation

Customer existence is validated via an external SOAP service:

https://www.crcind.com/csp/samples/SOAP.Demo.cls

If the SOAP service is unavailable or the customer does not exist, reward creation and CSV import will fail with a 404 Not Found response.

---

## Database

- Entity Framework Core
- Migrations are applied automatically on startup
- Default LocalDB connection:

Server=(localdb)\Local;
Database=CustomerCampaign;
Trusted_Connection=True;
MultipleActiveResultSets=true

---

## Summary

This project demonstrates:
- Secure JWT authentication
- Clear role separation
- End-to-end campaign and reward flow
- External SOAP integration
- CSV file handling
- Clean and testable API design
=======


