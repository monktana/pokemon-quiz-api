using PokeQuiz.Extensions;
using PokeQuiz.Services;

namespace PokeQuiz.UnitTests.Services;

public class TypeEffectivenessServiceTests
{
    private readonly TypeEffectivenessService _service = new(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type", "PokemonTypeMatrix.json"));

    [Fact]
    public void TypeEffectivenessService_CalculatesEffective()
    {
        var normalFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json");
        var fightingFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/fighting.json");

        var effectiveness = _service.CalculateEffectiveness(
            normalFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
            fightingFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>()
        );

        Assert.Equal(Models.PokeQuiz.TypeEffectiveness.Effective, effectiveness);
    }

    [Fact]
    public void TypeEffectivenessService_CalculatesNoEffect()
    {
        var normalFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json");
        var ghostFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/ghost.json");

        var effectiveness = _service.CalculateEffectiveness(
            normalFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
            ghostFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>()
        );

        Assert.Equal(Models.PokeQuiz.TypeEffectiveness.NoEffect, effectiveness);
    }

    [Fact]
    public void TypeEffectivenessService_CalculatesNotVeryEffective()
    {
        var normalFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json");
        var rockFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/rock.json");

        var effectiveness = _service.CalculateEffectiveness(
            normalFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
            rockFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>()
        );

        Assert.Equal(Models.PokeQuiz.TypeEffectiveness.NotVeryEffective, effectiveness);
    }

    [Fact]
    public void TypeEffectivenessService_CalculatesSuperEffective()
    {
        var fireFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/fire.json");
        var grassFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/grass.json");

        var effectiveness = _service.CalculateEffectiveness(
            fireFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
            grassFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>()
        );

        Assert.Equal(Models.PokeQuiz.TypeEffectiveness.SuperEffective, effectiveness);
    }

    [Fact]
    public void TypeEffectivenessService_CalculatesEffectivenessAgainstMultipleTypes()
    {
        var normalFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json");
        var fightingFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/fighting.json");
        var flyingFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/flying.json");

        var effectiveness = _service.CalculateEffectiveness(
            normalFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
            new List<PokeQuiz.Models.PokeQuiz.Type>
            {
                fightingFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
                flyingFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
            }
        );

        Assert.Equal(Models.PokeQuiz.TypeEffectiveness.Effective, effectiveness);
    }

    [Fact]
    public void TypeEffectivenessService_ThrowsOnUnknownEffectivenessValue()
    {
        var serviceWithRiggedMatrix = new TypeEffectivenessService(Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type", "RiggedPokemonTypeMatrix.json"));
        var normalFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/normal.json");
        var fightingFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/fighting.json");
        var flyingFilePath = Path.Join(Directory.GetCurrentDirectory(), "../../../Fixtures/type/flying.json");

        Assert.Throws<ArgumentException>(() =>
        {
            serviceWithRiggedMatrix.CalculateEffectiveness(
                normalFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
                new List<PokeQuiz.Models.PokeQuiz.Type>
                {
                    fightingFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
                    flyingFilePath.ToModel<PokeQuiz.Models.PokeApi.Type, PokeQuiz.Models.PokeQuiz.Type>(),
                }
            );
        });
    }
}