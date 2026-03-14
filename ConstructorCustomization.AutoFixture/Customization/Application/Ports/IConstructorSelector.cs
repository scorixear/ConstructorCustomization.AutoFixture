using System.Reflection;

namespace ConstructorCustomization.AutoFixture.Customization.Application.Ports;

/// <summary>
/// Selects the constructor used to create an instance of a target type.
/// </summary>
public interface IConstructorSelector
{
    /// <summary>
    /// Selects a constructor from the available public constructors.
    /// </summary>
    /// <param name="targetType">The type being instantiated.</param>
    /// <param name="constructors">The available public constructors for the target type.</param>
    /// <returns>The constructor that should be used to create the instance.</returns>
    ConstructorInfo SelectConstructor(Type targetType, ConstructorInfo[] constructors);
}
