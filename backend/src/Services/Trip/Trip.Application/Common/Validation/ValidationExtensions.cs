using BuildingBlocks.SharedKernel.Errors;
using BuildingBlocks.SharedKernel.Exceptions;
using FluentValidation;

namespace Trip.Application.Common.Validation;

public static class ValidationExtensions
{
    public static void EnsureValid<T>(this IValidator<T> validator, T instance)
    {
        var result = validator.Validate(instance);

        if (!result.IsValid)
        {
            var firstError = result.Errors[0];
            if (firstError.CustomState is ErrorCode errorCode)
            {
                throw new AppException(errorCode);
            }

            throw new AppException(new ErrorCode(400, firstError.ErrorMessage, ErrorType.Validation));
        }
    }

    public static async Task EnsureValidAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken = default)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);
        
        if (!result.IsValid)
        {
            var firstError = result.Errors[0];
            if (firstError.CustomState is ErrorCode errorCode)
            {
                throw new AppException(errorCode);
            }

            throw new AppException(new ErrorCode(400, firstError.ErrorMessage, ErrorType.Validation));
        }
    }
}
