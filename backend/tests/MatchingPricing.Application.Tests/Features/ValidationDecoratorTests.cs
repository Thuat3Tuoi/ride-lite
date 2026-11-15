using BuildingBlocks.SharedKernel.Exceptions;
using MatchingPricing.Application.Common.Validation;
using MatchingPricing.Application.Features.Driver.Commands.CreateDriver;
using MatchingPricing.Application.Features.Driver.Queries.GetDriverById;
using MatchingPricing.Application.Tests.Fakes;
using MatchingPricing.Domain.Errors;
using Xunit;

namespace MatchingPricing.Application.Tests.Features;

public sealed class ValidationDecoratorTests
{
    [Fact]
    public async Task ValidationCommandHandlerDecorator_WithInvalidCommand_ShouldThrowAppException()
    {
        // Arrange
        var repository = new FakeDriverRepository();
        var handler = new CreateDriverHandler(repository);
        var validator = new CreateDriverCommandValidator();
        var decoratedHandler = new ValidationCommandHandlerDecorator<CreateDriverCommand, CreateDriverResult>(
            handler,
            validator
        );

        var invalidCommand = new CreateDriverCommand(
            FullName: "",
            PhoneNumber: "0901234567"
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => decoratedHandler.HandleAsync(invalidCommand)
        );

        Assert.Equal(MatchingPricingErrors.InvalidFullName.Code, exception.Error.Code);
        Assert.Empty(repository.Drivers);
    }

    [Fact]
    public async Task ValidationCommandHandlerDecorator_WithValidCommand_ShouldDelegateToInnerHandler()
    {
        // Arrange
        var repository = new FakeDriverRepository();
        var handler = new CreateDriverHandler(repository);
        var validator = new CreateDriverCommandValidator();
        var decoratedHandler = new ValidationCommandHandlerDecorator<CreateDriverCommand, CreateDriverResult>(
            handler,
            validator
        );

        var validCommand = new CreateDriverCommand(
            FullName: "Nguyen Van A",
            PhoneNumber: "0901234567"
        );

        // Act
        var result = await decoratedHandler.HandleAsync(validCommand);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Nguyen Van A", result.FullName);
        Assert.Single(repository.Drivers);
    }

    [Fact]
    public async Task ValidationQueryHandlerDecorator_WithInvalidQuery_ShouldThrowAppException()
    {
        // Arrange
        var repository = new FakeDriverRepository();
        var handler = new GetDriverByIdHandler(repository);
        var validator = new GetDriverByIdQueryValidator();
        var decoratedHandler = new ValidationQueryHandlerDecorator<GetDriverByIdQuery, GetDriverByIdResult>(
            handler,
            validator
        );

        var invalidQuery = new GetDriverByIdQuery(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(
            () => decoratedHandler.HandleAsync(invalidQuery)
        );

        Assert.Equal(MatchingPricingErrors.DriverNotFound.Code, exception.Error.Code);
    }

    [Fact]
    public async Task ValidationQueryHandlerDecorator_WithValidQuery_ShouldDelegateToInnerHandler()
    {
        // Arrange
        var repository = new FakeDriverRepository();
        var driver = Domain.Entities.Driver.Create("Nguyen Van A", "0901234567");
        await repository.AddAsync(driver);

        var handler = new GetDriverByIdHandler(repository);
        var validator = new GetDriverByIdQueryValidator();
        var decoratedHandler = new ValidationQueryHandlerDecorator<GetDriverByIdQuery, GetDriverByIdResult>(
            handler,
            validator
        );

        var validQuery = new GetDriverByIdQuery(driver.Id);

        // Act
        var result = await decoratedHandler.HandleAsync(validQuery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driver.Id, result.Id);
        Assert.Equal("Nguyen Van A", result.FullName);
    }
}

