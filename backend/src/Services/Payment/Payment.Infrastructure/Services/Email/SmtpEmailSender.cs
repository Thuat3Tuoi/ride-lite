using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Payment.Application.Abstractions.Email;

namespace Payment.Infrastructure.Services.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        IOptions<SmtpOptions> options,
        ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> TrySendWelcomeEmailAsync(
        string email,
        string name,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _options.FromName,
                    _options.FromEmail
                )
            );

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Chào mừng bạn đăng ký!";

            message.Body = new TextPart("plain")
            {
                Text = $"Xin chào {name},\n\n" +
                       "Tài khoản của bạn đã được tạo thành công."
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _options.Host,
                _options.Port,
                SecureSocketOptions.None,
                cancellationToken
            );

            await smtp.SendAsync(
                message,
                cancellationToken
            );

            await smtp.DisconnectAsync(
                true,
                cancellationToken
            );

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Cannot send welcome email to {Email}",
                email
            );

            return false;
        }
    }
}
