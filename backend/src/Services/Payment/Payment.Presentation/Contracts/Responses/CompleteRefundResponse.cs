namespace Payment.Presentation.Contracts.Responses;

public sealed record CompleteRefundResponse(
    long RefundId,
    string Status,
    string ProviderRefundId,
    DateTimeOffset? RefundedAt,
    DateTimeOffset UpdatedAt
);

