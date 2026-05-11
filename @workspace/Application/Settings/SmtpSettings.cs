using System.Collections.Generic;

namespace Application.Settings;

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "Minimum Backend";
    public bool UseSsl { get; set; } = true;
    public bool RequireAuthentication { get; set; } = true;
}
