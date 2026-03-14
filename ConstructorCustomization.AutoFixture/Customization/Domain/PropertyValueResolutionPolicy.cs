using AutoFixture;

using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

namespace ConstructorCustomization.AutoFixture.Customization.Domain;

internal static class PropertyValueResolutionPolicy
{
    public static bool TryResolveConfiguredValue(
        string propertyName,
        IPropertyValueStore overrideStore,
        IPropertyValueStore defaultStore,
        IFixture fixture,
        Func<object?, IFixture, object?> configuredValueResolver,
        out object? resolvedValue,
        out PropertyValueSource valueSource)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(overrideStore);
        ArgumentNullException.ThrowIfNull(defaultStore);
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentNullException.ThrowIfNull(configuredValueResolver);

        if (overrideStore.TryGetValue(propertyName, out var overrideValue))
        {
            resolvedValue = configuredValueResolver(overrideValue, fixture);
            valueSource = PropertyValueSource.Override;
            return true;
        }

        if (defaultStore.TryGetValue(propertyName, out var defaultValue))
        {
            resolvedValue = configuredValueResolver(defaultValue, fixture);
            valueSource = PropertyValueSource.Default;
            return true;
        }

        resolvedValue = null;
        valueSource = PropertyValueSource.Generated;
        return false;
    }
}
