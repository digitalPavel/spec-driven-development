using Booking.Core;

namespace Booking.Infrastructure;

public sealed class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _items = new();
    private readonly object _lock = new();

    public Task AddAsync(Appointment appointment, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _items.Add(appointment);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult((IReadOnlyList<Appointment>)_items.ToList());
        }
    }

    public Task<bool> ExistsOverlapAsync(DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
    {
        lock (_lock)
        {
            // Spec overlap rule:
            // overlap when newStart < existingEnd AND newEnd > existingStart
            var exists = _items.Any(a => startUtc < a.EndUtc && endUtc > a.StartUtc);
            return Task.FromResult(exists);
        }
    }
}