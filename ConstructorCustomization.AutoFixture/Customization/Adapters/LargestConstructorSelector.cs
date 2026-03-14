using System.Reflection;

using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

namespace ConstructorCustomization.AutoFixture.Customization.Adapters;

/// <summary>
/// Selects the public constructor with the largest parameter count.
/// </summary>
internal sealed class LargestConstructorSelector : IConstructorSelector
{
    /// <inheritdoc />
    public ConstructorInfo SelectConstructor(Type targetType, ConstructorInfo[] constructors)
    {
        ThrowIfNull(targetType);
        ThrowIfNull(constructors);

        if (constructors.Length == 0)
        {
            throw new InvalidOperationException($"Type {targetType.FullName} does not have any public constructors.");
        }

        return constructors.OrderByDescending(c => c.GetParameters().Length).First();
    }
}
