namespace Booking.Api.Contracts;

public sealed record CreateAppointmentRequest(
    DateTime StartUtc,
    DateTime EndUtc,
    string Customer
);