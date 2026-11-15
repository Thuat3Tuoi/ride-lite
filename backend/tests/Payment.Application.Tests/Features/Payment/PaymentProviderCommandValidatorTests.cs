using Payment.Application.Features.PaymentProvider.Commands.ActivatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Commands.CreatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Commands.DeactivatePaymentProvider;
using Payment.Application.Features.PaymentProvider.Queries.GetActivePaymentProviders;
using Payment.Application.Features.PaymentProvider.Queries.GetPaymentProviderById;
using Payment.Domain.Enumerators;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class PaymentProviderCommandValidatorTests
{
    private readonly CreatePaymentProviderCommandValidator _createValidator = new();
    private readonly ActivatePaymentProviderCommandValidator _activateValidator = new();
    private readonly DeactivatePaymentProviderCommandValidator _deactivateValidator = new();
    private readonly GetPaymentProviderByIdQueryValidator _getByIdValidator = new();
    private readonly GetActivePaymentProvidersQueryValidator _getActiveValidator = new();

    [Fact]
    public void CreatePaymentProvider_WithValidData_ShouldBeValid()
    {
        var command = new CreatePaymentProviderCommand("STRIPE", "Stripe Gateway", PaymentHandlerType.Stripe);
        var result = _createValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentProvider_WithEmptyCode_ShouldHaveInvalidParametersError(string code)
    {
        var command = new CreatePaymentProviderCommand(code, "Stripe Gateway", PaymentHandlerType.Stripe);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentProvider_WithEmptyName_ShouldHaveInvalidNameError(string name)
    {
        var command = new CreatePaymentProviderCommand("STRIPE", name, PaymentHandlerType.Stripe);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidName, result.Errors[0].CustomState);
    }

    [Fact]
    public void CreatePaymentProvider_WithInvalidHandlerType_ShouldHaveInvalidParametersError()
    {
        var command = new CreatePaymentProviderCommand("STRIPE", "Stripe", (PaymentHandlerType)999);
        var result = _createValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidParameters, result.Errors[0].CustomState);
    }

    [Fact]
    public void ActivatePaymentProvider_WithValidId_ShouldBeValid()
    {
        var command = new ActivatePaymentProviderCommand(1);
        var result = _activateValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ActivatePaymentProvider_WithInvalidId_ShouldHaveInvalidProviderIdError()
    {
        var command = new ActivatePaymentProviderCommand(0);
        var result = _activateValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidProviderId, result.Errors[0].CustomState);
    }

    [Fact]
    public void DeactivatePaymentProvider_WithValidId_ShouldBeValid()
    {
        var command = new DeactivatePaymentProviderCommand(1);
        var result = _deactivateValidator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeactivatePaymentProvider_WithInvalidId_ShouldHaveInvalidProviderIdError()
    {
        var command = new DeactivatePaymentProviderCommand(0);
        var result = _deactivateValidator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.InvalidProviderId, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetPaymentProviderById_WithValidId_ShouldBeValid()
    {
        var query = new GetPaymentProviderByIdQuery(1);
        var result = _getByIdValidator.Validate(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPaymentProviderById_WithInvalidId_ShouldHavePaymentProviderNotFoundError()
    {
        var query = new GetPaymentProviderByIdQuery(0);
        var result = _getByIdValidator.Validate(query);
        Assert.False(result.IsValid);
        Assert.Equal(PaymentErrors.PaymentProviderNotFound, result.Errors[0].CustomState);
    }

    [Fact]
    public void GetActivePaymentProviders_ShouldBeValid()
    {
        var query = new GetActivePaymentProvidersQuery();
        var result = _getActiveValidator.Validate(query);
        Assert.True(result.IsValid);
    }
}

