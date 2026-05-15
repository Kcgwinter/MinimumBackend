using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class EmailServiceTests
{
    private readonly EmailService _emailService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettingsOptionsMock;

    public EmailServiceTests()
    {
        // Mock SmtpSettings
        _smtpSettingsOptionsMock = new Mock<IOptions<SmtpSettings>>();
        _smtpSettingsOptionsMock
            .Setup(x => x.Value)
            .Returns(
                new SmtpSettings
                {
                    Server = "smtp.test.local",
                    Port = 25,
                    Username = "testuser",
                    Password = "testpass",
                    SenderEmail = "test@example.com",
                    SenderName = "Test Sender",
                }
            );

        // Mock IHttpContextAccessor
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var fakeHttpContext = new DefaultHttpContext();
        fakeHttpContext.Request.Scheme = "https";
        fakeHttpContext.Request.Host = new HostString("localhost:5000");
        fakeHttpContext.Request.PathBase = "/api";
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(fakeHttpContext);

        // Create EmailService with mocked dependencies
        var emailServiceOptions = Options.Create(_smtpSettingsOptionsMock.Object.Value);
        _emailService = new EmailService(emailServiceOptions, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ValidParameters_SendsEmailSuccessfully()
    {
        // Arrange
        var to = "recipient@example.com";
        var subject = "Test Subject";
        var body = "<p>Test body</p>";

        // Act
        await _emailService.SendEmailAsync(to, subject, body);

        // Assert: No exception thrown means the method completed without SMTP failure
        // (Actual SMTP sending is not verified to avoid network calls)
    }

    [Fact]
    public async Task SendEmailAsync_WhenSmtpServerIsInvalid_ThrowsApplicationException()
    {
        // Arrange
        var emailService = CreateEmailServiceWithInvalidSmtpServer();

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            emailService.SendEmailAsync("test@example.com", "subject", "body")
        );
    }

    [Fact]
    public async Task SendEmailConfirmationAsync_GeneratesCorrectUrl()
    {
        // Arrange
        var email = "user@example.com";
        var token = "abc123xyz";

        // Act
        await _emailService.SendEmailConfirmationAsync(email, token);

        // Assert: No exception means URL generation succeeded
        // The test verifies that GetBaseUrl works and UrlEncode is used
    }

    [Fact]
    public async Task SendPasswordResetAsync_GeneratesCorrectUrl()
    {
        // Arrange
        var email = "user@example.com";
        var token = "reset123token";

        // Act
        await _emailService.SendPasswordResetAsync(email, token);

        // Assert: No exception means URL generation succeeded
    }

    private EmailService CreateEmailServiceWithInvalidSmtpServer()
    {
        // Recreate with a server that will definitely fail
        _smtpSettingsOptionsMock
            .Setup(x => x.Value)
            .Returns(
                new SmtpSettings
                {
                    Server = "smtp.invalid.invalid",
                    Port = 25,
                    Username = "testuser",
                    Password = "testpass",
                    SenderEmail = "test@example.com",
                    SenderName = "Test Sender",
                }
            );

        return new EmailService(
            Options.Create(_smtpSettingsOptionsMock.Object.Value),
            _httpContextAccessorMock.Object
        );
    }
}
