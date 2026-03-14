namespace ConstructorCustomization.AutoFixture.Tests.Customization.Adapters;

[TestFixture]
internal sealed class PropertyValueStoreTests
{
    [Test]
    public void SetAndTryGetValue_RoundTripsValue()
    {
        var store = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);

        store.SetValue("Name", "Alice");

        var found = store.TryGetValue("name", out var value);

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.True);
            Assert.That(value, Is.EqualTo("Alice"));
            Assert.That(store.Contains("NAME"), Is.True);
        });
    }

    [Test]
    public void RemoveValue_ReturnsExpectedState()
    {
        var store = new PropertyValueStore(StringComparer.Ordinal);
        store.SetValue("Name", "Alice");

        var removed = store.RemoveValue("Name");
        var removedMissing = store.RemoveValue("Name");

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(removedMissing, Is.False);
            Assert.That(store.Contains("Name"), Is.False);
        });
    }

    [Test]
    public void Clear_RemovesAllValues()
    {
        var store = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        store.SetValue("A", 1);
        store.SetValue("B", null);

        store.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(store.PropertyNames, Is.Empty);
            Assert.That(store.Contains("A"), Is.False);
            Assert.That(store.Contains("B"), Is.False);
        });
    }
}
