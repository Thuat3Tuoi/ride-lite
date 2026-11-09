using Payment.Domain.Enumerators;

namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentProviderResponse(
    long Id,
    string Code,
    string Name,
    PaymentHandlerType HandlerType,
    bool Active
);

