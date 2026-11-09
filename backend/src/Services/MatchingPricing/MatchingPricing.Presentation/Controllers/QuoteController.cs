using MatchingPricing.Application;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.Controllers;

// Hai route cung mot action de contract noi bo va Gateway khong bi lech nhau
[ApiController]
[Route("matching-pricing/quote")]
[Route("quote")]
public sealed class QuoteController(IQuoteService quoteService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<QuoteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public ActionResult<QuoteResponse> Quote([FromBody] QuoteRequest request)
        => Ok(quoteService.Quote(request));
}
