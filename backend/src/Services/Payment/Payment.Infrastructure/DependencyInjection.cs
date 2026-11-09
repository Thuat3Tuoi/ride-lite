using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Abstractions.Data;
using Payment.Application.Abstractions.Email;
using Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;
using Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;
using Payment.Application.Features.Payment.Commands.CancelPayment;
using Payment.Application.Features.Payment.Commands.CompletePayment;
using Payment.Application.Features.Payment.Commands.CreatePayment;
using Payment.Application.Features.Payment.Commands.ProcessPayment;
using Payment.Application.Features.Payment.Queries.GetPaymentById;
using Payment.Application.Features.Payment.Queries.GetPaymentByTripId;
using Payment.Application.Features.PaymentMethod.Commands.ActivatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;
using Payment.Application.Features.PaymentMethod.Queries.GetPaymentMethodById;
using Payment.Application.Features.PaymentOption.Commands.ActivatePaymentOption;
using Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;
using Payment.Application.Features.PaymentOption.Commands.DeactivatePaymentOption;
using Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;
using Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionById;
using Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;
using Payment.Application.Features.PaymentProvider.Commands.ActivatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Commands.CreatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Commands.DeactivatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;
using Payment.Application.Features.PaymentProvider.Queries.GetPaymentProviderById;
using Payment.Application.Features.Refund.Commands.CompleteRefund;
using Payment.Application.Features.Refund.Commands.CreateRefund;
using Payment.Application.Features.Refund.Commands.ProcessRefund;
using Payment.Application.Features.Refund.Queries.GetRefundById;
using Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;
using Payment.Infrastructure.Data.DataContext;
using Payment.Infrastructure.Data.Repositories;
using Payment.Infrastructure.Services.Email;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Database connection string is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IPaymentOptionRepository, PaymentOptionRepository>();
        services.AddScoped<IPaymentProviderRepository, PaymentProviderRepository>();
        services.AddScoped<ILedgerEntryRepository, LedgerEntryRepository>();

        // Email configuration
        services.Configure<SmtpOptions>(
            configuration.GetSection(
                SmtpOptions.SectionName
            )
        );

        // Email implementation
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        // Command & Query Handlers
        services.AddPaymentHandlers();

        return services;
    }

    private static IServiceCollection AddPaymentHandlers(this IServiceCollection services)
    {
        // Payment Handlers
        services.AddScoped<CreatePaymentHandler>();
        services.AddScoped<ProcessPaymentHandler>();
        services.AddScoped<CompletePaymentHandler>();
        services.AddScoped<CancelPaymentHandler>();
        services.AddScoped<GetPaymentByIdHandler>();
        services.AddScoped<GetPaymentByTripIdHandler>();

        // Refund Handlers
        services.AddScoped<CreateRefundHandler>();
        services.AddScoped<ProcessRefundHandler>();
        services.AddScoped<CompleteRefundHandler>();
        services.AddScoped<GetRefundByIdHandler>();
        services.AddScoped<GetRefundsByPaymentIdHandler>();

        // PaymentMethod Handlers
        services.AddScoped<CreatePaymentMethodHandler>();
        services.AddScoped<ActivatePaymentMethodHandler>();
        services.AddScoped<DeactivatePaymentMethodHandler>();
        services.AddScoped<GetPaymentMethodByIdHandler>();
        services.AddScoped<GetActivePaymentMethodsHandler>();

        // PaymentOption Handlers
        services.AddScoped<CreatePaymentOptionHandler>();
        services.AddScoped<ActivatePaymentOptionHandler>();
        services.AddScoped<DeactivatePaymentOptionHandler>();
        services.AddScoped<GetPaymentOptionByIdHandler>();
        services.AddScoped<GetActivePaymentOptionsHandler>();
        services.AddScoped<GetPaymentOptionsByMethodIdHandler>();

        // PaymentProvider Handlers
        services.AddScoped<CreatePaymentProviderHandler>();
        services.AddScoped<ActivatePaymentProviderHandler>();
        services.AddScoped<DeactivatePaymentProviderHandler>();
        services.AddScoped<GetPaymentProviderByIdHandler>();
        services.AddScoped<GetActivePaymentProvidersHandler>();

        // LedgerEntry Handlers
        services.AddScoped<CreatePaymentLedgerEntryHandler>();
        services.AddScoped<CreateRefundLedgerEntryHandler>();
        services.AddScoped<GetLedgerEntriesByPaymentIdHandler>();
        services.AddScoped<GetLedgerEntriesByRefundIdHandler>();

        return services;
    }
}
