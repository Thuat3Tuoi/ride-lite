namespace BuildingBlocks.SharedKernel.Errors;

public sealed record ErrorCode(
    int Code,
    string Message,
    ErrorType Type
);

