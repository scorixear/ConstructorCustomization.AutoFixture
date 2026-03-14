namespace ConstructorCustomization.AutoFixture.Customization.Domain;

public class CircularDependencyException : InvalidOperationException
{
    public CircularDependencyException(string propertyName, string dependencyChain)
        : base($"Circular dependency detected while resolving property '{propertyName}'. Dependency chain: {dependencyChain}")
    {
    }
}
