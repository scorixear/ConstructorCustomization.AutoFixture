namespace ConstructorCustomization.AutoFixture.Tests.SpecimenGeneration.Adapters;

[TestFixture]
internal sealed class SetLikeSpecimenBuilderStrategyTests
{
    [TestCase(typeof(HashSet<int>))]
    [TestCase(typeof(ISet<int>))]
    public void CanBuild_SupportedGenericShapes_ReturnsTrue(Type type)
    {
        var strategy = new SetLikeSpecimenBuilderStrategy();

        Assert.That(strategy.CanBuild(type), Is.True);
    }

    [Test]
    public void CanBuild_UnsupportedTypes_ReturnsFalse()
    {
        var strategy = new SetLikeSpecimenBuilderStrategy();

        Assert.Multiple(() =>
        {
            Assert.That(strategy.CanBuild(typeof(int)), Is.False);
            Assert.That(strategy.CanBuild(typeof(List<int>)), Is.False);
        });
    }

    [Test]
    public void Build_CreatesHashSetWithConfiguredElements()
    {
        var strategy = new SetLikeSpecimenBuilderStrategy();
        var service = new SequenceValueCreationService(new object?[] { 1, 2, 3 });
        var options = new ConstructorCustomizationOptions { CollectionItemCount = 3 };

        var result = (HashSet<int>)strategy.Build(typeof(HashSet<int>), new Fixture(), service, options)!;

        Assert.That(result.SetEquals(new[] { 1, 2, 3 }), Is.True);
    }

    [Test]
    public void Build_ValidatesArguments()
    {
        var strategy = new SetLikeSpecimenBuilderStrategy();

        Assert.Multiple(() =>
        {
            Assert.That(() => strategy.Build(null!, new Fixture(), new SequenceValueCreationService([]), new ConstructorCustomizationOptions()), Throws.ArgumentNullException);
            Assert.That(() => strategy.Build(typeof(HashSet<int>), new Fixture(), null!, new ConstructorCustomizationOptions()), Throws.ArgumentNullException);
            Assert.That(() => strategy.Build(typeof(HashSet<int>), new Fixture(), new SequenceValueCreationService([]), null!), Throws.ArgumentNullException);
        });
    }

    [Test]
    public void Build_WhenCollectionItemCountIsZero_ReturnsEmptySet()
    {
        var strategy = new SetLikeSpecimenBuilderStrategy();
        var result = (HashSet<int>)strategy.Build(
            typeof(HashSet<int>),
            new Fixture(),
            new SequenceValueCreationService([]),
            new ConstructorCustomizationOptions { CollectionItemCount = 0 })!;

        Assert.That(result, Is.Empty);
    }

    private sealed class SequenceValueCreationService(IEnumerable<object?> values) : IValueCreationService
    {
        private readonly Queue<object?> queue = new(values);

        public object? CreateValue(IFixture fixture, ParameterInfo parameter) => throw new NotSupportedException();

        public object? CreateValue(IFixture fixture, Type type) => queue.Dequeue();
    }
}
