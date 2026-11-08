namespace Trip.Domain.ValueObjects;

public readonly record struct Money
{
    public Money(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        if (decimal.Round(amount, 0) != amount)
        {
            throw new ArgumentException("VND amounts must not contain a fractional part.", nameof(amount));
        }

        Amount = amount;
    }

    public decimal Amount { get; }

    public string Currency => "VND";

    public override string ToString() => $"{Amount:0} {Currency}";
}
