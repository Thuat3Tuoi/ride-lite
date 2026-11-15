using Payment.Application.Features.Payment.Commands.CancelPayment;
using Payment.Application.Features.Payment.Commands.CompletePayment;
using Payment.Application.Features.Payment.Commands.CreatePayment;
using Payment.Application.Features.Payment.Commands.ProcessPayment;
using Payment.Application.Features.Payment.Queries.GetPaymentById;
using Payment.Application.Features.Payment.Queries.GetPaymentByTripId;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class PaymentCommandValidatorTests
{
    private readonly CreatePaymentCommandValidator _createValidator = new();
    private readonly ProcessPaymentCommandValidator _processValidator = new();
    private readonly CompletePaymentCommandValidator _completeValidator = new();
    private readonly CancelPaymentCommandValidator _cancelValidator = new();
    private readonly GetPaymentByIdQueryValidator _getByIdValidator = new();
    private readonly GetPaymentByTripIdQueryValidator _getByTripIdValidator = new();

    [Fact]
    public void CreatePayment_WithValidData_ShouldBeValid()
    {
        var command = new CreatePaymentCommand(1, 1, 50000m, "idem-1", 1);
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreatePayment_WithInvalidTripId_ShouldHaveInvalidTripIdError(long tripId)
    {
        var command = new CreatePaymentCommand(tripId, 1, 50000m);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidTripId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreatePayment_WithInvalidPaymentOptionId_ShouldHaveInvalidOptionIdError(long optionId)
    {
        var command = new CreatePaymentCommand(1, optionId, 50000m);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidOptionId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5000)]
    public void CreatePayment_WithInvalidAmount_ShouldHaveInvalidAmountError(decimal amount)
    {
        var command = new CreatePaymentCommand(1, 1, amount);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidAmount, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreatePayment_WithLongIdempotencyKey_ShouldHaveInvalidParametersError()
    {
        var command = new CreatePaymentCommand(1, 1, 50000m, new string('a', 129));
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreatePayment_WithInvalidProviderId_ShouldHaveInvalidProviderIdError()
    {
        var command = new CreatePaymentCommand(1, 1, 50000m, "key", -1);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidProviderId, result.Errors[0].CustomState);
    }

    [Fact]
    public void ProcessPayment_WithValidData_ShouldBeValid()
    {
        var command = new ProcessPaymentCommand(1);
        var result = _processValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ProcessPayment_WithInvalidId_ShouldHaveInvalidPaymentIdError()
    {
        var command = new ProcessPaymentCommand(0);
        var result = _processValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }

    [Fact]
    public void CompletePayment_WithValidData_ShouldBeValid()
    {
        var command = new CompletePaymentCommand(1, "txn-123");
        var result = _completeValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CompletePayment_WithInvalidId_ShouldHaveInvalidPaymentIdError()
    {
        var command = new CompletePaymentCommand(0, "txn-123");
        var result = _completeValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CompletePayment_WithEmptyTransactionId_ShouldHaveInvalidParametersError(string txnId)
    {
        var command = new CompletePaymentCommand(1, txnId);
        var result = _completeValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void CancelPayment_WithValidData_ShouldBeValid()
    {
        var command = new CancelPaymentCommand(1);
        var result = _cancelValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CancelPayment_WithInvalidId_ShouldHaveInvalidPaymentIdError()
    {
        var command = new CancelPaymentCommand(0);
        var result = _cancelValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetPaymentById_WithValidId_ShouldBeValid()
    {
        var query = new GetPaymentByIdQuery(1);
        var result = _getByIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentById_WithInvalidId_ShouldHaveNotFoundError()
    {
        var query = new GetPaymentByIdQuery(0);
        var result = _getByIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.NotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetPaymentByTripId_WithValidTripId_ShouldBeValid()
    {
        var query = new GetPaymentByTripIdQuery(1);
        var result = _getByTripIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentByTripId_WithInvalidTripId_ShouldHaveInvalidTripIdError()
    {
        var query = new GetPaymentByTripIdQuery(0);
        var result = _getByTripIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidTripId, result.Errors[0].CustomState);
    }
}

