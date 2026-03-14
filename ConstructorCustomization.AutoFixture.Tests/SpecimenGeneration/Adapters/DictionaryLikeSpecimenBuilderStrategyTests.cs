using System.Collections;

namespace ConstructorCustomization.AutoFixture.Tests.SpecimenGeneration.Adapters;

[TestFixture]
public sealed class DictionaryLikeSpecimenBuilderStrategyTests
{
    [TestCase(typeof(Dictionary<string, int>))]
    [TestCase(typeof(IDictionary<string, int>))]
    [TestCase(typeof(IReadOnlyDictionary<string, int>))]
    public void CanBuild_SupportedGenericShapes_ReturnsTrue(Type type)
    {
        var strategy = new DictionaryLikeSpecimenBuilderStrategy();

        Assert.That(strategy.CanBuild(type), Is.True);
    }

    [Test]
    public void CanBuild_UnsupportedTypes_ReturnsFalse()
    {
        var strategy = new DictionaryLikeSpecimenBuilderStrategy();

        Assert.Multiple(() =>
        {
            Assert.That(strategy.CanBuild(typeof(int)), Is.False);
            Assert.That(strategy.CanBuild(typeof(List<int>)), Is.False);
        });
    }

    [Test]
    public void Build_CreatesDictionaryWithConfiguredEntries()
    {
        var strategy = new DictionaryLikeSpecimenBuilderStrategy();
        var options = new ConstructorCustomizationOptions { CollectionItemCount = 2 };
        var service = new DictionarySequenceValueService(new object?[] { "k1", 1, "k2", 2 });

        var result = (IDictionary)strategy.Build(typeof(Dictionary<string, int>), new Fixture(), service, options)!;

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["k1"], Is.EqualTo(1));
            Assert.That(result["k2"], Is.EqualTo(2));
        });
    }

    private sealed class DictionarySequenceValueService(IEnumerable<object?> values) : IValueCreationService
    {
        private readonly Queue<object?> queue = new(values);

        public object? CreateValue(IFixture fixture, ParameterInfo parameter) => throw new NotSupportedException();

        public object? CreateValue(IFixture fixture, Type type) => queue.Dequeue();
    }
}
