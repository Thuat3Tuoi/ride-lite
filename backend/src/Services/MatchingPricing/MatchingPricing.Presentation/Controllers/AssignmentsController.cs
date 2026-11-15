using MatchingPricing.Application.Abstractions.Messaging;
using MatchingPricing.Application.Features.Assignment.Commands.AcceptAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.CreateAssignment;
using MatchingPricing.Application.Features.Assignment.Commands.RejectAssignment;
using MatchingPricing.Application.Features.Assignment.Queries.GetAssignmentsByTripId;
using MatchingPricing.Presentation.Contracts.Requests;
using MatchingPricing.Presentation.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.Controllers;

[ApiController]
[Route("api/assignments")]
public sealed class AssignmentsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(DriverAssignmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DriverAssignmentResponse>> Create(
        [FromBody] CreateAssignmentRequest request,
        [FromServices] ICommandHandler<CreateAssignmentCommand, CreateAssignmentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssignmentCommand(request.TripId, request.DriverId, request.OfferDuration);
        var result = await handler.HandleAsync(command, cancellationToken);

        var response = new DriverAssignmentResponse(
            result.Id,
            result.TripId,
            result.DriverId,
            result.Status,
            result.OfferedAt,
            result.ExpiresAt,
            null,
            result.CreatedAt,
            result.CreatedAt
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{id:long}/accept")]
    [ProducesResponseType(typeof(AcceptAssignmentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Accept(
        long id,
        [FromBody] AcceptAssignmentRequest request,
        [FromServices] ICommandHandler<AcceptAssignmentCommand, AcceptAssignmentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new AcceptAssignmentCommand(id, request.DriverId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/reject")]
    [ProducesResponseType(typeof(RejectAssignmentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reject(
        long id,
        [FromBody] RejectAssignmentRequest request,
        [FromServices] ICommandHandler<RejectAssignmentCommand, RejectAssignmentResult> handler,
        CancellationToken cancellationToken)
    {
        var command = new RejectAssignmentCommand(id, request.DriverId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpGet("trip/{tripId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<DriverAssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DriverAssignmentResponse>>> GetByTripId(
        long tripId,
        [FromServices] IQueryHandler<GetAssignmentsByTripIdQuery, GetAssignmentsByTripIdResult> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAssignmentsByTripIdQuery(tripId);
        var result = await handler.HandleAsync(query, cancellationToken);

        var response = result.Items.Select(a => new DriverAssignmentResponse(
            a.Id,
            a.TripId,
            a.DriverId,
            a.Status,
            a.OfferedAt,
            a.ExpiresAt,
            a.RespondedAt,
            a.CreatedAt,
            a.UpdatedAt
        )).ToList();

        return Ok(response);
    }
}
