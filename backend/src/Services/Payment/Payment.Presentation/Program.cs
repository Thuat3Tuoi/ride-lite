using System.Text.Json.Serialization;
using Payment.Infrastructure;
using Payment.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Dang ky controller va xu ly loi HTTP cua service
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Dang ky cac phu thuoc ha tang cua module
builder.Services.AddInfrastructure(builder.Configuration);

// Tai su dung OpenAPI lam nguon tai lieu cho Swagger UI
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/payment/health", () => Results.Ok(new 
{
    status = "payment-service active"
}));

// Chi mo tai lieu va giao dien thu API trong moi truong phat trien
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Payment API v1");
        options.DocumentTitle = "RideLite - Payment API";
        options.DisplayRequestDuration();
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
