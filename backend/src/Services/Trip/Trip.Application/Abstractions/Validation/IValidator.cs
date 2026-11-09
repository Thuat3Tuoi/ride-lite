namespace Trip.Application.Abstractions.Validation;

public interface IValidator<in T>
{
    void Validate(T instance);
}
