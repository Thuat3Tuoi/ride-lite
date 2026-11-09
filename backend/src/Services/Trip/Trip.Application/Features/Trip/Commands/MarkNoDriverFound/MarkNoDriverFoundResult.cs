namespace Trip.Application.Features.Trip.Commands.MarkNoDriverFound;

public sealed record MarkNoDriverFoundResult(
    long Id,
    string Status,
    DateTimeOffset UpdatedAt
);

