using BuildingBlocks.SharedKernel.Exceptions;
using Payment.Application.Common.Validation;
using Payment.Application.Features.Payment.Commands.CreatePayment;
using Payment.Application.Features.Payment.Queries.GetPaymentById;
using Payment.Application.Tests.Fakes;
using Payment.Domain.Errors;
using Xunit;

namespace Payment.Application.Tests.Features.Payment;

public sealed class ValidationDecoratorTests
{
    [Fact]
    public async Task ValidationCommandHandlerDecorator_WithInvalidCommand_ShouldThrowAppException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new CreatePaymentHandler(repository);
        var validator = new CreatePaymentCommandValidator();
        var decoratedHandler = new ValidationCommandHandlerDecorator<CreatePaymentCommand, CreatePaymentResult>(
            handler,
            validator
        );

        var invalidCommand = new CreatePaymentCommand(
            TripId: 0,
            PaymentOptionId: 1,
            Amount: 50000m
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => decoratedHandler.HandleAsync(invalidCommand)
        );

        Assert.Equal(PaymentErrors.InvalidTripId.Code, exception.Error.Code);
        Assert.Empty(repository.Payments);
    }

    [Fact]
    public async Task ValidationQueryHandlerDecorator_WithInvalidQuery_ShouldThrowAppException()
    {
        // Arrange
        var repository = new FakePaymentRepository();
        var handler = new GetPaymentByIdHandler(repository);
        var validator = new GetPaymentByIdQueryValidator();
        var decoratedHandler = new ValidationQueryHandlerDecorator<GetPaymentByIdQuery, GetPaymentByIdResult>(
            handler,
            validator
        );

        var invalidQuery = new GetPaymentByIdQuery(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => decoratedHandler.HandleAsync(invalidQuery)
        );

        Assert.Equal(PaymentErrors.NotFound.Code, exception.Error.Code);
    }
}
