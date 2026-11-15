using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;
using Payment.Application.Tests.Fakes;
using Payment.Domain.Entities;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class CreatePaymentMethodHandlerTests
{
    [Fact]
    public async Task Create_ValidPaymentMethod_ShouldSaveSuccessfully()
    {
        // Arrange
        var repository = new FakePaymentMethodRepository();
        var handler = new CreatePaymentMethodHandler(repository);
        var command = new CreatePaymentMethodCommand(
            Code: "CARD",
            Name: "Credit / Debit Card",
            Active: true
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("CARD", result.Code);
        Assert.Equal("Credit / Debit Card", result.Name);
        Assert.True(result.Active);
        Assert.Single(repository.Methods);
        Assert.True(repository.Saved);
    }

    [Fact]
    public async Task Create_DuplicateCode_ShouldThrowConflict()
    {
        // Arrange
        var repository = new FakePaymentMethodRepository();
        var existing = PaymentMethod.Create("CARD", "Existing Card");
        await repository.AddAsync(existing);

        var handler = new CreatePaymentMethodHandler(repository);
        var command = new CreatePaymentMethodCommand(
            Code: "card", // lowercase should be normalized and conflict
            Name: "New Card",
            Active: true
        );

        // Act + Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => handler.HandleAsync(command)
        );

        Assert.Equal(PaymentErrors.PaymentMethodAlreadyExists.Code, exception.Error.Code);
        Assert.Single(repository.Methods);
        Assert.False(repository.Saved);
    }

    [Fact]
    public async Task Create_EmptyCode_ShouldThrowArgumentException()
    {
        // Arrange
        var repository = new FakePaymentMethodRepository();
        var handler = new CreatePaymentMethodHandler(repository);
        var command = new CreatePaymentMethodCommand(
            Code: "   ",
            Name: "Invalid Method",
            Active: true
        );

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command)
        );

        Assert.Empty(repository.Methods);
        Assert.False(repository.Saved);
    }
}

