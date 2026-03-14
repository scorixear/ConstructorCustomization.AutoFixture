using System.Collections;

using AutoFixture;

using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.SpecimenGeneration.Adapters;

/// <summary>
/// Builds dictionary-like values for supported dictionary interfaces and types.
/// </summary>
internal sealed class DictionaryLikeSpecimenBuilderStrategy : ISpecimenBuilderStrategy
{
    /// <inheritdoc />
    public bool CanBuild(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(Dictionary<,>) ||
               genericTypeDefinition == typeof(IDictionary<,>) ||
               genericTypeDefinition == typeof(IReadOnlyDictionary<,>);
    }

    /// <inheritdoc />
    public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(valueCreationService);
        ArgumentNullException.ThrowIfNull(options);

        var genericArguments = type.GetGenericArguments();
        var keyType = genericArguments[0];
        var valueType = genericArguments[1];
        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dictionary = (IDictionary)Activator.CreateInstance(dictionaryType)!;

        for (var i = 0; i < options.CollectionItemCount; i++)
        {
            var key = valueCreationService.CreateValue(fixture, keyType);
            var value = valueCreationService.CreateValue(fixture, valueType);
            dictionary.Add(key!, value);
        }

        return dictionary;
    }
}
