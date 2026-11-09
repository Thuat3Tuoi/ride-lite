using Trip.Application.Abstractions.Validation;

namespace Trip.Application.Behaviors;

public static class ValidationBehavior
{
    public static void Validate<T>(IValidator<T> validator, T request) => validator.Validate(request);
}
