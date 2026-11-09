namespace BuildingBlocks.SharedKernel.Errors;

public static class SystemErrors
{
    public static readonly ErrorCode InternalError = new(
        9999,
        "An unexpected error occurred.",
        ErrorType.Internal
    );
}

