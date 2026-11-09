using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Refund.Commands.CompleteRefund;
using Payment.Application.Features.Refund.Commands.CreateRefund;
using Payment.Application.Features.Refund.Commands.ProcessRefund;
using Payment.Application.Features.Refund.Queries.GetRefundById;
using Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("api/refunds")]
public sealed class RefundsController : ControllerBase
{
    private readonly CreateRefundHandler _createRefundHandler;
    private readonly ProcessRefundHandler _processRefundHandler;
    private readonly CompleteRefundHandler _completeRefundHandler;
    private readonly GetRefundByIdHandler _getRefundByIdHandler;
    private readonly GetRefundsByPaymentIdHandler _getRefundsByPaymentIdHandler;

    public RefundsController(
        CreateRefundHandler createRefundHandler,
        ProcessRefundHandler processRefundHandler,
        CompleteRefundHandler completeRefundHandler,
        GetRefundByIdHandler getRefundByIdHandler,
        GetRefundsByPaymentIdHandler getRefundsByPaymentIdHandler)
    {
        _createRefundHandler = createRefundHandler;
        _processRefundHandler = processRefundHandler;
        _completeRefundHandler = completeRefundHandler;
        _getRefundByIdHandler = getRefundByIdHandler;
        _getRefundsByPaymentIdHandler = getRefundsByPaymentIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateRefundResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateRefundResponse>> Create(
        [FromBody] CreateRefundRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRefundCommand(
            request.PaymentId,
            request.Amount,
            request.IdempotencyKey,
            request.Reason
        );

        var result = await _createRefundHandler.HandleAsync(command, cancellationToken);

        var response = new CreateRefundResponse(
            result.Id,
            result.PaymentId,
            result.Amount,
            result.Status,
            result.IdempotencyKey,
            result.Reason,
            result.CreatedAt
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RefundResponse>> GetById(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var query = new GetRefundByIdQuery(id);
        var result = await _getRefundByIdHandler.HandleAsync(query, cancellationToken);

        var response = new RefundResponse(
            result.Id,
            result.PaymentId,
            result.ProviderRefundId,
            result.Amount,
            result.Reason,
            result.Status,
            result.IdempotencyKey,
            result.RefundedAt,
            result.CreatedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpGet("payment/{paymentId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<RefundResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RefundResponse>>> GetByPaymentId(
        [FromRoute] long paymentId,
        CancellationToken cancellationToken)
    {
        var query = new GetRefundsByPaymentIdQuery(paymentId);
        var result = await _getRefundsByPaymentIdHandler.HandleAsync(query, cancellationToken);

        var response = result.Refunds
            .Select(r => new RefundResponse(
                r.Id,
                r.PaymentId,
                r.ProviderRefundId,
                r.Amount,
                r.Reason,
                r.Status,
                r.IdempotencyKey,
                r.RefundedAt,
                r.CreatedAt))
            .ToList();

        return Ok(response);
    }

    [HttpPost("{id:long}/process")]
    [ProducesResponseType(typeof(ProcessRefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProcessRefundResponse>> Process(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var command = new ProcessRefundCommand(id);
        var result = await _processRefundHandler.HandleAsync(command, cancellationToken);

        var response = new ProcessRefundResponse(
            result.RefundId,
            result.Status,
            result.UpdatedAt
        );

        return Ok(response);
    }

    [HttpPost("{id:long}/complete")]
    [ProducesResponseType(typeof(CompleteRefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CompleteRefundResponse>> Complete(
        [FromRoute] long id,
        [FromBody] CompleteRefundRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteRefundCommand(id, request.ProviderRefundId);
        var result = await _completeRefundHandler.HandleAsync(command, cancellationToken);

        var response = new CompleteRefundResponse(
            result.RefundId,
            result.Status,
            result.ProviderRefundId,
            result.RefundedAt,
            result.UpdatedAt
        );

        return Ok(response);
    }
}

