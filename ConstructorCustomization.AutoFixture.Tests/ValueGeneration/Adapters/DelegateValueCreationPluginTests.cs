namespace ConstructorCustomization.AutoFixture.Tests.ValueGeneration.Adapters;

[TestFixture]
public sealed class DelegateValueCreationPluginTests
{
    [Test]
    public void CanCreate_DelegatesToPredicate()
    {
        var plugin = new DelegateValueCreationPlugin(
            t => t == typeof(int),
            static (_, _, _) => 0);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.CanCreate(typeof(int)), Is.True);
            Assert.That(plugin.CanCreate(typeof(string)), Is.False);
        });
    }

    [Test]
    public void Create_DelegatesToFactory()
    {
        var fixture = new Fixture();
        var service = new ValueCreationService([], [], new ConstructorCustomizationOptions());
        var plugin = new DelegateValueCreationPlugin(
            static _ => true,
            static (type, _, _) => type.Name);

        var value = plugin.Create(typeof(string), fixture, service);

        Assert.That(value, Is.EqualTo("String"));
    }
}
