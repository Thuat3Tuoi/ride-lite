using System.Text.Json.Serialization;
using Trip.Infrastructure;
using Trip.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Presentation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/trip/health", () => Results.Ok(new 
{
    status = "trip-service active"
}));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
