using System.Collections;

namespace ConstructorCustomization.AutoFixture.Tests.SpecimenGeneration.Adapters;

[TestFixture]
internal sealed class ListLikeSpecimenBuilderStrategyTests
{
    [TestCase(typeof(List<int>))]
    [TestCase(typeof(IList<int>))]
    [TestCase(typeof(IEnumerable<int>))]
    [TestCase(typeof(ICollection<int>))]
    [TestCase(typeof(IReadOnlyCollection<int>))]
    [TestCase(typeof(IReadOnlyList<int>))]
    public void CanBuild_SupportedGenericShapes_ReturnsTrue(Type type)
    {
        var strategy = new ListLikeSpecimenBuilderStrategy();

        Assert.That(strategy.CanBuild(type), Is.True);
    }

    [Test]
    public void CanBuild_UnsupportedTypes_ReturnsFalse()
    {
        var strategy = new ListLikeSpecimenBuilderStrategy();

        Assert.Multiple(() =>
        {
            Assert.That(strategy.CanBuild(typeof(int)), Is.False);
            Assert.That(strategy.CanBuild(typeof(Dictionary<string, int>)), Is.False);
        });
    }

    [Test]
    public void Build_CreatesListWithConfiguredItemCount()
    {
        var strategy = new ListLikeSpecimenBuilderStrategy();
        var service = new SequenceValueCreationService(["a", "b"]);
        var options = new ConstructorCustomizationOptions { CollectionItemCount = 2 };

        var result = (IList)strategy.Build(typeof(IReadOnlyList<string>), new Fixture(), service, options)!;

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Cast<string>(), Is.EqualTo(new[] { "a", "b" }));
        });
    }

    private sealed class SequenceValueCreationService(IEnumerable<object?> values) : IValueCreationService
    {
        private readonly Queue<object?> queue = new(values);

        public object? CreateValue(IFixture fixture, ParameterInfo parameter) => throw new NotSupportedException();

        public object? CreateValue(IFixture fixture, Type type) => queue.Dequeue();
    }
}
