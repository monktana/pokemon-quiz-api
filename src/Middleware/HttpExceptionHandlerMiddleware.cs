using Microsoft.AspNetCore.Mvc;

namespace PokeQuiz.Middleware;

public class HttpExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (exception is not HttpRequestException httpRequestException) throw;

            context.Response.StatusCode = (int)httpRequestException.StatusCode!;

            var problemDetails = new ProblemDetails
            {
                Status = (int)httpRequestException.StatusCode!
            };
            await Results.Problem(problemDetails).ExecuteAsync(context);
        }
    }
}