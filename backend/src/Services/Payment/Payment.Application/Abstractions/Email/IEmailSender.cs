namespace Payment.Application.Abstractions.Email;

public interface IEmailSender
{
    Task<bool> TrySendWelcomeEmailAsync(
        string email,
        string name,
        CancellationToken cancellationToken = default);
}