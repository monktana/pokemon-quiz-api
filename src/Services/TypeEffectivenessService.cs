using System.Text.Json;
using PokeQuiz.Models.PokeQuiz;
using Type = PokeQuiz.Models.PokeQuiz.Type;

namespace PokeQuiz.Services;

public class TypeEffectivenessService
{
    private readonly Dictionary<string, Dictionary<string, float>> _typeMatrix;

    public TypeEffectivenessService(string absolutePathToDataMatrix)
    {
        var content = File.ReadAllText(absolutePathToDataMatrix);
        _typeMatrix = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>(content)!;
    }

    /// <summary>
    /// Calculate the effectiveness of the attacking type against the defending types.
    /// <remarks>The effectiveness is calculated against the defending types combined. Not against each.</remarks>
    /// </summary>
    /// <param name="attacking">The attacking type</param>
    /// <param name="defending">An Iterator of defending types</param>
    /// <returns>The <see cref="TypeEffectiveness"/> of the attacking type against the defending</returns>
    public TypeEffectiveness CalculateEffectiveness(Type attacking, IEnumerable<Type> defending)
    {
        var effectiveness = defending.Aggregate(1f, (prev, type) => prev * GetEffectiveness(attacking, type));
        return ParseFloatToTypeEffectiveness(effectiveness);
    }

    /// <summary>
    /// Calculate the effectiveness of the attacking type against the defending types.
    /// <remarks>The effectiveness is calculated against the defending types combined. Not against each.</remarks>
    /// </summary>
    /// <param name="attacking">The attacking type</param>
    /// <param name="defending">An Iterator of defending types</param>
    /// <returns>The <see cref="TypeEffectiveness"/> of the attacking type against the defending</returns>
    public TypeEffectiveness CalculateEffectiveness(Type attacking, Type defending)
    {
        var effectiveness = GetEffectiveness(attacking, defending);
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
        return _typeMatrix[attacking.Name][defending.Name];
    }

    /// <summary>
    /// Parse a float effectiveness to TypeEffectiveness
    /// </summary>
    /// <param name="value">The effectiveness value</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private TypeEffectiveness ParseFloatToTypeEffectiveness(float value)
    {
        return value switch
        {
            0 => TypeEffectiveness.NoEffect,
            0.25f or 0.5f => TypeEffectiveness.NotVeryEffective,
            1f => TypeEffectiveness.Effective,
            2f or 4f => TypeEffectiveness.SuperEffective,
            _ => throw new ArgumentException("Unknown effectiveness value")
        };
    }
}