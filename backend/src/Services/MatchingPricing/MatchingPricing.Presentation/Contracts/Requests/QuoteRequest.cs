namespace MatchingPricing.Presentation.Contracts.Requests;

// Client chi chon ma chinh sach da cong bo, khong duoc truyen don gia hay he so
public sealed record QuoteRequest(decimal? DistanceKm = null, decimal? DurationMinutes = null,
    string PickupZone = "default", string? PromotionCode = null);
