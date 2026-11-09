using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CompletePaymentRequest
{
    [Required(ErrorMessage = "ProviderTransactionId is required.")]
    [StringLength(128, ErrorMessage = "ProviderTransactionId cannot exceed 128 characters.")]
    public string ProviderTransactionId { get; init; } = string.Empty;

    [StringLength(255, ErrorMessage = "GatewayToken cannot exceed 255 characters.")]
    public string? GatewayToken { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "ResolvedProviderId must be greater than 0.")]
    public long? ResolvedProviderId { get; init; }
}

