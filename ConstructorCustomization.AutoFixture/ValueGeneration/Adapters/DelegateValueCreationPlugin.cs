using AutoFixture;

using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.ValueGeneration.Adapters;

internal sealed class DelegateValueCreationPlugin : IValueCreationPlugin
{
    private readonly Func<Type, bool> predicate;
    private readonly Func<Type, IFixture, IValueCreationService, object?> factory;

    internal DelegateValueCreationPlugin(
        Func<Type, bool> predicate,
        Func<Type, IFixture, IValueCreationService, object?> factory)
    {
        this.predicate = predicate;
        this.factory = factory;
    }

    public bool CanCreate(Type type) => predicate(type);

    public object? Create(Type type, IFixture fixture, IValueCreationService valueCreationService)
        => factory(type, fixture, valueCreationService);
}
