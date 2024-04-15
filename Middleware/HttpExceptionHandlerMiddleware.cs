using System.Net;

namespace PokeQuiz.Middleware;

public class HttpExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            if (exception is not HttpRequestException { StatusCode: HttpStatusCode.NotFound })
            {
                await Results.Problem().ExecuteAsync(httpContext);
            }
            else
            {
                await Results.NotFound().ExecuteAsync(httpContext);
            }
        }
    }
}