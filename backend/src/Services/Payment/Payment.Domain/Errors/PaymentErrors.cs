using BuildingBlocks.SharedKernel.Errors;

namespace Payment.Domain.Errors;

public static class PaymentErrors
{
    // Payment (2001 - 2009)
    public static readonly ErrorCode NotFound = new(
        2001,
        "Payment not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode AlreadyExists = new(
        2002,
        "Payment already exists.",
        ErrorType.Conflict
    );

    public static readonly ErrorCode InvalidAmount = new(
        2003,
        "Payment amount is invalid.",
        ErrorType.Validation
    );

    public static readonly ErrorCode DuplicateIdempotencyKey = new(
        2004,
        "A transaction with the specified idempotency key already exists.",
        ErrorType.Conflict
    );

    // Refund (2010 - 2019)
    public static readonly ErrorCode RefundNotFound = new(
        2010,
        "Refund not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode DuplicateRefundIdempotencyKey = new(
        2011,
        "A refund with the specified idempotency key already exists.",
        ErrorType.Conflict
    );

    // PaymentMethod (2020 - 2029)
    public static readonly ErrorCode PaymentMethodNotFound = new(
        2020,
        "Payment method not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode PaymentMethodAlreadyExists = new(
        2021,
        "Payment method already exists.",
        ErrorType.Conflict
    );

    // PaymentOption (2030 - 2039)
    public static readonly ErrorCode PaymentOptionNotFound = new(
        2030,
        "Payment option not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode PaymentOptionAlreadyExists = new(
        2031,
        "Payment option already exists.",
        ErrorType.Conflict
    );

    // PaymentProvider (2040 - 2049)
    public static readonly ErrorCode PaymentProviderNotFound = new(
        2040,
        "Payment provider not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode PaymentProviderAlreadyExists = new(
        2041,
        "Payment provider already exists.",
        ErrorType.Conflict
    );

    // LedgerEntry (2050 - 2059)
    public static readonly ErrorCode LedgerEntryNotFound = new(
        2050,
        "Ledger entry not found.",
        ErrorType.NotFound
    );

    public static readonly ErrorCode DuplicateLedgerEntryRef = new(
        2051,
        "Ledger entry reference already exists.",
        ErrorType.Conflict
    );
}

