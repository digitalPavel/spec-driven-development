# Codegen Log (Evidence)

## Tools used
- Visual Studio 2026
- GitHub Copilot (and/or ChatGPT GPT-5.2 / Codex)

## Workflow
1) Wrote the spec first: booking.spec.md
2) Used codegen to scaffold: domain model, repo interface, service, API endpoints, tests
3) Reviewed and refined generated code to match the spec exactly

## Prompts (copy/paste)
### P1 - Domain model + repo interface
"Generate C# code for Booking.Core:
- Appointment record (Id, StartUtc, EndUtc, Customer)
- IAppointmentRepository with AddAsync, GetAllAsync, ExistsOverlapAsync
based on booking.spec.md"

### P2 - Domain service
"Generate BookingService enforcing spec rules:
- end after start
- min 15 min
- not in past using injectable time provider
- no overlap using repository
Throw ArgumentException for validation and InvalidOperationException for conflict"

### P3 - In-memory repo
"Generate InMemoryAppointmentRepository implementing IAppointmentRepository with overlap check:
newStart < existingEnd && newEnd > existingStart
Use thread-safety."

### P4 - Minimal API endpoints
"Generate Minimal API endpoints in Booking.Api Program.cs:
POST /appointments and GET /appointments
Return 400 for validation, 409 for overlap, 200 for success.
Include Swagger."

### P5 - Tests from spec
"Generate xUnit tests using FluentAssertions:
- valid booking
- past booking rejected
- invalid range rejected
- too short rejected
- overlap rejected
- boundary touching allowed (end == next start)"