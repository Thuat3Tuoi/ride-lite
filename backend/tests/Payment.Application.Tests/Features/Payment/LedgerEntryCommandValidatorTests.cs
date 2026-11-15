using Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;
using Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;
using Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class LedgerEntryCommandValidatorTests
{
    private readonly CreatePaymentLedgerEntryCommandValidator _createPaymentEntryValidator = new();
    private readonly CreateRefundLedgerEntryCommandValidator _createRefundEntryValidator = new();
    private readonly GetLedgerEntriesByPaymentIdQueryValidator _getByPaymentIdValidator = new();
    private readonly GetLedgerEntriesByRefundIdQueryValidator _getByRefundIdValidator = new();

    [Fact]
    public void CreatePaymentEntry_WithValidData_ShouldBeValid()
    {
        var command = new CreatePaymentLedgerEntryCommand(1, 50000m, "DEBIT_CASH", "CREDIT_REVENUE");
        var result = _createPaymentEntryValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreatePaymentEntry_WithInvalidPaymentId_ShouldHaveInvalidPaymentIdError()
    {
        var command = new CreatePaymentLedgerEntryCommand(0, 50000m, "DEBIT_CASH", "CREDIT_REVENUE");
        var result = _createPaymentEntryValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreatePaymentEntry_WithInvalidAmount_ShouldHaveInvalidAmountError()
    {
        var command = new CreatePaymentLedgerEntryCommand(1, 0, "DEBIT_CASH", "CREDIT_REVENUE");
        var result = _createPaymentEntryValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidAmount, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentEntry_WithEmptyDebitAccount_ShouldHaveInvalidParametersError(string debit)
    {
        var command = new CreatePaymentLedgerEntryCommand(1, 50000m, debit, "CREDIT_REVENUE");
        var result = _createPaymentEntryValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateRefundEntry_WithValidData_ShouldBeValid()
    {
        var command = new CreateRefundLedgerEntryCommand(1, 1, 25000m, "DEBIT_REVENUE", "CREDIT_CASH");
        var result = _createRefundEntryValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateRefundEntry_WithInvalidRefundId_ShouldHaveInvalidRefundIdError()
    {
        var command = new CreateRefundLedgerEntryCommand(1, 0, 25000m, "DEBIT_REVENUE", "CREDIT_CASH");
        var result = _createRefundEntryValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidRefundId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetLedgerEntriesByPaymentId_WithValidId_ShouldBeValid()
    {
        var query = new GetLedgerEntriesByPaymentIdQuery(1);
        var result = _getByPaymentIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetLedgerEntriesByPaymentId_WithInvalidId_ShouldHaveInvalidPaymentIdError()
    {
        var query = new GetLedgerEntriesByPaymentIdQuery(0);
        var result = _getByPaymentIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetLedgerEntriesByRefundId_WithValidId_ShouldBeValid()
    {
        var query = new GetLedgerEntriesByRefundIdQuery(1);
        var result = _getByRefundIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetLedgerEntriesByRefundId_WithInvalidId_ShouldHaveInvalidRefundIdError()
    {
        var query = new GetLedgerEntriesByRefundIdQuery(0);
        var result = _getByRefundIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidRefundId, result.Errors[0].CustomState);
    }
}

