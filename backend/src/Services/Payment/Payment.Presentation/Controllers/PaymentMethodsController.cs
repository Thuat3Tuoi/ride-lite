using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Abstractions.Messaging;
using Payment.Application.Features.PaymentMethod.Commands.ActivatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;
using Payment.Application.Features.PaymentMethod.Queries.GetPaymentMethodById;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("api/payment-methods")]
public sealed class PaymentMethodsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentMethodResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePaymentMethodResponse>> Create(
        [FromBody] CreatePaymentMethodRequest request,
        [FromServices] ICommandHandler<CreatePaymentMethodCommand, CreatePaymentMethodResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentMethodCommand(
            request.Code,
            request.Name,
            request.Active
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CreatePaymentMethodResponse(
            result.Id,
            result.Code,
            result.Name,
            result.Active
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentMethodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodResponse>> GetById(
        [FromRoute] long id,
        [FromServices] IQueryHandler<GetPaymentMethodByIdQuery, GetPaymentMethodByIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentMethodByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new PaymentMethodResponse(
            result.Id,
            result.Code,
            result.Name,
            result.Active
        );

        return Ok(response);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentMethodResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentMethodResponse>>> GetActive(
        [FromServices] IQueryHandler<GetActivePaymentMethodsQuery, GetActivePaymentMethodsResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetActivePaymentMethodsQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items
            .Select(m => new PaymentMethodResponse(
                m.Id,
                m.Code,
                m.Name,
                m.Active))
            .ToList();

        return Ok(response);
    }

    [HttpPut("{id:long}/activate")]
    [ProducesResponseType(typeof(PaymentMethodStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodStatusResponse>> Activate(
        [FromRoute] long id,
        [FromServices] ICommandHandler<ActivatePaymentMethodCommand, ActivatePaymentMethodResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new ActivatePaymentMethodCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new PaymentMethodStatusResponse(
            result.Id,
            result.Active
        );

        return Ok(response);
    }

    [HttpPut("{id:long}/deactivate")]
    [ProducesResponseType(typeof(PaymentMethodStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodStatusResponse>> Deactivate(
        [FromRoute] long id,
        [FromServices] ICommandHandler<DeactivatePaymentMethodCommand, DeactivatePaymentMethodResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePaymentMethodCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new PaymentMethodStatusResponse(
            result.Id,
            result.Active
        );

        return Ok(response);
    }
}
