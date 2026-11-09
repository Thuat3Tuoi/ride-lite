namespace MatchingPricing.Application;

// Phan biet loi du lieu cua nguoi goi voi loi noi bo can tra HTTP 500
public sealed class QuoteValidationException(string message) : Exception(message);
