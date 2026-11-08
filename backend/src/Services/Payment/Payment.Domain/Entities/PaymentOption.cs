namespace Payment.Domain.Entities;

public sealed class PaymentOption
{
    public long Id { get; private set; }
    public long MethodId { get; private set; }
    public long ProviderId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool Active { get; private set; }

    public PaymentMethod? Method { get; private set; }
    public PaymentProvider? Provider { get; private set; }

    private PaymentOption() { }

    public static PaymentOption Create(
        long methodId,
        long providerId,
        string code,
        string name,
        bool active = true)
    {
        if (methodId <= 0)
            throw new ArgumentException("MethodId must be a positive value.");

        if (providerId <= 0)
            throw new ArgumentException("ProviderId must be a positive value.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Payment option code cannot be empty.");

        code = code.Trim().ToUpperInvariant();

        if (code.Length > 32)
            throw new ArgumentException("Payment option code cannot exceed 32 characters.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Payment option name cannot be empty.");

        name = name.Trim();

        if (name.Length > 255)
            throw new ArgumentException("Payment option name cannot exceed 255 characters.");

        return new PaymentOption
        {
            MethodId = methodId,
            ProviderId = providerId,
            Code = code,
            Name = name,
            Active = active
        };
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
}
