namespace Payment.Presentation.Contracts.Responses;

public sealed record ProcessRefundResponse(
    long RefundId,
    string Status,
    DateTimeOffset UpdatedAt
);

