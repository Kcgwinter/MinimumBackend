using System;

namespace Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailConfirmationAsync(string to, string token);
    Task SendPasswordResetAsync(string to, string token);
}
