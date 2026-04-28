using System;

namespace Api.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
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

        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        Console.WriteLine($"Body: {requestBody}");
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseStream = new StreamReader(responseBody);
        var responseBodyText = await responseStream.ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        Console.WriteLine($"Response: {context.Response.StatusCode}");
        Console.WriteLine($"Body: {responseBodyText}");

        // Write the response body to the original stream
        await responseBody.CopyToAsync(context.Response.Body);
    }
}
