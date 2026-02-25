namespace Booking.Core;

public sealed record Appointment(
    Guid Id,
    DateTime StartUtc,
    DateTime EndUtc,
    string Customer
);