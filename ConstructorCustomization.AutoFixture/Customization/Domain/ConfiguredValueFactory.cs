using AutoFixture;

namespace ConstructorCustomization.AutoFixture.Customization.Domain;

/// <summary>
/// Represents a deferred configured value that is resolved against an active fixture instance.
/// </summary>
internal sealed class ConfiguredValueFactory
{
    private readonly Func<IFixture, object?> resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguredValueFactory"/> class.
    /// </summary>
    /// <param name="resolver">A resolver that creates a value using the active fixture instance.</param>
    public ConfiguredValueFactory(Func<IFixture, object?> resolver)
    {
        this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    /// <summary>
    /// Resolves the configured value.
    /// </summary>
    /// <param name="fixture">The active fixture instance.</param>
    /// <returns>The resolved value.</returns>
    public object? Resolve(IFixture fixture) => resolver(fixture);
}
