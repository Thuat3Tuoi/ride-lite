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
    private readonly CreateAssignmentHandler _createAssignmentHandler;
    private readonly AcceptAssignmentHandler _acceptAssignmentHandler;
    private readonly RejectAssignmentHandler _rejectAssignmentHandler;
    private readonly GetAssignmentsByTripIdHandler _getAssignmentsByTripIdHandler;

    public AssignmentsController(
        CreateAssignmentHandler createAssignmentHandler,
        AcceptAssignmentHandler acceptAssignmentHandler,
        RejectAssignmentHandler rejectAssignmentHandler,
        GetAssignmentsByTripIdHandler getAssignmentsByTripIdHandler)
    {
        _createAssignmentHandler = createAssignmentHandler;
        _acceptAssignmentHandler = acceptAssignmentHandler;
        _rejectAssignmentHandler = rejectAssignmentHandler;
        _getAssignmentsByTripIdHandler = getAssignmentsByTripIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(DriverAssignmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DriverAssignmentResponse>> Create(
        [FromBody] CreateAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssignmentCommand(request.TripId, request.DriverId, request.OfferDuration);
        var result = await _createAssignmentHandler.HandleAsync(command, cancellationToken);

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
        CancellationToken cancellationToken)
    {
        var command = new AcceptAssignmentCommand(id, request.DriverId);
        var result = await _acceptAssignmentHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:long}/reject")]
    [ProducesResponseType(typeof(RejectAssignmentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        long id,
        [FromBody] RejectAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RejectAssignmentCommand(id, request.DriverId);
        var result = await _rejectAssignmentHandler.HandleAsync(command, cancellationToken);

        return Ok(result);
    }

    [HttpGet("trip/{tripId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<DriverAssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DriverAssignmentResponse>>> GetByTripId(
        long tripId,
        CancellationToken cancellationToken)
    {
        var query = new GetAssignmentsByTripIdQuery(tripId);
        var result = await _getAssignmentsByTripIdHandler.HandleAsync(query, cancellationToken);

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

