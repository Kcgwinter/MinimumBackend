namespace Api.Middleware;

using System.Text;
using Microsoft.Extensions.Logging;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await LogRequest(context);

        var originalBodyStream = context.Response.Body;

        // Use standard 'using' block without the extra nested braces layout
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
        finally
        {
            // We use a try/finally to ensure that even if downstream code crashes,
            // we ALWAYS restore the original stream and log what we can.
            await LogResponse(context, responseBody);

            // Copy our intercepted buffer back to the real response stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        // CRITICAL: Pass Encoding and leaveOpen: true to prevent disposing the Request Body
        using (
            var requestStream = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true
            )
        )
        {
            var requestBody = await requestStream.ReadToEndAsync();
            context.Request.Body.Position = 0;

            _logger.LogInformation(
                "HTTP Request Information: Method: {Method}, Path: {Path}, Body: {Body}",
                context.Request.Method,
                context.Request.Path,
                requestBody
            );
        }
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);

        // CRITICAL: Pass Encoding and leaveOpen: true to prevent disposing your memory stream
        using (
            var responseStream = new StreamReader(
                responseBody,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true
            )
        )
        {
            var responseBodyText = await responseStream.ReadToEndAsync();

            _logger.LogInformation(
                "HTTP Response Information: StatusCode: {StatusCode}, Body: {Body}",
                context.Response.StatusCode,
                responseBodyText
            );
        }

        // Removed the CopyToAsync from here. It is now handled cleanly in the InvokeAsync finally block.
    }
}
