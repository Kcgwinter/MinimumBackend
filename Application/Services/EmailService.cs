using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Application.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailService(
        IOptions<SmtpSettings> smtpSettings,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _smtpSettings = smtpSettings.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetBaseUrl()
    {
        var httpContext =
            _httpContextAccessor.HttpContext
            ?? throw new ApplicationException("No active HTTP context is available.");

        var request = httpContext.Request;
        return $"{request.Scheme}://{request.Host}{request.PathBase}";
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(to);
        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to send email to {to}: {ex.Message}");
        }
    }

    public async Task SendEmailConfirmationAsync(string to, string token)
    {
        var baseUrl = GetBaseUrl();
        var confirmationLink = $"{baseUrl}/confirm-email?token={WebUtility.UrlEncode(token)}";
        var subject = "Email Confirmation";
        var body =
            $"<p>Please confirm your email by clicking the link below:</p><p><a href=\"{confirmationLink}\">Confirm Email</a></p>";
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetAsync(string to, string token)
    {
        var baseUrl = GetBaseUrl();
        var resetLink = $"{baseUrl}/reset-password?token={token}";
        var subject = "Reset Your Password";
        var body =
            $@"
            <h1>Password Reset Request</h1>
            <p>Click the link below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>
            <p>Or copy this link: {resetLink}</p>
            <p>This link expires in 1 hour.</p>";

        await SendEmailAsync(to, subject, body);
    }
}
