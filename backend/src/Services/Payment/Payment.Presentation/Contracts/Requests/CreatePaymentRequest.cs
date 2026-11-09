using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CreatePaymentRequest
{
    [Required(ErrorMessage = "TripId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "TripId must be greater than 0.")]
    public long TripId { get; init; }

    [Required(ErrorMessage = "PaymentOptionId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "PaymentOptionId must be greater than 0.")]
    public long PaymentOptionId { get; init; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; init; }

    [StringLength(128, ErrorMessage = "IdempotencyKey cannot exceed 128 characters.")]
    public string? IdempotencyKey { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "ProviderId must be greater than 0.")]
    public long? ProviderId { get; init; }
}

