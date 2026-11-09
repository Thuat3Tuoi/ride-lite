using MatchingPricing.Application.Features.Quote.Queries.GetQuote;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.Controllers;

// Hai route cung mot action de contract noi bo va Gateway khong bi lech nhau
[ApiController]
[Route("matching-pricing/quote")]
[Route("quote")]
public sealed class QuoteController(GetQuoteHandler getQuoteHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<QuoteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public ActionResult<QuoteResponse> Quote([FromBody] QuoteRequest request)
    {
        // Tach DTO HTTP khoi query de Application khong phu thuoc Presentation
        var query = new GetQuoteQuery(request.DistanceKm, request.DurationMinutes,
            request.PickupZone, request.PromotionCode);
        var result = getQuoteHandler.Handle(query);
        var response = new QuoteResponse(result.DistanceKm, result.DurationMinutes,
            result.DistanceFare, result.TimeFare, result.Subtotal, result.SurgeAmount,
            result.TollFee, result.PromotionDiscount, result.TaxAmount,
            result.RoundingAdjustment, result.TotalFare, result.Currency);
        return Ok(response);
    }
}
