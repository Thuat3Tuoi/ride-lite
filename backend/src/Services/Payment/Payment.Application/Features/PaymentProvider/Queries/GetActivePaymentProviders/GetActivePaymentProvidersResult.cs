namespace Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;

public sealed record GetActivePaymentProvidersResult(
    IReadOnlyList<PaymentProviderItemResult> Items
);
