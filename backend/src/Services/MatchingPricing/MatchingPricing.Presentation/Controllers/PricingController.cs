using MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.Controllers;

[ApiController]
[Route("api/pricing")]
public sealed class PricingController : ControllerBase
{
    private readonly EstimatePriceHandler _estimatePriceHandler;

    public PricingController(EstimatePriceHandler estimatePriceHandler)
    {
        _estimatePriceHandler = estimatePriceHandler;
    }

    [HttpPost("estimate")]
    [ProducesResponseType(typeof(EstimatePriceResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EstimatePriceResponse>> Estimate(
        [FromBody] EstimatePriceRequest request,
        CancellationToken cancellationToken)
    {
        var query = new EstimatePriceQuery(
            request.PickupLatitude,
            request.PickupLongitude,
            request.DropoffLatitude,
            request.DropoffLongitude
        );

        var result = await _estimatePriceHandler.HandleAsync(query, cancellationToken);

        var response = new EstimatePriceResponse(
            result.Fare,
            result.DistanceKm,
            result.Currency
        );

        return Ok(response);
    }
}

