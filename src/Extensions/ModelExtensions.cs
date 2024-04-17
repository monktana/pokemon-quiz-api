using System.Text.Json;

namespace PokeQuiz.Extensions;

public static class ModelExtensions
{
    public static TPokeQuiz ToModel<TPokeApi, TPokeQuiz>(this string absoluteFilePath) where TPokeQuiz : IDeserializable<TPokeApi>, new()
    {
        var typeFileContent = File.ReadAllText(absoluteFilePath);
        var typeJson = JsonSerializer.Deserialize<TPokeApi>(typeFileContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        var tPokeQuiz = new TPokeQuiz();
        tPokeQuiz.FromPokeApiResource(typeJson);

        return tPokeQuiz;
    }
}

public interface IDeserializable<TPokeApi>
{
    public void FromPokeApiResource(TPokeApi pokeApiType);
}