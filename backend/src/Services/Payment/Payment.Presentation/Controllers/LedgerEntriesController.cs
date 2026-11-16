using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Abstractions.Messaging;
using Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;
using Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("ledger-entries")]
public sealed class LedgerEntriesController : ControllerBase
{
    [HttpPost("payment")]
    [ProducesResponseType(typeof(CreatePaymentLedgerEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePaymentLedgerEntryResponse>> CreatePaymentEntry(
        [FromBody] CreatePaymentLedgerEntryRequest request,
        [FromServices] ICommandHandler<CreatePaymentLedgerEntryCommand, CreatePaymentLedgerEntryResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentLedgerEntryCommand(
            request.PaymentId,
            request.Amount,
            request.DebitAccount,
            request.CreditAccount
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CreatePaymentLedgerEntryResponse(
            result.Id,
            result.PaymentId,
            result.EntryType,
            result.EntryRef,
            result.DebitAccount,
            result.CreditAccount,
            result.Amount,
            result.CreatedAt
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("refund")]
    [ProducesResponseType(typeof(CreateRefundLedgerEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateRefundLedgerEntryResponse>> CreateRefundEntry(
        [FromBody] CreateRefundLedgerEntryRequest request,
        [FromServices] ICommandHandler<CreateRefundLedgerEntryCommand, CreateRefundLedgerEntryResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateRefundLedgerEntryCommand(
            request.PaymentId,
            request.RefundId,
            request.Amount,
            request.DebitAccount,
            request.CreditAccount
        );

        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new CreateRefundLedgerEntryResponse(
            result.Id,
            result.PaymentId,
            result.RefundId,
            result.EntryType,
            result.EntryRef,
            result.DebitAccount,
            result.CreditAccount,
            result.Amount,
            result.CreatedAt
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("payment/{paymentId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<LedgerEntryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LedgerEntryResponse>>> GetByPaymentId(
        [FromRoute] long paymentId,
        [FromServices] IQueryHandler<GetLedgerEntriesByPaymentIdQuery, GetLedgerEntriesByPaymentIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetLedgerEntriesByPaymentIdQuery(paymentId);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items
            .Select(e => new LedgerEntryResponse(
                e.Id,
                e.PaymentId,
                e.RefundId,
                e.EntryType,
                e.EntryRef,
                e.DebitAccount,
                e.CreditAccount,
                e.Amount,
                e.CreatedAt))
            .ToList();

        return Ok(response);
    }

    [HttpGet("refund/{refundId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<LedgerEntryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LedgerEntryResponse>>> GetByRefundId(
        [FromRoute] long refundId,
        [FromServices] IQueryHandler<GetLedgerEntriesByRefundIdQuery, GetLedgerEntriesByRefundIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetLedgerEntriesByRefundIdQuery(refundId);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items
            .Select(e => new LedgerEntryResponse(
                e.Id,
                e.PaymentId,
                e.RefundId,
                e.EntryType,
                e.EntryRef,
                e.DebitAccount,
                e.CreditAccount,
                e.Amount,
                e.CreatedAt))
            .ToList();

        return Ok(response);
    }
}
