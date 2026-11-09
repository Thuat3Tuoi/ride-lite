namespace MatchingPricing.Application;

// Hop dong use case khong phu thuoc HTTP hay Trip Domain
public interface IQuoteService
{
    QuoteResponse Quote(QuoteRequest request);
}
