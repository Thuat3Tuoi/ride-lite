using Trip.Infrastructure;
using Trip.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("TripDb")
    ?? throw new InvalidOperationException(
        "Connection string 'TripDb' is required to run the Trip service.");

builder.Services.AddTripInfrastructure(connectionString);

var app = builder.Build();

app.UseTripService();

app.Run();
