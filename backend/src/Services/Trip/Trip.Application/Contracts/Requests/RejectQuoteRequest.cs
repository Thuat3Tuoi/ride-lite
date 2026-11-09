using System.ComponentModel.DataAnnotations;

namespace Trip.Application.Contracts.Requests;

public sealed class RejectQuoteRequest
{
    [StringLength(500)]
    public string? Reason { get; init; }
}
