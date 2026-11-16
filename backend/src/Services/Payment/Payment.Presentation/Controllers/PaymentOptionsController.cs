using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Abstractions.Messaging;
using Payment.Application.Features.PaymentOption.Commands.ActivatePaymentOption;
using Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;
using Payment.Application.Features.PaymentOption.Commands.DeactivatePaymentOption;
using Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;
using Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionById;
using Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("payment-options")]
public sealed class PaymentOptionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentOptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePaymentOptionResponse>> Create(
        [FromBody] CreatePaymentOptionRequest request,
        [FromServices] ICommandHandler<CreatePaymentOptionCommand, CreatePaymentOptionResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentOptionCommand(
            request.MethodId,
            request.ProviderId,
            request.Code,
            request.Name,
            request.Active
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CreatePaymentOptionResponse(
            result.Id,
            result.MethodId,
            result.ProviderId,
            result.Code,
            result.Name,
            result.Active
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentOptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentOptionResponse>> GetById(
        [FromRoute] long id,
        [FromServices] IQueryHandler<GetPaymentOptionByIdQuery, GetPaymentOptionByIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentOptionByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new PaymentOptionResponse(
            result.Id,
            result.MethodId,
            result.ProviderId,
            result.Code,
            result.Name,
            result.Active
        );

        return Ok(response);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentOptionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentOptionResponse>>> GetActive(
        [FromServices] IQueryHandler<GetActivePaymentOptionsQuery, GetActivePaymentOptionsResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetActivePaymentOptionsQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items
            .Select(o => new PaymentOptionResponse(
                o.Id,
                o.MethodId,
                o.ProviderId,
                o.Code,
                o.Name,
                o.Active))
            .ToList();

        return Ok(response);
    }

    [HttpGet("method/{methodId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentOptionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentOptionResponse>>> GetByMethodId(
        [FromRoute] long methodId,
        [FromServices] IQueryHandler<GetPaymentOptionsByMethodIdQuery, GetPaymentOptionsByMethodIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentOptionsByMethodIdQuery(methodId);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items
            .Select(o => new PaymentOptionResponse(
                o.Id,
                o.MethodId,
                o.ProviderId,
                o.Code,
                o.Name,
                o.Active))
            .ToList();

        return Ok(response);
    }

    [HttpPut("{id:long}/activate")]
    [ProducesResponseType(typeof(PaymentOptionStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentOptionStatusResponse>> Activate(
        [FromRoute] long id,
        [FromServices] ICommandHandler<ActivatePaymentOptionCommand, ActivatePaymentOptionResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new ActivatePaymentOptionCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new PaymentOptionStatusResponse(
            result.Id,
            result.Active
        );

        return Ok(response);
    }

    [HttpPut("{id:long}/deactivate")]
    [ProducesResponseType(typeof(PaymentOptionStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentOptionStatusResponse>> Deactivate(
        [FromRoute] long id,
        [FromServices] ICommandHandler<DeactivatePaymentOptionCommand, DeactivatePaymentOptionResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePaymentOptionCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new PaymentOptionStatusResponse(
            result.Id,
            result.Active
        );

        return Ok(response);
    }
}
