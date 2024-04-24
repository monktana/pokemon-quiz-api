using System.ComponentModel.DataAnnotations;

namespace PokeQuiz.Models;

public record CorsSettings
{
    public const string Position = "Cors";

    [Required] public string[] AllowedOrigins { get; init; }

    [Required] public string[] AllowedMethods { get; init; }
}