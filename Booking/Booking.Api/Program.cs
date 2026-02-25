using Booking.Api.Contracts;
using Booking.Core;
using Booking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI + Swagger UI
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();
builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
builder.Services.AddSingleton<BookingService>();

var app = builder.Build();

// Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// -------------------------
// Endpoints (per spec)
// -------------------------

app.MapPost("/appointments", async (
    CreateAppointmentRequest req,
    BookingService service,
    CancellationToken ct) =>
{
    try
    {
        var created = await service.CreateAsync(
            startUtc: req.StartUtc,
            endUtc: req.EndUtc,
            customer: req.Customer,
            ct: ct);

        return Results.Ok(created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex) when (ex.Message == BookingErrors.Conflict)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.WithName("CreateAppointment")
.WithOpenApi();

app.MapGet("/appointments", async (
    IAppointmentRepository repo,
    CancellationToken ct) =>
{
    var items = await repo.GetAllAsync(ct);
    return Results.Ok(items);
})
.WithName("GetAppointments")
.WithOpenApi();

app.Run();