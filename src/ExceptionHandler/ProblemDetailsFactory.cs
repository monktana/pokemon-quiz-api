using Microsoft.AspNetCore.Mvc;

namespace PokeQuiz.ExceptionHandler;

public static class ProblemDetailsFactory
{
    public static ProblemDetails FromException(Exception exception)
    {
        return new ProblemDetails
        {
            Status = GetStatusCode(exception),
            Title = GetTitle(exception),
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            HttpRequestException requestException => (int)requestException.StatusCode!,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            HttpRequestException requestException => requestException.StatusCode.ToString() ?? "No StatusCode",
            _ => "Internal Servererror"
        };
}