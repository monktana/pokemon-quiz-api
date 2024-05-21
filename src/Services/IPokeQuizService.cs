using PokeQuizModels = PokeQuiz.Models.PokeQuiz;
using PokeApiModels = PokeQuiz.Models.PokeApi;

namespace PokeQuiz.Services;

public interface IPokeQuizService
{
    /// <summary>
    /// Constructs a <see cref="PokeQuizModels.Matchup"/>.
    /// </summary>
    /// <returns>The matchup containing two <see cref="PokeQuizModels.Pokemon"/> and a <see cref="PokeQuizModels.Move"/></returns>
    Task<PokeQuizModels.Matchup> GetMatchup();

    /// <summary>
    /// Get a <see cref="PokeQuizModels.Pokemon"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.Pokemon"/></param>
    /// <returns>The object representing <see cref="PokeQuizModels.Pokemon"/></returns>
    Task<PokeQuizModels.Pokemon> GetPokemon(string name);

    /// <summary>
    /// Get a <see cref="PokeQuizModels.PokemonSpecies"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.PokemonSpecies"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.PokemonSpecies"/></returns>
    Task<PokeQuizModels.PokemonSpecies> GetSpecies(string name);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Type"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Type"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Type"/></returns>
    Task<PokeQuizModels.Type> GetType(string name);

    /// <summary>
    /// Get an overview of all <see cref="Models.PokeQuiz.Type"/>
    /// </summary>
    /// <returns>A list containing all <see cref="Models.PokeQuiz.Type"/></returns>
    Task<List<PokeApiModels.NamedApiResource<PokeApiModels.Type>>> GetTypes();

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Move"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Move"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Move"/></returns>
    Task<PokeQuizModels.Move> GetMove(string name);
}