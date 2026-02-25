namespace Booking.Core;

public interface IAppointmentRepository
{
    Task AddAsync(Appointment appointment, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsOverlapAsync(DateTime startUtc, DateTime endUtc, CancellationToken ct = default);
}