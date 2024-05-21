using PokeQuiz.Extensions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeQuiz;

/// <summary>
/// Types are properties for Pokémon and their moves. Each type can be
/// super effective, effective, not very effective or completely ineffective against#
/// another type.
/// <remarks>The different type effectiveness are represented by <see cref="TypeEffectiveness"/></remarks>
/// </summary>
public class Type : IDeserializable<PokeApi.Type>
{
    /// <summary>
    /// The identifier for this resource.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name for this resource.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name of this resource listed in different languages.
    /// </summary>
    public List<InternationalName> Names { get; set; }

    /// <summary>
    /// Maps a PokeApi.Type to a PokeQuiz.Type
    /// </summary>
    /// <param name="type">The PokeApi.Type</param>
    /// <returns>The PokeQuiz.Type</returns>
    public void FromPokeApiResource(PokeApi.Type type)
    {
        Id = type.Id;
        Name = type.Name;
        Names = type.Names.Select(name =>
        {
            var internationalName = new InternationalName();
            internationalName.FromPokeApiResource(name);
            return internationalName;
        }).ToList();
    }
}

/// <summary>
/// Representation of all available types
/// </summary>
public static class Types
{
    public static string Bug = "bug";
    public static string Dark = "dark";
    public static string Dragon = "dragon";
    public static string Electric = "electric";
    public static string Fairy = "fairy";
    public static string Fighting = "fighting";
    public static string Fire = "fire";
    public static string Flying = "flying";
    public static string Ghost = "ghost";
    public static string Grass = "grass";
    public static string Ground = "ground";
    public static string Ice = "ice";
    public static string Normal = "normal";
    public static string Poison = "poison";
    public static string Psychic = "psychic";
    public static string Rock = "rock";
    public static string Steel = "steel";
    public static string Water = "water";
}

/// <summary>
/// Representation of the different type match ups.
/// </summary>
public enum TypeEffectiveness
{
    NoEffect,
    NotVeryEffective,
    Effective,
    SuperEffective
}