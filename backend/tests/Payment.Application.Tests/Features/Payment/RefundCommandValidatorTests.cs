using Payment.Application.Features.Refund.Commands.CompleteRefund;
using Payment.Application.Features.Refund.Commands.CreateRefund;
using Payment.Application.Features.Refund.Commands.ProcessRefund;
using Payment.Application.Features.Refund.Queries.GetRefundById;
using Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class RefundCommandValidatorTests
{
    private readonly CreateRefundCommandValidator _createValidator = new();
    private readonly ProcessRefundCommandValidator _processValidator = new();
    private readonly CompleteRefundCommandValidator _completeValidator = new();
    private readonly GetRefundByIdQueryValidator _getByIdValidator = new();
    private readonly GetRefundsByPaymentIdQueryValidator _getByPaymentIdValidator = new();

    [Fact]
    public void CreateRefund_WithValidData_ShouldBeValid()
    {
        var command = new CreateRefundCommand(1, 25000m, "refund-key-1", "Customer requested");
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateRefund_WithInvalidPaymentId_ShouldHaveInvalidPaymentIdError(long paymentId)
    {
        var command = new CreateRefundCommand(paymentId, 25000m, "refund-key-1");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CreateRefund_WithInvalidAmount_ShouldHaveInvalidAmountError(decimal amount)
    {
        var command = new CreateRefundCommand(1, amount, "refund-key-1");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidAmount, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRefund_WithEmptyIdempotencyKey_ShouldHaveInvalidParametersError(string key)
    {
        var command = new CreateRefundCommand(1, 25000m, key);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreateRefund_WithEmptyReason_ShouldHaveInvalidReasonError()
    {
        var command = new CreateRefundCommand(1, 25000m, "refund-key-1", "   ");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidReason, result.Errors[0].CustomState);
    }

    [Fact]
    public void ProcessRefund_WithValidData_ShouldBeValid()
    {
        var command = new ProcessRefundCommand(1);
        var result = _processValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ProcessRefund_WithInvalidId_ShouldHaveInvalidRefundIdError()
    {
        var command = new ProcessRefundCommand(0);
        var result = _processValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidRefundId, result.Errors[0].CustomState);
    }

    [Fact]
    public void CompleteRefund_WithValidData_ShouldBeValid()
    {
        var command = new CompleteRefundCommand(1, "prov-ref-123");
        var result = _completeValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CompleteRefund_WithInvalidId_ShouldHaveInvalidRefundIdError()
    {
        var command = new CompleteRefundCommand(0, "prov-ref-123");
        var result = _completeValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidRefundId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CompleteRefund_WithEmptyProviderRefundId_ShouldHaveInvalidParametersError(string provRefundId)
    {
        var command = new CompleteRefundCommand(1, provRefundId);
        var result = _completeValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetRefundById_WithValidId_ShouldBeValid()
    {
        var query = new GetRefundByIdQuery(1);
        var result = _getByIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetRefundById_WithInvalidId_ShouldHaveRefundNotFoundError()
    {
        var query = new GetRefundByIdQuery(0);
        var result = _getByIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.RefundNotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetRefundsByPaymentId_WithValidId_ShouldBeValid()
    {
        var query = new GetRefundsByPaymentIdQuery(1);
        var result = _getByPaymentIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetRefundsByPaymentId_WithInvalidId_ShouldHaveInvalidPaymentIdError()
    {
        var query = new GetRefundsByPaymentIdQuery(0);
        var result = _getByPaymentIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidPaymentId, result.Errors[0].CustomState);
    }
}

