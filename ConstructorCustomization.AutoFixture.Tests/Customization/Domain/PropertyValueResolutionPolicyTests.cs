namespace ConstructorCustomization.AutoFixture.Tests.Customization.Domain;

[TestFixture]
internal sealed class PropertyValueResolutionPolicyTests
{
    [Test]
    public void TryResolveConfiguredValue_PrioritizesOverrideOverDefault()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();
        overrideStore.SetValue("Name", "override");
        defaultStore.SetValue("Name", "default");

        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            fixture,
            static (v, _) => v,
            out var value,
            out var source);

        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.EqualTo("override"));
            Assert.That(source, Is.EqualTo(PropertyValueSource.Override));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_UsesDefaultWhenNoOverride()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        defaultStore.SetValue("Name", "default");

        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            new Fixture(),
            static (v, _) => v,
            out var value,
            out var source);

        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.EqualTo("default"));
            Assert.That(source, Is.EqualTo(PropertyValueSource.Default));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_ReturnsGeneratedWhenNotConfigured()
    {
        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            new PropertyValueStore(StringComparer.OrdinalIgnoreCase),
            new PropertyValueStore(StringComparer.OrdinalIgnoreCase),
            new Fixture(),
            static (v, _) => v,
            out var value,
            out var source);

        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.False);
            Assert.That(value, Is.Null);
            Assert.That(source, Is.EqualTo(PropertyValueSource.Generated));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_ValidatesArguments()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();

        Assert.Multiple(() =>
        {
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("", overrideStore, defaultStore, fixture, static (v, _) => v, out _, out _), Throws.ArgumentException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", null!, defaultStore, fixture, static (v, _) => v, out _, out _), Throws.ArgumentNullException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", overrideStore, null!, fixture, static (v, _) => v, out _, out _), Throws.ArgumentNullException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", overrideStore, defaultStore, null!, static (v, _) => v, out _, out _), Throws.ArgumentNullException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", overrideStore, defaultStore, fixture, null!, out _, out _), Throws.ArgumentNullException);
        });
    }
}
