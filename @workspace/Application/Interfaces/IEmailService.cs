using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
    Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken);
    Task<bool> SendVerificationEmailAsync(string toEmail, string userName, string verificationToken);
}
