using Trip.Domain.Shared;

namespace Trip.Domain.ValueObjects;

public sealed class Money : ValueObject
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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0} {Currency}";
}
