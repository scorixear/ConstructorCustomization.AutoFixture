namespace ConstructorCustomization.AutoFixture.Tests.Customization.Domain;

[TestFixture]
public sealed class CustomizationDomainOptionsTests
{
    [Test]
    public void From_WhenOptionsValid_CopiesValues()
    {
        var source = new ConstructorCustomizationOptions
        {
            CollectionItemCount = 5,
            PropertyNameComparer = StringComparer.Ordinal
        };

        var options = CustomizationDomainOptions.From(source);

        Assert.Multiple(() =>
        {
            Assert.That(options.CollectionItemCount, Is.EqualTo(5));
            Assert.That(options.PropertyNameComparer, Is.EqualTo(StringComparer.Ordinal));
        });
    }

    [Test]
    public void From_WhenCollectionItemCountNegative_ThrowsArgumentOutOfRangeException()
    {
        var source = new ConstructorCustomizationOptions { CollectionItemCount = -1 };

        Assert.That(() => CustomizationDomainOptions.From(source), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void From_WhenOptionsNull_ThrowsArgumentNullException()
    {
        Assert.That(() => CustomizationDomainOptions.From(null!), Throws.ArgumentNullException);
    }
}
