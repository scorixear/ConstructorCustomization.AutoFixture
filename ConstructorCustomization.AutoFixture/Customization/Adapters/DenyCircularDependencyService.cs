using AutoFixture;

using ConstructorCustomization.AutoFixture.Customization.Application.Ports;
using ConstructorCustomization.AutoFixture.Customization.Domain;

namespace ConstructorCustomization.AutoFixture.Customization.Adapters;

public class DenyCircularDependencyService : ICircularDependencyService
{
    private HashSet<string> CurrentlyResolvedProperties { get; } = new();
    private Stack<string> ResolvedPropertyChain { get; } = new();
    private string? SourcePropertyName { get; set; }

    public void StartResolving(string propertyName)
    {
        CurrentlyResolvedProperties.Add(propertyName);
        ResolvedPropertyChain.Push(propertyName);
        if (SourcePropertyName == null)
        {
            SourcePropertyName = propertyName;
        }
    }

    public void StopResolving(string propertyName)
    {
        if (ResolvedPropertyChain.Count == 0 || ResolvedPropertyChain.Peek() != propertyName)
        {
            throw new InvalidOperationException($"Attempted to stop resolving property '{propertyName}' which is not currently being resolved. Current chain: {string.Join(" -> ", ResolvedPropertyChain.Reverse())}");
        }
        CurrentlyResolvedProperties.Remove(propertyName);
        ResolvedPropertyChain.Pop();
        if (CurrentlyResolvedProperties.Count == 0)
        {
            SourcePropertyName = null;
        }
    }

    public bool CheckCircularDependency(string propertyName)
    {
        if (CurrentlyResolvedProperties.Contains(propertyName))
        {
            return true;
        }

        return false;
    }

    public object? HandleCircularDependency(string propertyName, Func<IFixture, object?> valueFactory)
    {
        var dependencyChain = string.Join(" -> ", ResolvedPropertyChain.Reverse()) + $" -> {propertyName}";
        throw new CircularDependencyException(SourcePropertyName!, dependencyChain);
    }
}
