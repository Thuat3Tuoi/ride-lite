using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CreatePaymentLedgerEntryRequest
{
    [Required(ErrorMessage = "PaymentId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "PaymentId must be greater than 0.")]
    public long PaymentId { get; init; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; init; }

    [Required(ErrorMessage = "DebitAccount is required.")]
    [StringLength(50, ErrorMessage = "DebitAccount cannot exceed 50 characters.")]
    public string DebitAccount { get; init; } = string.Empty;

    [Required(ErrorMessage = "CreditAccount is required.")]
    [StringLength(50, ErrorMessage = "CreditAccount cannot exceed 50 characters.")]
    public string CreditAccount { get; init; } = string.Empty;
}

