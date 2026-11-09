var builder = WebApplication.CreateBuilder(args);

// Dang ky controller va tai lieu API cua service

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapGet("/trip/health", () => "trip-service active");

// Chi mo tai lieu va giao dien thu API trong moi truong phat trien
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Trip API v1");
        options.DocumentTitle = "RideLite - Trip API";
        options.DisplayRequestDuration();
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
