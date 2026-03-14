using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

namespace ConstructorCustomization.AutoFixture.Customization.Adapters;

public class DenyCircularDependenyServiceFactory : ICircularDependencyServiceFactory
{
    public ICircularDependencyService Create()
    {
        return new DenyCircularDependencyService();
    }
}