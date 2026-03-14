using AutoFixture;

using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.SpecimenGeneration.Adapters;

/// <summary>
/// Builds set-like values for supported set interfaces and types.
/// </summary>
internal sealed class SetLikeSpecimenBuilderStrategy : ISpecimenBuilderStrategy
{
    /// <inheritdoc />
    public bool CanBuild(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(HashSet<>) ||
               genericTypeDefinition == typeof(ISet<>);
    }

    /// <inheritdoc />
    public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
    {
        ThrowIfNull(type);
        ThrowIfNull(valueCreationService);
        ThrowIfNull(options);

        var elementType = type.GetGenericArguments()[0];
        var hashSetType = typeof(HashSet<>).MakeGenericType(elementType);
        var hashSet = Activator.CreateInstance(hashSetType) ?? throw new InvalidOperationException($"Unable to create HashSet instance for type {type.FullName}.");
        var addMethod = hashSetType.GetMethod("Add") ?? throw new InvalidOperationException($"Unable to find Add method for type {hashSetType.FullName}.");

        for (var i = 0; i < options.CollectionItemCount; i++)
        {
            var elementValue = valueCreationService.CreateValue(fixture, elementType);
            addMethod.Invoke(hashSet, [elementValue]);
        }

        return hashSet;
    }
}
