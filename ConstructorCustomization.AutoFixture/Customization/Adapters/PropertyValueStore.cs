using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

namespace ConstructorCustomization.AutoFixture.Customization.Adapters;

/// <summary>
/// Default in-memory implementation of <see cref="IPropertyValueStore"/>.
/// </summary>
internal sealed class PropertyValueStore : IPropertyValueStore
{
    private readonly Dictionary<string, object?> values;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyValueStore"/> class.
    /// </summary>
    /// <param name="comparer">The comparer used to match property names.</param>
    public PropertyValueStore(StringComparer comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        values = new Dictionary<string, object?>(comparer);
    }

    /// <inheritdoc />
    public IEnumerable<string> PropertyNames => values.Keys;

    /// <inheritdoc />
    public bool Contains(string propertyName) => values.ContainsKey(propertyName);

    /// <inheritdoc />
    public bool TryGetValue(string propertyName, out object? value) => values.TryGetValue(propertyName, out value);

    /// <inheritdoc />
    public void SetValue(string propertyName, object? value)
    {
        values[propertyName] = value;
    }

    /// <inheritdoc />
    public bool RemoveValue(string propertyName) => values.Remove(propertyName);

    /// <inheritdoc />
    public void Clear() => values.Clear();
}
