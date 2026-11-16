using System.Text.Json.Serialization;
using Payment.Infrastructure;
using Payment.Presentation.Conventions;
using Payment.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

var routePrefix = builder.Configuration["RoutePrefix"] ?? "api/payment";

// Presentation
builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new GlobalRoutePrefixConvention(routePrefix));
    })
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

app.MapGet("/health", () => Results.Ok(new 
{
    status = "payment-service active"
}));

app.MapGet("/api/payment/health", () => Results.Ok(new 
{
    status = "payment-service active"
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

