using BuildingBlocks.SharedKernel.Errors;

namespace MatchingPricing.Domain.Errors;

public static class MatchingPricingErrors
{
    // Driver (3001 - 3019)
    public static readonly ErrorCode DriverNotFound = new(
        3001,
        "Driver not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode DriverAlreadyExists = new(
        3002,
        "Driver with this phone number already exists.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode InvalidPhoneNumber = new(
        3003,
        "Invalid phone number.",
        ErrorType.Validation
    );

    public static readonly ErrorCode InvalidCoordinates = new(
        3004,
        "Invalid latitude or longitude coordinates.",
        ErrorType.Validation
    );

    public static readonly ErrorCode DriverNotAvailable = new(
        3005,
        "Driver is not available.",
        ErrorType.Validation
    );

    // Assignment (3020 - 3039)
    public static readonly ErrorCode AssignmentNotFound = new(
        3020,
        "Driver assignment not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode AssignmentExpired = new(
        3021,
        "Assignment offer has expired.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode TripAlreadyAssigned = new(
        3022,
        "Trip has already been assigned to another driver.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode InvalidAssignmentStatusTransition = new(
        3023,
        "Invalid assignment status transition.",
        ErrorType.Validation
    );

    public static readonly ErrorCode DriverMismatch = new(
        3024,
        "Driver does not match the assignment offer.",
        ErrorType.Forbidden
    );
}

