using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CancelPaymentRequest
{
    [StringLength(255, ErrorMessage = "Reason cannot exceed 255 characters.")]
    public string? Reason { get; init; }
}

