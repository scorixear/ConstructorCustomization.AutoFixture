using ConstructorCustomization.AutoFixture.SpecimenGeneration.Adapters;
using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.Composition;

/// <summary>
/// Creates the default service graph used by constructor customization.
/// </summary>
internal static class DefaultServiceFactory
{
    /// <summary>
    /// Creates the default ordered list of specimen strategies.
    /// </summary>
    /// <returns>An ordered list of default specimen strategies.</returns>
    public static IReadOnlyList<ISpecimenBuilderStrategy> CreateSpecimenStrategies()
    {
        return
        [
            new ArraySpecimenBuilderStrategy(),
            new ListLikeSpecimenBuilderStrategy(),
            new DictionaryLikeSpecimenBuilderStrategy(),
            new SetLikeSpecimenBuilderStrategy()
        ];
    }
}
