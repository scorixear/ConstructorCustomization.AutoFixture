namespace ConstructorCustomization.AutoFixture.Tests.SpecimenGeneration.Adapters;

[TestFixture]
internal sealed class ArraySpecimenBuilderStrategyTests
{
    [Test]
    public void CanBuild_ReturnsTrueForArraysOnly()
    {
        var strategy = new ArraySpecimenBuilderStrategy();

        Assert.Multiple(() =>
        {
            Assert.That(strategy.CanBuild(typeof(int[])), Is.True);
            Assert.That(strategy.CanBuild(typeof(List<int>)), Is.False);
        });
    }

    [Test]
    public void Build_CreatesArrayWithExpectedLengthAndValues()
    {
        var strategy = new ArraySpecimenBuilderStrategy();
        var service = new SequenceValueCreationService([1, 2, 3]);
        var options = new ConstructorCustomizationOptions { CollectionItemCount = 3 };

        var result = (int[])strategy.Build(typeof(int[]), new Fixture(), service, options)!;

        Assert.That(result, Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Build_ValidatesArguments()
    {
        var strategy = new ArraySpecimenBuilderStrategy();

        Assert.Multiple(() =>
        {
            Assert.That(() => strategy.Build(null!, new Fixture(), new SequenceValueCreationService([]), new ConstructorCustomizationOptions()), Throws.ArgumentNullException);
            Assert.That(() => strategy.Build(typeof(int[]), new Fixture(), null!, new ConstructorCustomizationOptions()), Throws.ArgumentNullException);
            Assert.That(() => strategy.Build(typeof(int[]), new Fixture(), new SequenceValueCreationService([]), null!), Throws.ArgumentNullException);
        });
    }

    [Test]
    public void Build_WhenCollectionItemCountIsZero_ReturnsEmptyArray()
    {
        var strategy = new ArraySpecimenBuilderStrategy();
        var result = (int[])strategy.Build(
            typeof(int[]),
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
