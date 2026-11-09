namespace Payment.Presentation.Contracts.Responses;

public sealed record CreatePaymentOptionResponse(
    long Id,
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active
);

