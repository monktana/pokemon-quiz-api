using System.Net;
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
            Type = GetType(exception)
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
            HttpRequestException requestException => GetTitleFromStatusCode(requestException.StatusCode ?? HttpStatusCode.InternalServerError),
            _ => GetTitleFromStatusCode(HttpStatusCode.InternalServerError)
        };

    private static string GetTitleFromStatusCode(HttpStatusCode statuscode) =>
        statuscode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
            HttpStatusCode.NotAcceptable => "Not Acceptable",
            HttpStatusCode.RequestTimeout => "Request Timeout",
            HttpStatusCode.Conflict => "Conflict",
            HttpStatusCode.PreconditionFailed => "Precondition Failed",
            HttpStatusCode.UnsupportedMediaType => "Unsupported Media Type",
            HttpStatusCode.UnprocessableContent => "Unprocessable Entity",
            HttpStatusCode.UpgradeRequired => "Upgrade Required",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            HttpStatusCode.BadGateway => "Bad Gateway",
            HttpStatusCode.ServiceUnavailable => "Service Unavailable",
            HttpStatusCode.GatewayTimeout => "Gateway Timeout",
            _ => "Internal Server Error"
        };

    private static string GetType(Exception exception) =>
        exception switch
        {
            HttpRequestException requestException => GetTypeFromStatusCode(requestException.StatusCode ?? HttpStatusCode.InternalServerError),
            _ => GetTypeFromStatusCode(HttpStatusCode.InternalServerError)
        };

    private static string GetTypeFromStatusCode(HttpStatusCode statuscode) =>
        statuscode switch
        {
            HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            HttpStatusCode.MethodNotAllowed => "https://tools.ietf.org/html/rfc9110#section-15.5.6",
            HttpStatusCode.NotAcceptable => "https://tools.ietf.org/html/rfc9110#section-15.5.7",
            HttpStatusCode.RequestTimeout => "https://tools.ietf.org/html/rfc9110#section-15.5.9",
            HttpStatusCode.Conflict => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            HttpStatusCode.PreconditionFailed => "https://tools.ietf.org/html/rfc9110#section-15.5.13",
            HttpStatusCode.UnsupportedMediaType => "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            HttpStatusCode.UnprocessableEntity => "https://tools.ietf.org/html/rfc4918#section-11.2",
            HttpStatusCode.UpgradeRequired => "https://tools.ietf.org/html/rfc9110#section-15.5.22",
            HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            HttpStatusCode.BadGateway => "https://tools.ietf.org/html/rfc9110#section-15.6.3",
            HttpStatusCode.ServiceUnavailable => "https://tools.ietf.org/html/rfc9110#section-15.6.4",
            HttpStatusCode.GatewayTimeout => "https://tools.ietf.org/html/rfc9110#section-15.6.5",
            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };
}