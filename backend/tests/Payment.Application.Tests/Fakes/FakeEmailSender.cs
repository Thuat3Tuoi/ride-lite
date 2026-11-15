using Payment.Application.Abstractions.Email;

namespace Payment.Application.Tests.Fakes;

public sealed class FakeEmailSender : IEmailSender
{
    public bool ShouldSucceed { get; set; } = true;
    public bool WasCalled { get; private set; }
    public List<string> Recipients { get; } = [];

    public Task<bool> TrySendWelcomeEmailAsync(
        string email,
        string name,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        Recipients.Add(email);
        return Task.FromResult(ShouldSucceed);
    }
}

