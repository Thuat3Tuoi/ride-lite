namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentOptionResponse(
    long Id,
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active
);

