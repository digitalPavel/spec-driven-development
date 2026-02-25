# Booking API (Spec-Driven + Codegen + Tests)

A small appointment booking API built using a spec-driven workflow and code generation tools.

## What it does (workflow)

- Create an appointment (POST /appointments)
- View appointments (GET /appointments)
- Enforces domain rules:
  - EndUtc must be after StartUtc
  - Minimum duration is 15 minutes
  - Cannot book in the past
  - No overlapping appointments

Data is stored in-memory (no database required).

## Spec and codegen evidence

- Spec: `Booking/booking.spec.md`
- Codegen log: `Booking/docs/codegen-log.md`

## Tech

- .NET 10 Minimal API
- xUnit + FluentAssertions
- Swagger UI

## Run the API locally

From the repo root:

```bash
dotnet run --project Booking/Booking.Api
```

Open Swagger UI:

```
https://localhost:xxxx/swagger
```

(The exact port is printed in the console)

## Example requests

### Create appointment

POST /appointments

```json
{
  "startUtc": "2026-03-01T18:00:00Z",
  "endUtc": "2026-03-01T18:30:00Z",
  "customer": "John"
}
```

### List appointments

GET /appointments

## Run tests

From the repo root:

```bash
dotnet test
```

## Notes

- No secrets/credentials are used or required.
- Business rules are implemented in Booking.Core.
- Data storage is in-memory for simplicity.