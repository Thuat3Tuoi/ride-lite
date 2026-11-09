namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentProviderStatusResponse(
    long Id,
    bool Active
);

