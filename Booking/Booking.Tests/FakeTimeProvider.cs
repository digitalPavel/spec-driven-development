using Booking.Core;

namespace Booking.Tests;

public sealed class FakeTimeProvider : ITimeProvider
{
    public DateTime UtcNow { get; set; }
}