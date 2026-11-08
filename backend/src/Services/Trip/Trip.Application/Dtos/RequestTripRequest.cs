using System.ComponentModel.DataAnnotations;

namespace Trip.Application.Dtos;

public sealed class RequestTripRequest
{
    [Range(typeof(long), "1", "9223372036854775807")]
    public long RiderId { get; init; }

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string PickupAddress { get; init; } = string.Empty;

    [Range(typeof(decimal), "-90", "90")]
    public decimal PickupLatitude { get; init; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal PickupLongitude { get; init; }

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string DropoffAddress { get; init; } = string.Empty;

    [Range(typeof(decimal), "-90", "90")]
    public decimal DropoffLatitude { get; init; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal DropoffLongitude { get; init; }
}
