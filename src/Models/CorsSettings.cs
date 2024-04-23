namespace PokeQuiz.Models;

public record CorsOptions
{
    public const string Position = "Cors";
    public string AllowedHosts { get; init; } = string.Empty;
    public string AllowedOrigins { get; init; } = string.Empty;
    public string[] AllowedMethods { get; init; } = [];
}