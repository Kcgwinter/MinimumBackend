namespace Api.Middleware;

public class CsrfTokenMiddleware
{
    private readonly RequestDelegate _next;

    public CsrfTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (
            context.Request.Method == HttpMethods.Post
            || context.Request.Method == HttpMethods.Put
            || context.Request.Method == HttpMethods.Delete
        )
        {
            if (
                !context.Request.Headers.TryGetValue("X-CSRF-Token", out var csrfToken)
                || csrfToken != "your-secure-token"
            )
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("CSRF token validation failed.");
                return;
            }
        }

        await _next(context);
    }
}
