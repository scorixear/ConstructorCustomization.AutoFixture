namespace ConstructorCustomization.AutoFixture.Tests.Customization.Adapters;

public class DenyCircularDependencyServiceTest
{
    [Test]
    public void HandleCircularDependency_EmptyService_DoesNotThrow()
    {
        // Arrange
        var service = new DenyCircularDependencyService();

        // Act & Assert
        Assert.That(() => service.CheckCircularDependency("PropertyA"), Is.False);
    }

    [Test]
    public void HandleCircularDependency_WithResolvingChain_Resolves_DoesNotThrow()
    {
        // Arrange
        var service = new DenyCircularDependencyService();
        service.StartResolving("PropertyA");
        service.StartResolving("PropertyB");

        // Act & Assert
        Assert.That(() => service.CheckCircularDependency("PropertyC"), Is.False);
    }

    [Test]
    public void HandleCircularDependency_WithResolvingChain_AndUnresolves_DoesNotThrow()
    {
        // Arrange
        var service = new DenyCircularDependencyService();
        service.StartResolving("PropertyA");
        service.StartResolving("PropertyB");
        service.StopResolving("PropertyB");

        // Act & Assert
        Assert.That(() => service.CheckCircularDependency("PropertyB"), Is.False);
    }

    [Test]
    public void HandleCircularDependency_WithResolvingChain_AndCircularDependency_Throws()
    {
        // Arrange
        var service = new DenyCircularDependencyService();
        service.StartResolving("PropertyA");
        service.StartResolving("PropertyB");

        // Act & Assert
        Assert.That(() => service.CheckCircularDependency("PropertyA"), Is.True);
        var ex = Assert.Throws<CircularDependencyException>(() => service.HandleCircularDependency("PropertyA", (_) => null));
        Assert.That(ex!.Message, Does.Contain("PropertyA -> PropertyB -> PropertyA"));
    }

    [Test]
    public void HandleCircularDependency_WithDeepCircularDependency_Throws()
    {
        // Arrange
        var service = new DenyCircularDependencyService();
        service.StartResolving("PropertyA");
        service.StartResolving("PropertyB");
        service.StartResolving("PropertyC");

        // Act & Assert
        Assert.That(() => service.CheckCircularDependency("PropertyA"), Is.True);
        var ex = Assert.Throws<CircularDependencyException>(() => service.HandleCircularDependency("PropertyA", (_) => null));
        Assert.That(ex!.Message, Does.Contain("PropertyA -> PropertyB -> PropertyC -> PropertyA"));
    }

    [Test]
    public void StopResolving_WithMismatchedProperty_Throws()
    {
        // Arrange
        var service = new DenyCircularDependencyService();
        service.StartResolving("PropertyA");

        // Act & Assert
        Assert.That(() => service.StopResolving("PropertyB"), Throws.InvalidOperationException);
        var ex = Assert.Throws<InvalidOperationException>(() => service.StopResolving("PropertyB"));
    }
}
