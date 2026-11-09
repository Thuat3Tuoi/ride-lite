using Microsoft.EntityFrameworkCore;
using Trip.Infrastructure.Data.DataContext;
using Trip.Presentation.Middlewares;

namespace Trip.Presentation.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseTripService(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TripDbContext>();
            dbContext.Database.Migrate();
        }

        app.MapGet("/trip/health", () => "trip-service active");
        app.MapControllers();

        return app;
    }
}
