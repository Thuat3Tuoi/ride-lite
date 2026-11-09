using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CreateRefundRequest
{
    [Required(ErrorMessage = "PaymentId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "PaymentId must be greater than 0.")]
    public long PaymentId { get; init; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; init; }

    [Required(ErrorMessage = "IdempotencyKey is required.")]
    [StringLength(128, ErrorMessage = "IdempotencyKey cannot exceed 128 characters.")]
    public string IdempotencyKey { get; init; } = string.Empty;

    [StringLength(255, ErrorMessage = "Reason cannot exceed 255 characters.")]
    public string? Reason { get; init; }
}

