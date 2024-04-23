using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace PokeQuiz.Models.PokeApi;

/// <summary>
/// Methods by which the player might can encounter Pokémon
/// in the wild, e.g., walking in tall grass.
/// </summary>
[ExcludeFromCodeCoverage]
public record EncounterMethod : NamedApiResource
{
    /// <summary>
    /// The identifier for this resource.
    /// </summary>
    public override int Id { get; set; }

    internal new static string ApiEndpoint { get; } = "encounter-method";

    /// <summary>
    /// The name for this resource.
    /// </summary>
    public override string Name { get; set; }

    /// <summary>
    /// A good value for sorting.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The name of this resource listed in different
    /// languages.
    /// </summary>
    public List<Names> Names { get; set; }
}

/// <summary>
/// Conditions which affect what pokemon might appear in the
/// wild, e.g., day or night.
/// </summary>
[ExcludeFromCodeCoverage]
public record EncounterCondition : NamedApiResource
{
    /// <summary>
    /// The identifier for this resource.
    /// </summary>
    public override int Id { get; set; }

    internal new static string ApiEndpoint { get; } = "encounter-condition";

    /// <summary>
    /// The name for this resource.
    /// </summary>
    public override string Name { get; set; }

    /// <summary>
    /// The name of this resource listed in different
    /// languages.
    /// </summary>
    public List<Names> Names { get; set; }

    /// <summary>
    /// A list of possible values for this encounter condition.
    /// </summary>
    public List<NamedApiResource<EncounterConditionValue>> Values { get; set; }
}

/// <summary>
/// Encounter condition values are the various states that an encounter
/// condition can have, i.e., time of day can be either day or night.
/// </summary>
[ExcludeFromCodeCoverage]
public record EncounterConditionValue : NamedApiResource
{
    /// <summary>
    /// The identifier for this resource.
    /// </summary>
    public override int Id { get; set; }

    internal new static string ApiEndpoint { get; } = "encounter-condition-value";

    /// <summary>
    /// The name for this resource.
    /// </summary>
    public override string Name { get; set; }

    /// <summary>
    /// The condition this encounter condition value pertains
    /// to.
    /// </summary>
    public NamedApiResource<EncounterCondition> Condition { get; set; }

    /// <summary>
    /// The name of this resource listed in different
    /// languages.
    /// </summary>
    public List<Names> Names { get; set; }
}