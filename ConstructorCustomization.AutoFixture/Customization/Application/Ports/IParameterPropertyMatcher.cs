using System.Reflection;

namespace ConstructorCustomization.AutoFixture.Customization.Application.Ports;

/// <summary>
/// Resolves a configured property name for a constructor parameter.
/// </summary>
public interface IParameterPropertyMatcher
{
    /// <summary>
    /// Attempts to find a matching configured property name for the provided parameter.
    /// </summary>
    /// <param name="parameter">The constructor parameter to match.</param>
    /// <param name="configuredPropertyNames">The set of configured property names.</param>
    /// <param name="propertyName">When this method returns <see langword="true"/>, contains the matched property name.</param>
    /// <returns><see langword="true"/> when a match is found; otherwise, <see langword="false"/>.</returns>
    bool TryGetPropertyName(ParameterInfo parameter, IEnumerable<string> configuredPropertyNames, out string propertyName);
}
