using AutoFixture;

using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;

/// <summary>
/// Builds specimen values for specific type shapes.
/// </summary>
public interface ISpecimenBuilderStrategy
{
    /// <summary>
    /// Determines whether this strategy can build a value for the specified type.
    /// </summary>
    /// <param name="type">The target type to evaluate.</param>
    /// <returns><see langword="true"/> when the strategy can build the type; otherwise, <see langword="false"/>.</returns>
    bool CanBuild(Type type);

    /// <summary>
    /// Builds a value for the specified type.
    /// </summary>
    /// <param name="type">The target type to build.</param>
    /// <param name="fixture">The active fixture instance.</param>
    /// <param name="valueCreationService">The value creation service used for nested values.</param>
    /// <param name="options">The active customization options.</param>
    /// <returns>A generated value for the specified type.</returns>
    object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options);
}
