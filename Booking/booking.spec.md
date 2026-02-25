# Booking Spec (Spec-Driven)

## Feature: Appointment booking

### Domain rules
1) Appointment has StartUtc and EndUtc (UTC).
2) EndUtc must be after StartUtc.
3) Minimum duration: 15 minutes.
4) Cannot book in the past: StartUtc < UtcNow is invalid.
5) No overlaps allowed:
   Overlap exists when: newStart < existingEnd AND newEnd > existingStart.

### Data model
Appointment:
- id: guid
- startUtc: datetime (UTC)
- endUtc: datetime (UTC)
- customer: string

### API
POST /appointments
Request JSON:
{
  "startUtc": "2026-02-24T18:00:00Z",
  "endUtc": "2026-02-24T18:30:00Z",
  "customer": "John"
}

Responses:
- 200 OK: returns created Appointment
- 400 BadRequest: validation errors (range, duration, past, empty customer)
- 409 Conflict: overlaps an existing appointment

GET /appointments
Responses:
- 200 OK: returns list of Appointment