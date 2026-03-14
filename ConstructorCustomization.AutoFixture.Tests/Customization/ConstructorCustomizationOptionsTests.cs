namespace ConstructorCustomization.AutoFixture.Tests.Customization;

[TestFixture]
internal sealed class ConstructorCustomizationOptionsTests
{
    [Test]
    public void Defaults_AreExpected()
    {
        var options = new ConstructorCustomizationOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.CollectionItemCount, Is.EqualTo(3));
            Assert.That(options.PropertyNameComparer, Is.EqualTo(StringComparer.OrdinalIgnoreCase));
        });
    }

    [Test]
    public void InitProperties_AreSettable()
    {
        var options = new ConstructorCustomizationOptions
        {
            CollectionItemCount = 7,
            PropertyNameComparer = StringComparer.Ordinal
        };

        Assert.Multiple(() =>
        {
            Assert.That(options.CollectionItemCount, Is.EqualTo(7));
            Assert.That(options.PropertyNameComparer, Is.EqualTo(StringComparer.Ordinal));
        });
    }
}
