using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;
using Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;
using Payment.Presentation.Contracts.Requests;
using Payment.Presentation.Contracts.Responses;

namespace Payment.Presentation.Controllers;

[ApiController]
[Route("api/ledger-entries")]
public sealed class LedgerEntriesController : ControllerBase
{
    private readonly CreatePaymentLedgerEntryHandler _createPaymentLedgerEntryHandler;
    private readonly CreateRefundLedgerEntryHandler _createRefundLedgerEntryHandler;
    private readonly GetLedgerEntriesByPaymentIdHandler _getLedgerEntriesByPaymentIdHandler;
    private readonly GetLedgerEntriesByRefundIdHandler _getLedgerEntriesByRefundIdHandler;

    public LedgerEntriesController(
        CreatePaymentLedgerEntryHandler createPaymentLedgerEntryHandler,
        CreateRefundLedgerEntryHandler createRefundLedgerEntryHandler,
        GetLedgerEntriesByPaymentIdHandler getLedgerEntriesByPaymentIdHandler,
        GetLedgerEntriesByRefundIdHandler getLedgerEntriesByRefundIdHandler)
    {
        _createPaymentLedgerEntryHandler = createPaymentLedgerEntryHandler;
        _createRefundLedgerEntryHandler = createRefundLedgerEntryHandler;
        _getLedgerEntriesByPaymentIdHandler = getLedgerEntriesByPaymentIdHandler;
        _getLedgerEntriesByRefundIdHandler = getLedgerEntriesByRefundIdHandler;
    }

    [HttpPost("payment")]
    [ProducesResponseType(typeof(CreatePaymentLedgerEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePaymentLedgerEntryResponse>> CreatePaymentEntry(
        [FromBody] CreatePaymentLedgerEntryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentLedgerEntryCommand(
            request.PaymentId,
            request.Amount,
            request.DebitAccount,
            request.CreditAccount
        );

        var result = await _createPaymentLedgerEntryHandler.HandleAsync(command, cancellationToken);

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
        CancellationToken cancellationToken)
    {
        var command = new CreateRefundLedgerEntryCommand(
            request.PaymentId,
            request.RefundId,
            request.Amount,
            request.DebitAccount,
            request.CreditAccount
        );

        var result = await _createRefundLedgerEntryHandler.HandleAsync(command, cancellationToken);

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
        CancellationToken cancellationToken)
    {
        var query = new GetLedgerEntriesByPaymentIdQuery(paymentId);
        var result = await _getLedgerEntriesByPaymentIdHandler.HandleAsync(query, cancellationToken);

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
        CancellationToken cancellationToken)
    {
        var query = new GetLedgerEntriesByRefundIdQuery(refundId);
        var result = await _getLedgerEntriesByRefundIdHandler.HandleAsync(query, cancellationToken);

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

