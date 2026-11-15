using System.Text.RegularExpressions;
using FluentValidation;
using MatchingPricing.Domain.Errors;

namespace MatchingPricing.Application.Features.Driver.Commands.CreateDriver;

public sealed partial class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    private static readonly Regex PhoneRegex = new(@"^0\d{9}$", RegexOptions.Compiled);

    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithState(_ => MatchingPricingErrors.InvalidFullName)
            .Must(name => name.Trim().Length <= 255)
            .WithState(_ => MatchingPricingErrors.InvalidFullName);

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .Must(phone => !string.IsNullOrWhiteSpace(phone))
            .WithState(_ => MatchingPricingErrors.InvalidPhoneNumber)
            .Must(phone => PhoneRegex.IsMatch(phone.Trim()))
            .WithState(_ => MatchingPricingErrors.InvalidPhoneNumber);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => !dob.HasValue || dob.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithState(_ => MatchingPricingErrors.InvalidDateOfBirth);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithState(_ => MatchingPricingErrors.InvalidParameters);
    }
}

