using Trip.Application.Abstractions;

namespace Trip.Infrastructure.Clients;

public sealed class FakeMatchingClient : IMatchingClient
{
    public Task<MatchingResult> MatchAsync(
        MatchingRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(new MatchingResult(Fare: 85000m, DriverId: 501, EtaSeconds: 300));
}
