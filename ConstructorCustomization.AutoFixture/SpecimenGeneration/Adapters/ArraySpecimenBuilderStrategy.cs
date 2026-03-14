using AutoFixture;

using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.SpecimenGeneration.Adapters;

/// <summary>
/// Builds array instances and populates them with generated values.
/// </summary>
internal sealed class ArraySpecimenBuilderStrategy : ISpecimenBuilderStrategy
{
    /// <inheritdoc />
    public bool CanBuild(Type type) => type.IsArray;

    /// <inheritdoc />
    public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(valueCreationService);
        ArgumentNullException.ThrowIfNull(options);

        var elementType = type.GetElementType() ?? throw new InvalidOperationException($"Unable to resolve array element type for {type.FullName}.");
        var array = Array.CreateInstance(elementType, options.CollectionItemCount);

        for (var i = 0; i < options.CollectionItemCount; i++)
        {
            array.SetValue(valueCreationService.CreateValue(fixture, elementType), i);
        }

        return array;
    }
}
