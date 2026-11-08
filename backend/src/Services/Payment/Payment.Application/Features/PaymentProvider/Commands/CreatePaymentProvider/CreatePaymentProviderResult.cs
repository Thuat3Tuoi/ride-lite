using Payment.Domain.Enumerators;

namespace Payment.Application.Features.PaymentProvider.Commands.CreatePaymentProvider;

public sealed record CreatePaymentProviderResult(
    long Id,
    string Code,
    string Name,
    PaymentHandlerType HandlerType,
    bool Active
);

