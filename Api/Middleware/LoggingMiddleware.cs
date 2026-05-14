namespace Api.Middleware;

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
        using var responseBody = new MemoryStream();
        {
            context.Response.Body = responseBody;

            await _next(context);

            await LogResponse(context, responseBody);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        context.Response.Body = originalBodyStream;
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestStream = new StreamReader(context.Request.Body);
        var requestBody = await requestStream.ReadToEndAsync();
        context.Request.Body.Position = 0;

        _logger.LogInformation(
            "HTTP Request Information: Method: {Method}, Path: {Path}, Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            requestBody
        );
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseStream = new StreamReader(responseBody);
        var responseBodyText = await responseStream.ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation(
            "HTTP Response Information: StatusCode: {StatusCode}, Body: {Body}",
            context.Response.StatusCode,
            responseBodyText
        );

        // Write the response body to the original stream
        await responseBody.CopyToAsync(context.Response.Body);
    }
}
