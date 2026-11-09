using BuildingBlocks.SharedKernel.Errors;

namespace Trip.Domain.Errors;

public static class TripErrors
{
    // Trip (1001 - 1019)
    public static readonly ErrorCode NotFound = new(
        1001,
        "Trip not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode InvalidStatusTransition = new(
        1002,
        "Invalid trip status transition.",
        ErrorType.Validation
    );

    public static readonly ErrorCode AlreadyCompleted = new(
        1003,
        "Trip has already been completed.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode AlreadyCancelled = new(
        1004,
        "Trip has already been cancelled.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode InvalidCost = new(
        1005,
        "Trip cost must be non-negative.",
        ErrorType.Validation
    );

    public static readonly ErrorCode InvalidCoordinates = new(
        1006,
        "Invalid pickup or dropoff coordinates.",
        ErrorType.Validation
    );

    public static readonly ErrorCode InvalidDriverId = new(
        1007,
        "Driver ID must be a positive value.",
        ErrorType.Validation
    );

    // Rider (1020 - 1029)
    public static readonly ErrorCode RiderNotFound = new(
        1020,
        "Rider not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode RiderAlreadyExists = new(
        1021,
        "Rider with this phone number already exists.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode InvalidPhoneNumber = new(
        1022,
        "Invalid phone number.",
        ErrorType.Validation
    );

    public static readonly ErrorCode InvalidDateOfBirth = new(
        1023,
        "Date of birth cannot be in the future.",
        ErrorType.Validation
    );

    public static readonly ErrorCode InvalidFullName = new(
        1024,
        "Full name cannot be empty.",
        ErrorType.Validation
    );
}

