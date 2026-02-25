namespace Booking.Core;

public static class BookingErrors
{
    public const string CustomerRequired = "Customer is required.";
    public const string InvalidRange = "EndUtc must be after StartUtc.";
    public const string TooShort = "Minimum duration is 15 minutes.";
    public const string InPast = "Cannot book an appointment in the past.";
    public const string Conflict = "Appointment time conflicts with an existing booking.";
}