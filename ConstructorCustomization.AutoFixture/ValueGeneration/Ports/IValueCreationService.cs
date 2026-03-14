using System.Reflection;

using AutoFixture;

namespace ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

/// <summary>
/// Creates constructor argument values using configured strategies and fixture fallback.
/// </summary>
public interface IValueCreationService
{
    /// <summary>
    /// Creates a value for the specified constructor parameter.
    /// </summary>
    /// <param name="fixture">The active fixture instance.</param>
    /// <param name="parameter">The constructor parameter that requires a value.</param>
    /// <returns>A generated value for the parameter.</returns>
    object? CreateValue(IFixture fixture, ParameterInfo parameter);

    /// <summary>
    /// Creates a value for the specified type.
    /// </summary>
    /// <param name="fixture">The active fixture instance.</param>
    /// <param name="type">The type that requires a value.</param>
    /// <returns>A generated value for the type.</returns>
    object? CreateValue(IFixture fixture, Type type);
}
