using System.IO;
using System.Text;
using Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Api;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenDelegateThrows_ReturnsJsonErrorResponse()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();

        var env = new Mock<IHostEnvironment>();
        env.SetupGet(x => x.EnvironmentName).Returns(Environments.Development);

        var context = new DefaultHttpContext();
        await using var bodyStream = new MemoryStream();
        context.Response.Body = bodyStream;

        var middleware = new ExceptionMiddleware(
            _ => throw new InvalidOperationException("boom"),
            logger.Object,
            env.Object
        );

        await middleware.InvokeAsync(context);

        bodyStream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(bodyStream, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        Assert.Equal("application/json", context.Response.ContentType);
        Assert.Contains("boom", body);
        Assert.Contains("statusCode", body, StringComparison.OrdinalIgnoreCase);
    }
}
