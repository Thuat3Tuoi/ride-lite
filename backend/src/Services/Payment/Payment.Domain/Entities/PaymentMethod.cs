namespace Payment.Domain.Entities;

public sealed class PaymentMethod
{
    public long Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool Active { get; private set; }

    public IReadOnlyCollection<PaymentOption> Options => _options.AsReadOnly();
    private readonly List<PaymentOption> _options = [];

    private PaymentMethod() { }

    public static PaymentMethod Create(string code, string name, bool active = true)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Payment method code cannot be empty.");

        code = code.Trim().ToUpperInvariant();

        if (code.Length > 32)
            throw new ArgumentException("Payment method code cannot exceed 32 characters.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Payment method name cannot be empty.");

        name = name.Trim();

        if (name.Length > 255)
            throw new ArgumentException("Payment method name cannot exceed 255 characters.");

        return new PaymentMethod
        {
            Code = code,
            Name = name,
            Active = active
        };
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
}
