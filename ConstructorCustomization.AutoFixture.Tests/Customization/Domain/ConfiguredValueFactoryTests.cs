namespace ConstructorCustomization.AutoFixture.Tests.Customization.Domain;

[TestFixture]
internal sealed class ConfiguredValueFactoryTests
{
    [Test]
    public void Resolve_InvokesResolverWithFixture()
    {
        var fixture = new Fixture();
        var called = false;
        var factory = new ConfiguredValueFactory(f =>
        {
            called = true;
            return ReferenceEquals(f, fixture) ? "ok" : "bad";
        });

        var value = factory.Resolve(fixture);

        Assert.Multiple(() =>
        {
            Assert.That(called, Is.True);
            Assert.That(value, Is.EqualTo("ok"));
        });
    }

    [Test]
    public void Ctor_WhenResolverIsNull_ThrowsArgumentNullException()
    {
        Assert.That(() => new ConfiguredValueFactory(null!), Throws.ArgumentNullException);
    }
}
