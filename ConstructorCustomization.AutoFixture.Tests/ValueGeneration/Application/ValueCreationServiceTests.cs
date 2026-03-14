using ConstructorCustomization.AutoFixture.Tests.Shared;

namespace ConstructorCustomization.AutoFixture.Tests.ValueGeneration.Application;

[TestFixture]
public sealed class ValueCreationServiceTests
{
    [Test]
    public void CreateValue_UsesPluginBeforeStrategies()
    {
        var plugin = new StubPlugin(typeof(string), "plugin-value");
        var strategy = new StubStrategy(typeof(string), "strategy-value");
        var service = new ValueCreationService([plugin], [strategy], new ConstructorCustomizationOptions());

        var value = service.CreateValue(new Fixture(), typeof(string));

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo("plugin-value"));
            Assert.That(strategy.BuildCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public void CreateValue_UsesStrategyWhenNoPluginMatches()
    {
        var strategy = new StubStrategy(typeof(int), 42);
        var service = new ValueCreationService([], [strategy], new ConstructorCustomizationOptions());

        var value = service.CreateValue(new Fixture(), typeof(int));

        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public void CreateValue_FallsBackToFixtureWhenNoPluginOrStrategyMatches()
    {
        var fixture = new Fixture();
        var service = new ValueCreationService([], [], new ConstructorCustomizationOptions());

        var value = service.CreateValue(fixture, typeof(Guid));

        Assert.That(value, Is.TypeOf<Guid>());
    }

    [Test]
    public void CreateValue_ForParameter_UsesParameterType()
    {
        var strategy = new StubStrategy(typeof(string), "from-parameter");
        var service = new ValueCreationService([], [strategy], new ConstructorCustomizationOptions());
        var parameter = typeof(CtorParameterCarrier).GetConstructors()[0].GetParameters().Single(p => p.Name == "firstName");

        var value = service.CreateValue(new Fixture(), parameter);

        Assert.That(value, Is.EqualTo("from-parameter"));
    }

    [Test]
    public void Ctor_ValidatesArguments()
    {
        var options = new ConstructorCustomizationOptions();

        Assert.Multiple(() =>
        {
            Assert.That(() => new ValueCreationService(null!, [], options), Throws.ArgumentNullException);
            Assert.That(() => new ValueCreationService([], null!, options), Throws.ArgumentNullException);
            Assert.That(() => new ValueCreationService([], [], null!), Throws.ArgumentNullException);
        });
    }

    [Test]
    public void CreateValue_ValidatesArguments()
    {
        var service = new ValueCreationService([], [], new ConstructorCustomizationOptions());
        var parameter = typeof(CtorParameterCarrier).GetConstructors()[0].GetParameters()[0];

        Assert.Multiple(() =>
        {
            Assert.That(() => service.CreateValue(null!, typeof(string)), Throws.ArgumentNullException);
            Assert.That(() => service.CreateValue(new Fixture(), (Type)null!), Throws.ArgumentNullException);
            Assert.That(() => service.CreateValue(new Fixture(), (ParameterInfo)null!), Throws.ArgumentNullException);
            Assert.That(() => service.CreateValue((IFixture)null!, parameter), Throws.ArgumentNullException);
        });
    }

    private sealed class StubPlugin(Type matchType, object? value) : IValueCreationPlugin
    {
        public bool CanCreate(Type type) => type == matchType;

        public object? Create(Type type, IFixture fixture, IValueCreationService valueCreationService) => value;
    }

    private sealed class StubStrategy(Type matchType, object? value) : ISpecimenBuilderStrategy
    {
        public int BuildCallCount { get; private set; }

        public bool CanBuild(Type type) => type == matchType;

        public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
        {
            BuildCallCount++;
            return value;
        }
    }
}
