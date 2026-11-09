using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CreatePaymentOptionRequest
{
    [Required(ErrorMessage = "MethodId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "MethodId must be greater than 0.")]
    public long MethodId { get; init; }

    [Required(ErrorMessage = "ProviderId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "ProviderId must be greater than 0.")]
    public long ProviderId { get; init; }

    [Required(ErrorMessage = "Code is required.")]
    [StringLength(32, ErrorMessage = "Code cannot exceed 32 characters.")]
    public string Code { get; init; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
    public string Name { get; init; } = string.Empty;

    public bool Active { get; init; } = true;
}

