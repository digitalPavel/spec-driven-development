using Booking.Core;
using Booking.Infrastructure;
using FluentAssertions;

namespace Booking.Tests;

public class BookingServiceTests
{
    private static (BookingService service, InMemoryAppointmentRepository repo, FakeTimeProvider time) CreateSut(DateTime nowUtc)
    {
        var repo = new InMemoryAppointmentRepository();
        var time = new FakeTimeProvider { UtcNow = nowUtc };
        var service = new BookingService(repo, time);
        return (service, repo, time);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Appointment_When_Valid()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, repo, _) = CreateSut(now);

        var start = now.AddHours(2);
        var end = start.AddMinutes(30);

        var appt = await service.CreateAsync(start, end, "John");

        appt.Id.Should().NotBeEmpty();
        appt.Customer.Should().Be("John");
        appt.StartUtc.Should().Be(start);
        appt.EndUtc.Should().Be(end);

        var all = await repo.GetAllAsync();
        all.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Empty_Customer()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, _, _) = CreateSut(now);

        var start = now.AddHours(1);
        var end = start.AddMinutes(30);

        Func<Task> act = () => service.CreateAsync(start, end, "  ");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(BookingErrors.CustomerRequired);
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Invalid_Range_When_End_Before_Or_Equal_Start()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, _, _) = CreateSut(now);

        var start = now.AddHours(1);
        var end = start; // equal

        Func<Task> act = () => service.CreateAsync(start, end, "A");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(BookingErrors.InvalidRange);
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Too_Short_Duration()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, _, _) = CreateSut(now);

        var start = now.AddHours(1);
        var end = start.AddMinutes(10);

        Func<Task> act = () => service.CreateAsync(start, end, "A");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(BookingErrors.TooShort);
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Start_In_Past()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, _, _) = CreateSut(now);

        var start = now.AddMinutes(-1);
        var end = now.AddMinutes(30);

        Func<Task> act = () => service.CreateAsync(start, end, "A");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(BookingErrors.InPast);
    }

    [Fact]
    public async Task CreateAsync_Should_Reject_Overlap()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, _, _) = CreateSut(now);

        var start1 = now.AddHours(1);
        var end1 = start1.AddMinutes(30);

        await service.CreateAsync(start1, end1, "A");

        // overlaps: starts inside existing
        Func<Task> act = () => service.CreateAsync(
            start1.AddMinutes(10),
            end1.AddMinutes(10),
            "B");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(BookingErrors.Conflict);
    }

    [Fact]
    public async Task CreateAsync_Should_Allow_Boundary_Touching_End_Equals_Next_Start()
    {
        var now = new DateTime(2026, 02, 25, 12, 00, 00, DateTimeKind.Utc);
        var (service, repo, _) = CreateSut(now);

        var start1 = now.AddHours(1);
        var end1 = start1.AddMinutes(30);

        await service.CreateAsync(start1, end1, "A");

        // boundary touch: start == existing end => NOT overlap per spec
        var start2 = end1;
        var end2 = start2.AddMinutes(30);

        var appt2 = await service.CreateAsync(start2, end2, "B");

        appt2.Customer.Should().Be("B");
        (await repo.GetAllAsync()).Should().HaveCount(2);
    }
}