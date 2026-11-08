using Payment.Domain.Enumerators;

namespace Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;

public sealed record PaymentProviderItemResult(
    long Id,
    string Code,
    string Name,
    PaymentHandlerType HandlerType,
    bool Active
);

public sealed record GetActivePaymentProvidersResult(
    IReadOnlyList<PaymentProviderItemResult> Items
);

