using Microsoft.EntityFrameworkCore;
using Trip.Infrastructure;
using Trip.Infrastructure.Persistence;
using Trip.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("TripDb")
    ?? "Host=localhost;Port=5432;Database=ridelite_trip;Username=postgres;Password=postgres";

builder.Services.AddTripInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TripDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
