namespace Booking.Core;

public sealed class BookingService
{
    private readonly IAppointmentRepository _repo;
    private readonly ITimeProvider _time;

    public BookingService(IAppointmentRepository repo, ITimeProvider time)
    {
        _repo = repo;
        _time = time;
    }

    public async Task<Appointment> CreateAsync(
        DateTime startUtc,
        DateTime endUtc,
        string customer,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customer))
            throw new ArgumentException(BookingErrors.CustomerRequired);

        // (Spec) End must be after Start
        if (endUtc <= startUtc)
            throw new ArgumentException(BookingErrors.InvalidRange);

        // (Spec) Min duration 15 minutes
        if (endUtc - startUtc < TimeSpan.FromMinutes(15))
            throw new ArgumentException(BookingErrors.TooShort);

        // (Spec) Cannot book in the past
        if (startUtc < _time.UtcNow)
            throw new ArgumentException(BookingErrors.InPast);

        // (Spec) No overlap allowed
        var conflict = await _repo.ExistsOverlapAsync(startUtc, endUtc, ct);
        if (conflict)
            throw new InvalidOperationException(BookingErrors.Conflict);

        var appointment = new Appointment(
            Id: Guid.NewGuid(),
            StartUtc: startUtc,
            EndUtc: endUtc,
            Customer: customer.Trim()
        );

        await _repo.AddAsync(appointment, ct);
        return appointment;
    }
}