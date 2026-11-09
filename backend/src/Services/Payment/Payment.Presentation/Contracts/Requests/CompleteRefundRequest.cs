using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CompleteRefundRequest
{
    [Required(ErrorMessage = "ProviderRefundId is required.")]
    [StringLength(128, ErrorMessage = "ProviderRefundId cannot exceed 128 characters.")]
    public string ProviderRefundId { get; init; } = string.Empty;
}

