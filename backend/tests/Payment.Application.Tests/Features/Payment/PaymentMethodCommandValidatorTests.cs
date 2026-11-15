using Payment.Application.Features.PaymentMethod.Commands.ActivatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;
using Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;
using Payment.Application.Features.PaymentMethod.Queries.GetPaymentMethodById;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class PaymentMethodCommandValidatorTests
{
    private readonly CreatePaymentMethodCommandValidator _createValidator = new();
    private readonly ActivatePaymentMethodCommandValidator _activateValidator = new();
    private readonly DeactivatePaymentMethodCommandValidator _deactivateValidator = new();
    private readonly GetPaymentMethodByIdQueryValidator _getByIdValidator = new();
    private readonly GetActivePaymentMethodsQueryValidator _getActiveValidator = new();

    [Fact]
    public void CreatePaymentMethod_WithValidData_ShouldBeValid()
    {
        var command = new CreatePaymentMethodCommand("CARD", "Credit/Debit Card");
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentMethod_WithEmptyCode_ShouldHaveInvalidParametersError(string code)
    {
        var command = new CreatePaymentMethodCommand(code, "Credit/Debit Card");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentMethod_WithEmptyName_ShouldHaveInvalidNameError(string name)
    {
        var command = new CreatePaymentMethodCommand("CARD", name);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidName, result.Errors[0].CustomState);
    }

    [Fact]
    public void ActivatePaymentMethod_WithValidId_ShouldBeValid()
    {
        var command = new ActivatePaymentMethodCommand(1);
        var result = _activateValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ActivatePaymentMethod_WithInvalidId_ShouldHaveInvalidMethodIdError()
    {
        var command = new ActivatePaymentMethodCommand(0);
        var result = _activateValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidMethodId, result.Errors[0].CustomState);
    }

    [Fact]
    public void DeactivatePaymentMethod_WithValidId_ShouldBeValid()
    {
        var command = new DeactivatePaymentMethodCommand(1);
        var result = _deactivateValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeactivatePaymentMethod_WithInvalidId_ShouldHaveInvalidMethodIdError()
    {
        var command = new DeactivatePaymentMethodCommand(0);
        var result = _deactivateValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidMethodId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetPaymentMethodById_WithValidId_ShouldBeValid()
    {
        var query = new GetPaymentMethodByIdQuery(1);
        var result = _getByIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentMethodById_WithInvalidId_ShouldHavePaymentMethodNotFoundError()
    {
        var query = new GetPaymentMethodByIdQuery(0);
        var result = _getByIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.PaymentMethodNotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetActivePaymentMethods_ShouldBeValid()
    {
        var query = new GetActivePaymentMethodsQuery();
        var result = _getActiveValidator.Validate(query);
        Assert.True(result.IsValid);
    }
}

