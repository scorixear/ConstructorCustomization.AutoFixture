namespace ConstructorCustomization.AutoFixture.Customization.Application.Ports;

/// <summary>
/// Stores configured values keyed by property name.
/// </summary>
public interface IPropertyValueStore
{
    /// <summary>
    /// Gets all configured property names.
    /// </summary>
    IEnumerable<string> PropertyNames { get; }

    /// <summary>
    /// Determines whether a value exists for the specified property name.
    /// </summary>
    /// <param name="propertyName">The property name to look up.</param>
    /// <returns><see langword="true"/> if a value exists; otherwise, <see langword="false"/>.</returns>
    bool Contains(string propertyName);

    /// <summary>
    /// Attempts to get the value for the specified property name.
    /// </summary>
    /// <param name="propertyName">The property name to look up.</param>
    /// <param name="value">When this method returns <see langword="true"/>, contains the associated value.</param>
    /// <returns><see langword="true"/> when a value exists; otherwise, <see langword="false"/>.</returns>
    bool TryGetValue(string propertyName, out object? value);

    /// <summary>
    /// Sets the configured value for the specified property name.
    /// </summary>
    /// <param name="propertyName">The property name to set.</param>
    /// <param name="value">The configured value.</param>
    void SetValue(string propertyName, object? value);

    /// <summary>
    /// Removes the configured value for the specified property name.
    /// </summary>
    /// <param name="propertyName">The property name to remove.</param>
    /// <returns><see langword="true"/> if an entry was removed; otherwise, <see langword="false"/>.</returns>
    bool RemoveValue(string propertyName);

    /// <summary>
    /// Removes all configured property values.
    /// </summary>
    void Clear();
}
