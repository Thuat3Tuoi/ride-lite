using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Payment.Application.Abstractions.Email;
using Payment.Domain.Entities;
using Payment.Domain.Enumerators;
using Payment.Infrastructure.Data.DataContext;

namespace Payment.Integration.Tests.Infrastructure;

public sealed class PaymentApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    public FakeEmailSender EmailSender { get; } = new();

    public PaymentApiFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext configurations
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ApplicationDbContext>();

            // Use SQLite in-memory
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection));

            // Replace SMTP with Fake
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(EmailSender);
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();

        // Seed reference data so foreign key constraints for Payment are satisfied
        if (!await db.PaymentMethods.AnyAsync())
        {
            var method = PaymentMethod.Create("CASH", "Cash Payment");
            var provider = PaymentProvider.Create("INTERNAL", "Internal System", PaymentHandlerType.Cod);
            await db.PaymentMethods.AddAsync(method);
            await db.PaymentProviders.AddAsync(provider);
            await db.SaveChangesAsync();

            var option = PaymentOption.Create(method.Id, provider.Id, "CASH_INTERNAL", "Standard Cash");
            await db.PaymentOptions.AddAsync(option);
            await db.SaveChangesAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}

