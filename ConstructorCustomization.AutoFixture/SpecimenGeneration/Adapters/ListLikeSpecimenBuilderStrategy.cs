using System.Collections;

using AutoFixture;

using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.SpecimenGeneration.Adapters;

/// <summary>
/// Builds list-like values for supported generic collection interfaces and types.
/// </summary>
internal sealed class ListLikeSpecimenBuilderStrategy : ISpecimenBuilderStrategy
{
    /// <inheritdoc />
    public bool CanBuild(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(List<>) ||
               genericTypeDefinition == typeof(IList<>) ||
               genericTypeDefinition == typeof(IEnumerable<>) ||
               genericTypeDefinition == typeof(ICollection<>) ||
               genericTypeDefinition == typeof(IReadOnlyCollection<>) ||
               genericTypeDefinition == typeof(IReadOnlyList<>);
    }

    /// <inheritdoc />
    public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
    {
        ThrowIfNull(type);
        ThrowIfNull(valueCreationService);
        ThrowIfNull(options);

        var elementType = type.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType)!;

        for (var i = 0; i < options.CollectionItemCount; i++)
        {
            list.Add(valueCreationService.CreateValue(fixture, elementType));
        }

        return list;
    }
}
