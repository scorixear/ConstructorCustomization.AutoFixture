namespace ConstructorCustomization.AutoFixture;

/// <summary>
/// Defines options used by constructor-based customization behavior.
/// </summary>
public class ConstructorCustomizationOptions
{
    /// <summary>
    /// Gets or sets the number of elements generated for collection-based constructor arguments.
    /// </summary>
    public int CollectionItemCount { get; init; } = 3;

    /// <summary>
    /// Gets or sets the comparer used when matching configured property names.
    /// </summary>
    public StringComparer PropertyNameComparer { get; init; } = StringComparer.OrdinalIgnoreCase;
}
