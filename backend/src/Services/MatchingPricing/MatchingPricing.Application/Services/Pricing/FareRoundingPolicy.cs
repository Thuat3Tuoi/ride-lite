namespace MatchingPricing.Application.Services.Pricing;

// Chi lam tron tong den VND nguyen tai buoc cuoi, moc nua dong duoc lam tron len
public sealed class FareRoundingPolicy
{
    public decimal Round(decimal amount) => decimal.Round(amount, 0, MidpointRounding.AwayFromZero);
}
