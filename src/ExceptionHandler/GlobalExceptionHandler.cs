using Microsoft.AspNetCore.Diagnostics;

namespace PokeQuiz.ExceptionHandler;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = ProblemDetailsFactory.FromException(exception);
        await Results.Problem(problemDetails: problemDetails).ExecuteAsync(httpContext);

        return true;
    }
}