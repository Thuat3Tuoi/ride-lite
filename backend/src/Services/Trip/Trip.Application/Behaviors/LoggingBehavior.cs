using Microsoft.Extensions.Logging;

namespace Trip.Application.Behaviors;

public static class LoggingBehavior
{
    public static void LogHandled(ILogger logger, string useCase, long tripId) =>
        logger.LogInformation("{UseCase} handled for trip {TripId}", useCase, tripId);
}
