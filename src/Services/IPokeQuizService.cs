using PokeQuiz.Models.PokeQuiz;

namespace PokeQuiz.Services;

public interface IPokeQuizService
{
    /// <summary>
    /// Constructs a <see cref="Matchup"/>.
    /// </summary>
    /// <returns>The matchup containing two <see cref="Pokemon"/> and a <see cref="Move"/></returns>
    Task<Models.PokeQuiz.Matchup> GetMatchup();

    /// <summary>
    /// Get a <see cref="Pokemon"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="Pokemon"/></param>
    /// <returns>The object representing <see cref="Pokemon"/></returns>
    Task<Models.PokeQuiz.Pokemon> GetPokemon(string name);

    /// <summary>
    /// Get a <see cref="Pokemon"/> by id.
    /// </summary>
    /// <param name="id">The id of the <see cref="Pokemon"/></param>
    /// <returns>The object representing <see cref="Pokemon"/></returns>
    Task<Models.PokeQuiz.Pokemon> GetPokemon(int id);

    /// <summary>
    /// Get a <see cref="PokemonSpecies"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokemonSpecies"/></param>
    /// <returns>The object representing the <see cref="PokemonSpecies"/></returns>
    Task<Models.PokeQuiz.PokemonSpecies> GetSpecies(string name);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Type"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Type"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Type"/></returns>
    Task<Models.PokeQuiz.Type> GetType(string name);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Type"/> by id
    /// </summary>
    /// <param name="id">The id of the <see cref="Models.PokeQuiz.Type"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Type"/></returns>
    Task<Models.PokeQuiz.Type> GetType(int id);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Move"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Move"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Move"/></returns>
    Task<Models.PokeQuiz.Move> GetMove(string name);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Move"/> by id
    /// </summary>
    /// <param name="id">The id of the <see cref="Models.PokeQuiz.Move"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Move"/></returns>
    Task<Models.PokeQuiz.Move> GetMove(int id);
}