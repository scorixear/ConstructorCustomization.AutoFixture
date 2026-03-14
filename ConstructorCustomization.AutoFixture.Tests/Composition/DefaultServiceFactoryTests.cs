namespace ConstructorCustomization.AutoFixture.Tests.Composition;

[TestFixture]
internal sealed class DefaultServiceFactoryTests
{
    [Test]
    public void CreateSpecimenStrategies_ReturnsExpectedOrderedStrategies()
    {
        var strategies = DefaultServiceFactory.CreateSpecimenStrategies();

        Assert.Multiple(() =>
        {
            Assert.That(strategies, Has.Count.EqualTo(4));
            Assert.That(strategies[0], Is.TypeOf<ArraySpecimenBuilderStrategy>());
            Assert.That(strategies[1], Is.TypeOf<ListLikeSpecimenBuilderStrategy>());
            Assert.That(strategies[2], Is.TypeOf<DictionaryLikeSpecimenBuilderStrategy>());
            Assert.That(strategies[3], Is.TypeOf<SetLikeSpecimenBuilderStrategy>());
        });
    }
}
