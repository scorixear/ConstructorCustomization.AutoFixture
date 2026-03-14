namespace ConstructorCustomization.AutoFixture.Customization.Application.Ports;

public interface ICircularDependencyServiceFactory
{
    ICircularDependencyService Create();
}

