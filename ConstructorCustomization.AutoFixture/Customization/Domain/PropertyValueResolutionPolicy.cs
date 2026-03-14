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
        ICircularDependencyService circularDependencyService,
        out object? resolvedValue,
        out PropertyValueSource valueSource)
    {
        ThrowIfNullOrWhiteSpace(propertyName);
        ThrowIfNull(overrideStore);
        ThrowIfNull(defaultStore);
        ThrowIfNull(fixture);
        ThrowIfNull(circularDependencyService);

        if (overrideStore.TryGetValue(propertyName, out var overrideValue))
        {
            resolvedValue = ResolveConfiguredValue(propertyName, circularDependencyService, overrideValue, fixture);
            valueSource = PropertyValueSource.Override;
            return true;
        }

        if (defaultStore.TryGetValue(propertyName, out var defaultValue))
        {
            resolvedValue = ResolveConfiguredValue(propertyName, circularDependencyService, defaultValue, fixture);
            valueSource = PropertyValueSource.Default;
            return true;
        }

        resolvedValue = null;
        valueSource = PropertyValueSource.Generated;
        return false;
    }

    /// <summary>
    /// Resolves a configured value, evaluating deferred factories when necessary.
    /// </summary>
    private static object? ResolveConfiguredValue(string propertyName, ICircularDependencyService circularDependencyService, object? configuredValue, IFixture fixture)
    {
        var resolvedValue = configuredValue;
        if (configuredValue is ConfiguredValueFactory configuredValueFactory)
        {
            var hasCircularDependency = circularDependencyService.CheckCircularDependency(propertyName);
            if (hasCircularDependency)
            {
                resolvedValue = circularDependencyService.HandleCircularDependency(propertyName, configuredValueFactory.Resolve);
            }
            else
            {
                circularDependencyService.StartResolving(propertyName);
                resolvedValue = configuredValueFactory.Resolve(fixture);
                circularDependencyService.StopResolving(propertyName);
            }
        }

        return resolvedValue;
    }
}
