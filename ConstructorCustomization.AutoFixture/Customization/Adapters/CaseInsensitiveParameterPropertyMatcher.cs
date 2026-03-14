using System.Reflection;

using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

namespace ConstructorCustomization.AutoFixture.Customization.Adapters;

/// <summary>
/// Matches constructor parameter names to configured property names using case-insensitive comparison.
/// </summary>
internal sealed class CaseInsensitiveParameterPropertyMatcher : IParameterPropertyMatcher
{
    private readonly StringComparer comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaseInsensitiveParameterPropertyMatcher"/> class.
    /// </summary>
    /// <param name="comparer">The comparer used when evaluating parameter and property names.</param>
    public CaseInsensitiveParameterPropertyMatcher(StringComparer comparer)
    {
        ThrowIfNull(comparer);
        this.comparer = comparer;
    }

    /// <inheritdoc />
    public bool TryGetPropertyName(ParameterInfo parameter, IEnumerable<string> configuredPropertyNames, out string propertyName)
    {
        ThrowIfNull(parameter);
        ThrowIfNull(configuredPropertyNames);

        var parameterName = parameter.Name;
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            propertyName = string.Empty;
            return false;
        }

        var directMatch = configuredPropertyNames.FirstOrDefault(name => comparer.Equals(name, parameterName));
        if (directMatch is not null)
        {
            propertyName = directMatch;
            return true;
        }

        var pascalCaseName = char.ToUpperInvariant(parameterName[0]) + parameterName[1..];
        var pascalCaseMatch = configuredPropertyNames.FirstOrDefault(name => comparer.Equals(name, pascalCaseName));
        if (pascalCaseMatch is not null)
        {
            propertyName = pascalCaseMatch;
            return true;
        }

        propertyName = string.Empty;
        return false;
    }
}
