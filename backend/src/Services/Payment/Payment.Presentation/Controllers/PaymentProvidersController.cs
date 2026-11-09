using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.PaymentProvider.Commands.ActivatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Commands.CreatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Commands.DeactivatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;
using Payment.Application.Features.PaymentProvider.Queries.GetPaymentProviderById;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("api/payment-providers")]
public sealed class PaymentProvidersController : ControllerBase
{
    private readonly CreatePaymentProviderHandler _createPaymentProviderHandler;
    private readonly ActivatePaymentProviderHandler _activatePaymentProviderHandler;
    private readonly DeactivatePaymentProviderHandler _deactivatePaymentProviderHandler;
    private readonly GetPaymentProviderByIdHandler _getPaymentProviderByIdHandler;
    private readonly GetActivePaymentProvidersHandler _getActivePaymentProvidersHandler;

    public PaymentProvidersController(
        CreatePaymentProviderHandler createPaymentProviderHandler,
        ActivatePaymentProviderHandler activatePaymentProviderHandler,
        DeactivatePaymentProviderHandler deactivatePaymentProviderHandler,
        GetPaymentProviderByIdHandler getPaymentProviderByIdHandler,
        GetActivePaymentProvidersHandler getActivePaymentProvidersHandler)
    {
        _createPaymentProviderHandler = createPaymentProviderHandler;
        _activatePaymentProviderHandler = activatePaymentProviderHandler;
        _deactivatePaymentProviderHandler = deactivatePaymentProviderHandler;
        _getPaymentProviderByIdHandler = getPaymentProviderByIdHandler;
        _getActivePaymentProvidersHandler = getActivePaymentProvidersHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentProviderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePaymentProviderResponse>> Create(
        [FromBody] CreatePaymentProviderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentProviderCommand(
            request.Code,
            request.Name,
            request.HandlerType,
            request.Active
        );

        var result = await _createPaymentProviderHandler.HandleAsync(command, cancellationToken);

        var response = new CreatePaymentProviderResponse(
            result.Id,
            result.Code,
            result.Name,
            result.HandlerType,
            result.Active
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentProviderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentProviderResponse>> GetById(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentProviderByIdQuery(id);
        var result = await _getPaymentProviderByIdHandler.HandleAsync(query, cancellationToken);

        var response = new PaymentProviderResponse(
            result.Id,
            result.Code,
            result.Name,
            result.HandlerType,
            result.Active
        );

        return Ok(response);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentProviderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentProviderResponse>>> GetActive(
        CancellationToken cancellationToken)
    {
        var query = new GetActivePaymentProvidersQuery();
        var result = await _getActivePaymentProvidersHandler.HandleAsync(query, cancellationToken);

        var response = result.Items
            .Select(p => new PaymentProviderResponse(
                p.Id,
                p.Code,
                p.Name,
                p.HandlerType,
                p.Active))
            .ToList();

        return Ok(response);
    }

    [HttpPut("{id:long}/activate")]
    [ProducesResponseType(typeof(PaymentProviderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentProviderStatusResponse>> Activate(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var command = new ActivatePaymentProviderCommand(id);
        var result = await _activatePaymentProviderHandler.HandleAsync(command, cancellationToken);

        var response = new PaymentProviderStatusResponse(
            result.Id,
            result.Active
        );

        return Ok(response);
    }

    [HttpPut("{id:long}/deactivate")]
    [ProducesResponseType(typeof(PaymentProviderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentProviderStatusResponse>> Deactivate(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePaymentProviderCommand(id);
        var result = await _deactivatePaymentProviderHandler.HandleAsync(command, cancellationToken);

        var response = new PaymentProviderStatusResponse(
            result.Id,
            result.Active
        );

        return Ok(response);
    }
}

