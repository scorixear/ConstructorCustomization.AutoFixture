using AutoFixture;

namespace ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

/// <summary>
/// Defines a plugin that creates values for specific types during constructor customization.
/// Plugins are evaluated before built-in specimen strategies, allowing you to override value
/// creation for any type while falling back to default package behavior when not matched.
/// </summary>
public interface IValueCreationPlugin
{
    /// <summary>
    /// Determines whether this plugin can create a value for the specified type.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns><see langword="true"/> when this plugin can create a value for the type; otherwise, <see langword="false"/>.</returns>
    bool CanCreate(Type type);

    /// <summary>
    /// Creates a value for the specified type.
    /// </summary>
    /// <param name="type">The type to create a value for.</param>
    /// <param name="fixture">The active fixture instance.</param>
    /// <param name="valueCreationService">The value creation service for recursive value creation.</param>
    /// <returns>A created value for the specified type.</returns>
    object? Create(Type type, IFixture fixture, IValueCreationService valueCreationService);
}
