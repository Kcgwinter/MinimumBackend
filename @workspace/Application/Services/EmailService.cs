using System.Net.Mail;
using System.Net;
using Serilog;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly Serilog.Log.Logger _logger;

    public EmailService(
        SmtpSettings smtpSettings,
        Serilog.Log.Logger logger)
    {
        _smtpSettings = smtpSettings;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = "Welcome to Minimum Backend!",
                Body = $"Hello {userName},\n\n" +
                       $"Welcome to Minimum Backend! Your account has been created successfully.\n\n" +
                       $"Best regards,\n" +
                       $"Minimum Backend Team",
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            await SendAsync(message);
            _logger.LogInformation($"Welcome email sent to {toEmail} for user {userName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email");
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        try
        {
            var resetLink = $"http://localhost:5000/Account/ResetPassword?token={resetToken}";

            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = "Password Reset Request",
                Body = $"Hello,\n\n" +
                       $"You have requested to reset your password.\n\n" +
                       $"Click the link below to reset your password:\n\n" +
                       $"<{resetLink}>\n\n" +
                       $"This link will expire in 1 hour.\n\n" +
                       $"If you did not request a password reset, please ignore this email.\n\n" +
                       $"Best regards,\n" +
                       $"Minimum Backend Team",
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            await SendAsync(message);
            _logger.LogInformation($"Password reset email sent to {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email");
            return false;
        }
    }

    public async Task<bool> SendVerificationEmailAsync(string toEmail, string userName, string verificationToken)
    {
        try
        {
            var verificationLink = $"http://localhost:5000/Account/VerifyEmail?token={verificationToken}";

            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = "Verify Your Email Address",
                Body = $"Hello {userName},\n\n" +
                       $"Thank you for registering with Minimum Backend.\n\n" +
                       $"Please verify your email address by clicking the link below:\n\n" +
                       $"<{verificationLink}>\n\n" +
                       $"Once verified, you will be able to log in and access your account.\n\n" +
                       $"If you did not create an account, please ignore this email.\n\n" +
                       $"Best regards,\n" +
                       $"Minimum Backend Team",
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            await SendAsync(message);
            _logger.LogInformation($"Verification email sent to {toEmail} for user {userName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email");
            return false;
        }
    }

    private async Task SendAsync(MailMessage message)
    {
        using var smtpClient = new SmtpClient
        {
            Host = _smtpSettings.Host,
            Port = _smtpSettings.Port,
            EnableSsl = _smtpSettings.UseSsl,
            Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password)
        };

        if (_smtpSettings.RequireAuthentication)
        {
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        await smtpClient.SendMailAsync(message);
    }
}
