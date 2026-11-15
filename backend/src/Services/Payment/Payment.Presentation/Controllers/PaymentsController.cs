using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Abstractions.Messaging;
using Payment.Application.Features.Payment.Commands.CancelPayment;
using Payment.Application.Features.Payment.Commands.CompletePayment;
using Payment.Application.Features.Payment.Commands.CreatePayment;
using Payment.Application.Features.Payment.Commands.ProcessPayment;
using Payment.Application.Features.Payment.Queries.GetPaymentById;
using Payment.Application.Features.Payment.Queries.GetPaymentByTripId;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePaymentResponse>> Create(
        [FromBody] CreatePaymentRequest request,
        [FromServices] ICommandHandler<CreatePaymentCommand, CreatePaymentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(
            request.TripId,
            request.PaymentOptionId,
            request.Amount,
            request.IdempotencyKey,
            request.ProviderId
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CreatePaymentResponse(
            result.Id,
            result.TripId,
            result.PaymentOptionId,
            result.Amount,
            result.Status,
            result.IdempotencyKey,
            result.CreatedAt
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> GetById(
        [FromRoute] long id,
        [FromServices] IQueryHandler<GetPaymentByIdQuery, GetPaymentByIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new PaymentResponse(
            result.Id,
            result.TripId,
            result.PaymentOptionId,
            result.ProviderId,
            result.ProviderTransactionId,
            result.GatewayToken,
            result.Amount,
            result.IdempotencyKey,
            result.Status,
            result.PaidAt,
            result.CreatedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpGet("trip/{tripId:long}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> GetByTripId(
        [FromRoute] long tripId,
        [FromServices] IQueryHandler<GetPaymentByTripIdQuery, GetPaymentByTripIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByTripIdQuery(tripId);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = new PaymentResponse(
            result.Id,
            result.TripId,
            result.PaymentOptionId,
            result.ProviderId,
            result.ProviderTransactionId,
            result.GatewayToken,
            result.Amount,
            result.IdempotencyKey,
            result.Status,
            result.PaidAt,
            result.CreatedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpPost("{id:long}/process")]
    [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProcessPaymentResponse>> Process(
        [FromRoute] long id,
        [FromServices] ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new ProcessPaymentResponse(
            result.PaymentId,
            result.Status,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpPost("{id:long}/complete")]
    [ProducesResponseType(typeof(CompletePaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CompletePaymentResponse>> Complete(
        [FromRoute] long id,
        [FromBody] CompletePaymentRequest request,
        [FromServices] ICommandHandler<CompletePaymentCommand, CompletePaymentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CompletePaymentCommand(
            id,
            request.ProviderTransactionId,
            request.GatewayToken,
            request.ResolvedProviderId
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CompletePaymentResponse(
            result.PaymentId,
            result.Status,
            result.ProviderTransactionId,
            result.GatewayToken,
            result.PaidAt,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(typeof(CancelPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CancelPaymentResponse>> Cancel(
        [FromRoute] long id,
        [FromServices] ICommandHandler<CancelPaymentCommand, CancelPaymentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CancelPaymentCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CancelPaymentResponse(
            result.PaymentId,
            result.Status,
            result.UpdatedAt
        );

        return Ok(response);
    }
}
