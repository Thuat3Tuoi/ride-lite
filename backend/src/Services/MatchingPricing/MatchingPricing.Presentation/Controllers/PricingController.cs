using MatchingPricing.Application.Abstractions.Messaging;
using MatchingPricing.Application.Features.Pricing.Queries.CalculatePrice;
using MatchingPricing.Application.Features.Pricing.Queries.EstimatePrice;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.Controllers;

[ApiController]
[Route("pricing")]
public sealed class PricingController : ControllerBase
{
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(CalculatePriceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CalculatePriceResponse>> Calculate(
        [FromBody] CalculatePriceRequest request,
        [FromServices] IQueryHandler<CalculatePriceQuery, CalculatePriceResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new CalculatePriceQuery(
            request.CostPerDistance,
            request.Distance,
            request.CostPerTime,
            request.EstimatedTime,
            request.AdditionalFee,
            request.TaxRate,
            request.PricingMethod
        );

        var result = await handler.HandleAsync(query, cancellationToken);

        return Ok(new CalculatePriceResponse(result.Price, result.PricingMethod));
    }

    [HttpPost("estimate")]
    [ProducesResponseType(typeof(EstimatePriceResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EstimatePriceResponse>> Estimate(
        [FromBody] EstimatePriceRequest request,
        [FromServices] IQueryHandler<EstimatePriceQuery, EstimatePriceResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new EstimatePriceQuery(
            request.PickupLatitude,
            request.PickupLongitude,
            request.DropoffLatitude,
            request.DropoffLongitude
        );

        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new EstimatePriceResponse(
            result.Fare,
            result.DistanceKm,
            result.Currency
        );

        return Ok(response);
    }
}
