using Payment.Application.Abstractions.Email;

namespace Payment.Integration.Tests.Infrastructure;

public sealed class FakeEmailSender : IEmailSender
{
    public bool ShouldSucceed { get; set; } = true;
    public List<string> Recipients { get; } = [];

    public Task<bool> TrySendWelcomeEmailAsync(
        string email,
        string name,
        CancellationToken cancellationToken = default)
    {
        Recipients.Add(email);
        return Task.FromResult(ShouldSucceed);
    }
}

