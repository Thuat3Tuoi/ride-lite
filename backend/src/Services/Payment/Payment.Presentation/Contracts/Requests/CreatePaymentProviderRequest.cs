using System.ComponentModel.DataAnnotations;
using Payment.Domain.Enumerators;

namespace Payment.Presentation.Contracts.Requests;

public sealed class CreatePaymentProviderRequest
{
    [Required(ErrorMessage = "Code is required.")]
    [StringLength(32, ErrorMessage = "Code cannot exceed 32 characters.")]
    public string Code { get; init; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "HandlerType is required.")]
    [EnumDataType(typeof(PaymentHandlerType), ErrorMessage = "Invalid HandlerType.")]
    public PaymentHandlerType HandlerType { get; init; }

    public bool Active { get; init; } = true;
}

