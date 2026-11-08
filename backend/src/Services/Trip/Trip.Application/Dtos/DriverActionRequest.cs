using System.ComponentModel.DataAnnotations;

namespace Trip.Application.Dtos;

public sealed class DriverActionRequest
{
    [Range(typeof(long), "1", "9223372036854775807")]
    public long DriverId { get; init; }
}
