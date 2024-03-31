using System.Text.Json;
using PokeQuiz.Models.PokeQuiz;
using Type = PokeQuiz.Models.PokeQuiz.Type;

namespace PokeQuiz.Services;

public class TypeEffectivenessService
{
    private readonly Dictionary<string, Dictionary<string, float>> _typeMatrix;

    public TypeEffectivenessService()
    {
        var filePath = Path.Join(Directory.GetCurrentDirectory(), "Data", "PokemonTypeMatrix.json");
        var content = File.ReadAllText(filePath);
        _typeMatrix = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>(content);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attacking"></param>
    /// <param name="defending"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public TypeEffectiveness CalculateEffectiveness(Type attacking, IEnumerable<Type> defending)
    {
        var effectiveness = defending.Aggregate(1f, (prev, type) => prev * GetEffectiveness(attacking, type));
        return ParseFloatToTypeEffectiveness(effectiveness);
    }

    /// <summary>
    /// Get the effectiveness of attacking against defending.
    /// </summary>
    /// <param name="attacking">The attacking type</param>
    /// <param name="defending">The defending type</param>
    /// <returns>The effectiveness value</returns>
    private float GetEffectiveness(Type attacking, Type defending)
    {
        return _typeMatrix![attacking.Name][defending.Name];
    }

    /// <summary>
    /// Parse a float effectiveness to TypeEffectiveness
    /// </summary>
    /// <param name="value">The effectiveness value</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private TypeEffectiveness ParseFloatToTypeEffectiveness(float value)
    {
        switch (value)
        {
            case 0:
                return TypeEffectiveness.NoEffect;
            case 0.25f:
            case 0.5f:
                return TypeEffectiveness.NotVeryEffective;
            case 1f:
                return TypeEffectiveness.Effective;
            case 2f:
            case 4f:
                return TypeEffectiveness.SuperEffective;
            default:
                throw new ArgumentException("Unknown effectiveness value");
        }
    }
}