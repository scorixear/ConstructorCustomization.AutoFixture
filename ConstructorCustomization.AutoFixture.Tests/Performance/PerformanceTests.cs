using System.Diagnostics;

namespace ConstructorCustomization.AutoFixture.Tests.Performance;

public class PerformanceTests
{
    [Test]
    public void DirectUsage_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new ConstructorCustomization<SimpleObject>();
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Direct usage - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void DirectUsage_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ConstructorCustomization<ComplexObject>();
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Direct usage - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void WithCustomization_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new ConstructorCustomization<SimpleObject>();
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        customization.With(x => x.Value, 42);
        watch.Stop();
        TestContext.WriteLine($"With customization - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void WithCustomization_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ConstructorCustomization<ComplexObject>();
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        customization.With(x => x.Value, 42)
                   .With(x => x.Text, "Hello World")
                   .With(x => x.Date, new DateTime(2024, 1, 1))
                   .With(x => x.Items, new List<string> { "Item1", "Item2", "Item3" })
                   .With(x => x.Mapping, new Dictionary<string, int> { { "Key1", 1 }, { "Key2", 2 } });
        watch.Stop();
        TestContext.WriteLine($"With customization - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void SetDefaultCustomization_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new SimpleProbeCustomization()
        {
            BeforeCreate = (c, f) =>
            {
                c.SetDefaultAction(x => x.Value, 42);
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Set default customization - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void SetDefaultCustomization_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ComplexProbeCustomization()
        {
            BeforeCreate = (c, f) =>
            {
                c.SetDefaultAction(x => x.Value, 42);
                c.SetDefaultAction(x => x.Text, "Hello World");
                c.SetDefaultAction(x => x.Date, new DateTime(2024, 1, 1));
                c.SetDefaultAction(x => x.Items, new List<string> { "Item1", "Item2", "Item3" });
                c.SetDefaultAction(x => x.Mapping, new Dictionary<string, int> { { "Key1", 1 }, { "Key2", 2 } });
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Set default customization - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void SetDefaultFactory_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new SimpleProbeCustomization()
        {
            BeforeCreate = (c, f) =>
            {
                c.SetDefaultAction(x => x.Value, () => 42);
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Set default factory - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void SetDefaultFactory_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ComplexProbeCustomization()
        {
            BeforeCreate = (c, f) =>
            {
                c.SetDefaultAction(x => x.Value, () => 42);
                c.SetDefaultAction(x => x.Text, () => "Hello World");
                c.SetDefaultAction(x => x.Date, () => new DateTime(2024, 1, 1));
                c.SetDefaultAction(x => x.Items, () => new List<string> { "Item1", "Item2", "Item3" });
                c.SetDefaultAction(x => x.Mapping, () => new Dictionary<string, int> { { "Key1", 1 }, { "Key2", 2 } });
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Set default factory - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void UsePluginCustomization_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new SimpleProbeCustomization()
        {
            ConfigureAction = (c) =>
            {
                c.UsePluginAction((type) => type == typeof(int), (type, fixture, service) => 42);
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Use plugin customization - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void UsePluginCustomization_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ComplexProbeCustomization()
        {
            ConfigureAction = (c) =>
            {
                c.UsePluginAction((type) => type == typeof(int), (type, fixture, service) => 42);
                c.UsePluginAction((type) => type == typeof(string), (type, fixture, service) => "Hello World");
                c.UsePluginAction((type) => type == typeof(DateTime), (type, fixture, service) => new DateTime(2024, 1, 1));
                c.UsePluginAction((type) => type == typeof(List<string>), (type, fixture, service) => new List<string> { "Item1", "Item2", "Item3" });
                c.UsePluginAction((type) => type == typeof(Dictionary<string, int>), (type, fixture, service) => new Dictionary<string, int> { { "Key1", 1 }, { "Key2", 2 } });
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Use plugin customization - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void UseValueFor_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new SimpleProbeCustomization()
        {
            ConfigureAction = (c) =>
             {
                 c.UseValueForAction<int>((f) => 42);
             }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Use value for - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);
        Assert.Pass();
    }

    [Test]
    public void UseValueFor_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ComplexProbeCustomization()
        {
            ConfigureAction = (c) =>
            {
                c.UseValueForAction<int>((f) => 42);
                c.UseValueForAction<string>((f) => "Hello World");
                c.UseValueForAction<DateTime>((f) => new DateTime(2024, 1, 1));
                c.UseValueForAction<List<string>>((f) => new List<string> { "Item1", "Item2", "Item3" });
                c.UseValueForAction<Dictionary<string, int>>((f) => new Dictionary<string, int> { { "Key1", 1 }, { "Key2", 2 } });
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Use value for - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void UseStrategy_SimpleObject()
    {
        var fixture = new Fixture();
        var customization = new SimpleProbeCustomization()
        {
            ConfigureAction = (c) =>
            {
                c.UseStrategyAction(new SimpleObjectBuilder());
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Use strategy - SimpleObject: {watch.ElapsedTicks} ticks");
        CreateObject<SimpleObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void UseStrategy_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ComplexProbeCustomization()
        {
            ConfigureAction = (c) =>
            {
                c.UseStrategyAction(new ComplexObjectBuilder());
            }
        }
        ;
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        watch.Stop();
        TestContext.WriteLine($"Use strategy - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);

        Assert.Pass();
    }

    [Test]
    public void Combined_ComplexObject()
    {
        var fixture = new Fixture();
        var customization = new ComplexProbeCustomization()
        {
            ConfigureAction = (c) =>
            {
                c.UsePluginAction((type) => type == typeof(int), (type, fixture, service) => 42);
                c.UseValueForAction<string>((f) => "Hello World");
            },
            BeforeCreate = (c, f) =>
            {
                c.SetDefaultAction(x => x.Date, new DateTime(2024, 1, 1));
                c.SetDefaultAction(x => x.Items, new List<string> { "Item1", "Item2", "Item3" });
                c.SetDependencyDefaultAction(x => x.Mapping, x => x.Items, (items) => items.ToDictionary(item => item, item => item.Length));
            }
        };
        var watch = Stopwatch.StartNew();
        customization.Customize(fixture);
        customization.With(x => x.Date, new DateTime(2024, 1, 1));
        watch.Stop();
        TestContext.WriteLine($"Combined customization - ComplexObject: {watch.ElapsedTicks} ticks");
        CreateObject<ComplexObject>(fixture);
    }

    private void CreateObject<T>(Fixture fixture)
    {
        var watch = Stopwatch.StartNew();
        long averageTime = 1;
        long peakTime = 0;
        long minimumTime = long.MaxValue;
        long totalTime = 0;
        for (int i = 0; i < 1000; i++)
        {
            watch.Restart();
            fixture.Create<T>();
            watch.Stop();
            averageTime += watch.ElapsedTicks;
            if (watch.ElapsedTicks > peakTime)
                peakTime = watch.ElapsedTicks;
            if (watch.ElapsedTicks < minimumTime)
                minimumTime = watch.ElapsedTicks;
            totalTime += watch.ElapsedTicks;
        }
        averageTime /= 1000;
        TestContext.WriteLine($"[Average]: {averageTime} ticks");
        TestContext.WriteLine($"[Peak]:    {peakTime} ticks");
        TestContext.WriteLine($"[Minimum]: {minimumTime} ticks");
        TestContext.WriteLine($"[Total]:   {totalTime} ticks");
    }

    private sealed class SimpleProbeCustomization :
    ConstructorCustomization<SimpleObject, SimpleProbeCustomization>
    {
        public Action<SimpleProbeCustomization> ConfigureAction { get; set; } = _ => { };
        public Action<SimpleProbeCustomization, IFixture> BeforeCreate { get; set; } = (_, __) => { };

        protected override void Configure()
        {
            ConfigureAction.Invoke(this);
            base.Configure();
        }

        protected override SimpleObject CreateInstance(IFixture fixture)
        {
            BeforeCreate.Invoke(this, fixture);
            return base.CreateInstance(fixture);
        }

        public void SetDefaultAction<TProperty>(Expression<Func<SimpleObject, TProperty>> propertyExpression, TProperty value) => SetDefault(propertyExpression, value);
        public void SetDefaultAction<TProperty>(Expression<Func<SimpleObject, TProperty>> propertyExpression, Func<TProperty> valueFactory) => SetDefault(propertyExpression, valueFactory);
        public void SetDefaultAction<TProperty>(Expression<Func<SimpleObject, TProperty>> propertyExpression, Func<IFixture, TProperty> valueFactory) => SetDefault(propertyExpression, valueFactory);
        public void SetDependencyDefaultAction<TProperty, TDependency>(Expression<Func<SimpleObject, TProperty>> propertyExpression,
        Expression<Func<SimpleObject, TDependency>> dependencyExpression,
        Func<TDependency, TProperty> valueFactory) => SetDependencyDefault(propertyExpression, dependencyExpression, valueFactory);

        public void UsePluginAction(Func<Type, bool> predicate, Func<Type, IFixture, IValueCreationService, object?> factory) => UsePlugin(predicate, factory);
        public void UsePluginAction(IValueCreationPlugin plugin) => UsePlugin(plugin);
        public void UseValueForAction<TType>(Func<IFixture, TType?> factory) => UseValueFor(factory);
        public void UseStrategyAction(ISpecimenBuilderStrategy strategy) => UseStrategy(strategy);
    }

    private sealed class SimpleObjectBuilder : ISpecimenBuilderStrategy
    {
        public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
        {
            if (type == typeof(int))
                return 42;
            return null;
        }

        public bool CanBuild(Type type)
        {
            return type == typeof(int);
        }
    }

    private sealed class SimpleObject(int value)
    {
        public int Value { get; } = value;
    }

    private sealed class ComplexProbeCustomization :
    ConstructorCustomization<ComplexObject, ComplexProbeCustomization>
    {
        public Action<ComplexProbeCustomization> ConfigureAction { get; set; } = _ => { };
        public Action<ComplexProbeCustomization, IFixture> BeforeCreate { get; set; } = (_, __) => { };

        protected override void Configure()
        {
            ConfigureAction.Invoke(this);
            base.Configure();
        }

        protected override ComplexObject CreateInstance(IFixture fixture)
        {
            BeforeCreate.Invoke(this, fixture);
            return base.CreateInstance(fixture);
        }

        public void SetDefaultAction<TProperty>(Expression<Func<ComplexObject, TProperty>> propertyExpression, TProperty value) => SetDefault(propertyExpression, value);
        public void SetDefaultAction<TProperty>(Expression<Func<ComplexObject, TProperty>> propertyExpression, Func<TProperty> valueFactory) => SetDefault(propertyExpression, valueFactory);
        public void SetDefaultAction<TProperty>(Expression<Func<ComplexObject, TProperty>> propertyExpression, Func<IFixture, TProperty> valueFactory) => SetDefault(propertyExpression, valueFactory);
        public void SetDependencyDefaultAction<TProperty, TDependency>(Expression<Func<ComplexObject, TProperty>> propertyExpression,
        Expression<Func<ComplexObject, TDependency>> dependencyExpression,
        Func<TDependency, TProperty> valueFactory) => SetDependencyDefault(propertyExpression, dependencyExpression, valueFactory);

        public void UsePluginAction(Func<Type, bool> predicate, Func<Type, IFixture, IValueCreationService, object?> factory) => UsePlugin(predicate, factory);
        public void UsePluginAction(IValueCreationPlugin plugin) => UsePlugin(plugin);
        public void UseValueForAction<TType>(Func<IFixture, TType?> factory) => UseValueFor(factory);
        public void UseStrategyAction(ISpecimenBuilderStrategy strategy) => UseStrategy(strategy);
    }

    private sealed class ComplexObjectBuilder : ISpecimenBuilderStrategy
    {
        public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
        {
            if (type == typeof(int))
                return 42;
            if (type == typeof(string))
                return "Hello World";
            if (type == typeof(DateTime))
                return new DateTime(2024, 1, 1);
            if (type == typeof(List<string>))
                return new List<string> { "Item1", "Item2", "Item3" };
            if (type == typeof(Dictionary<string, int>))
                return new Dictionary<string, int> { { "Key1", 1 }, { "Key2", 2 } };
            return null;
        }

        public bool CanBuild(Type type)
        {
            return type == typeof(int) || type == typeof(string) || type == typeof(DateTime) || type == typeof(List<string>) || type == typeof(Dictionary<string, int>);
        }
    }

    private sealed class ComplexObject(int value, string text, DateTime date, List<string> items, Dictionary<string, int> mapping)
    {
        public int Value { get; } = value;
        public string Text { get; } = text;
        public DateTime Date { get; } = date;
        public List<string> Items { get; } = items;
        public Dictionary<string, int> Mapping { get; } = mapping;
    }
}
