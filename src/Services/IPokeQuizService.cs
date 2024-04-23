using PokeQuizModels = PokeQuiz.Models.PokeQuiz;

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
    /// Get a <see cref="PokeQuizModels.Pokemon"/> by id.
    /// </summary>
    /// <param name="id">The id of the <see cref="PokeQuizModels.Pokemon"/></param>
    /// <returns>The object representing <see cref="PokeQuizModels.Pokemon"/></returns>
    Task<PokeQuizModels.Pokemon> GetPokemon(int id);

    /// <summary>
    /// Get a <see cref="PokeQuizModels.PokemonSpecies"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="PokeQuizModels.PokemonSpecies"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.PokemonSpecies"/></returns>
    Task<PokeQuizModels.PokemonSpecies> GetSpecies(string name);

    /// <summary>
    /// Get a <see cref="PokeQuizModels.PokemonSpecies"/> by id.
    /// </summary>
    /// <param name="id">The id of the <see cref="PokeQuizModels.PokemonSpecies"/></param>
    /// <returns>The object representing the <see cref="PokeQuizModels.PokemonSpecies"/></returns>
    Task<PokeQuizModels.PokemonSpecies> GetSpecies(int id);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Type"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Type"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Type"/></returns>
    Task<PokeQuizModels.Type> GetType(string name);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Type"/> by id
    /// </summary>
    /// <param name="id">The id of the <see cref="Models.PokeQuiz.Type"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Type"/></returns>
    Task<PokeQuizModels.Type> GetType(int id);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Move"/> by name
    /// </summary>
    /// <param name="name">The name of the <see cref="Models.PokeQuiz.Move"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Move"/></returns>
    Task<PokeQuizModels.Move> GetMove(string name);

    /// <summary>
    /// Get a <see cref="Models.PokeQuiz.Move"/> by id
    /// </summary>
    /// <param name="id">The id of the <see cref="Models.PokeQuiz.Move"/></param>
    /// <returns>The object representing the <see cref="Models.PokeQuiz.Move"/></returns>
    Task<PokeQuizModels.Move> GetMove(int id);
}