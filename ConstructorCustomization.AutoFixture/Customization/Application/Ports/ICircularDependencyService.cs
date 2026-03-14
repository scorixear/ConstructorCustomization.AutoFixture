using AutoFixture;

namespace ConstructorCustomization.AutoFixture.Customization.Application.Ports;

public interface ICircularDependencyService
{
    void StartResolving(string propertyName);
    void StopResolving(string propertyName);
    bool CheckCircularDependency(string propertyName);
    object? HandleCircularDependency(string propertyName, Func<IFixture, object?> valueFactory);
}
