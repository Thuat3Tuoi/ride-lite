using Payment.Domain.Enumerators;

namespace Payment.Application.Features.PaymentProvider.Queries.GetPaymentProviderById;

public sealed record GetPaymentProviderByIdResult(
    long Id,
    string Code,
    string Name,
    PaymentHandlerType HandlerType,
    bool Active
);

