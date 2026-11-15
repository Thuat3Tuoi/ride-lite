using Payment.Application.Features.PaymentOption.Commands.ActivatePaymentOption;
using Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;
using Payment.Application.Features.PaymentOption.Commands.DeactivatePaymentOption;
using Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;
using Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionById;
using Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class PaymentOptionCommandValidatorTests
{
    private readonly CreatePaymentOptionCommandValidator _createValidator = new();
    private readonly ActivatePaymentOptionCommandValidator _activateValidator = new();
    private readonly DeactivatePaymentOptionCommandValidator _deactivateValidator = new();
    private readonly GetPaymentOptionByIdQueryValidator _getByIdValidator = new();
    private readonly GetActivePaymentOptionsQueryValidator _getActiveValidator = new();
    private readonly GetPaymentOptionsByMethodIdQueryValidator _getByMethodIdValidator = new();

    [Fact]
    public void CreatePaymentOption_WithValidData_ShouldBeValid()
    {
        var command = new CreatePaymentOptionCommand(1, 1, "MOMO_WALLET", "MoMo Wallet");
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreatePaymentOption_WithInvalidMethodId_ShouldHaveInvalidMethodIdError(long methodId)
    {
        var command = new CreatePaymentOptionCommand(methodId, 1, "MOMO_WALLET", "MoMo Wallet");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidMethodId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreatePaymentOption_WithInvalidProviderId_ShouldHaveInvalidProviderIdError(long providerId)
    {
        var command = new CreatePaymentOptionCommand(1, providerId, "MOMO_WALLET", "MoMo Wallet");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidProviderId, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentOption_WithEmptyCode_ShouldHaveInvalidParametersError(string code)
    {
        var command = new CreatePaymentOptionCommand(1, 1, code, "MoMo Wallet");
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void ActivatePaymentOption_WithValidId_ShouldBeValid()
    {
        var command = new ActivatePaymentOptionCommand(1);
        var result = _activateValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ActivatePaymentOption_WithInvalidId_ShouldHaveInvalidOptionIdError()
    {
        var command = new ActivatePaymentOptionCommand(0);
        var result = _activateValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidOptionId, result.Errors[0].CustomState);
    }

    [Fact]
    public void DeactivatePaymentOption_WithValidId_ShouldBeValid()
    {
        var command = new DeactivatePaymentOptionCommand(1);
        var result = _deactivateValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeactivatePaymentOption_WithInvalidId_ShouldHaveInvalidOptionIdError()
    {
        var command = new DeactivatePaymentOptionCommand(0);
        var result = _deactivateValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidOptionId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetPaymentOptionById_WithValidId_ShouldBeValid()
    {
        var query = new GetPaymentOptionByIdQuery(1);
        var result = _getByIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentOptionById_WithInvalidId_ShouldHavePaymentOptionNotFoundError()
    {
        var query = new GetPaymentOptionByIdQuery(0);
        var result = _getByIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.PaymentOptionNotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetActivePaymentOptions_ShouldBeValid()
    {
        var query = new GetActivePaymentOptionsQuery();
        var result = _getActiveValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentOptionsByMethodId_WithValidId_ShouldBeValid()
    {
        var query = new GetPaymentOptionsByMethodIdQuery(1);
        var result = _getByMethodIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentOptionsByMethodId_WithInvalidId_ShouldHaveInvalidMethodIdError()
    {
        var query = new GetPaymentOptionsByMethodIdQuery(0);
        var result = _getByMethodIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidMethodId, result.Errors[0].CustomState);
    }
}

