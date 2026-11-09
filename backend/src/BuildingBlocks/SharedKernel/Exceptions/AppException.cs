using BuildingBlocks.SharedKernel.Errors;

namespace BuildingBlocks.SharedKernel.Exceptions;

public sealed class AppException(ErrorCode error)
    : Exception(error.Message)
{
    public ErrorCode Error { get; } = error;
}

