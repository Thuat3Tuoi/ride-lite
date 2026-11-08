using Payment.Domain.Enumerators;

namespace Payment.Domain.Entities;

public sealed class PaymentProvider
{
    public long Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public PaymentHandlerType HandlerType { get; private set; }
    public bool Active { get; private set; }

    public IReadOnlyCollection<PaymentOption> Options => _options.AsReadOnly();
    private readonly List<PaymentOption> _options = [];

    private PaymentProvider() { }

    public static PaymentProvider Create(
        string code,
        string name,
        PaymentHandlerType handlerType,
        bool active = true)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Payment provider code cannot be empty.");

        code = code.Trim().ToUpperInvariant();

        if (code.Length > 32)
            throw new ArgumentException("Payment provider code cannot exceed 32 characters.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Payment provider name cannot be empty.");

        name = name.Trim();

        if (name.Length > 255)
            throw new ArgumentException("Payment provider name cannot exceed 255 characters.");

        return new PaymentProvider
        {
            Code = code,
            Name = name,
            HandlerType = handlerType,
            Active = active
        };
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
}
