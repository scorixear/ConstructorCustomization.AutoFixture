namespace ConstructorCustomization.AutoFixture.Tests.Customization.Domain;

[TestFixture]
public sealed class PropertyValueResolutionPolicyTests
{
    private ICircularDependencyService DefaultCircularDependencyService { get; set; } = null!;

    [SetUp]
    public void SetUp()
    {
        DefaultCircularDependencyService = new DenyCircularDependencyService();
    }
    [Test]
    public void TryResolveConfiguredValue_PrioritizesOverrideOverDefault()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();
        overrideStore.SetValue("Name", "override");
        defaultStore.SetValue("Name", "default");

        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            fixture,
            DefaultCircularDependencyService,
            out var value,
            out var source);

        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.EqualTo("override"));
            Assert.That(source, Is.EqualTo(PropertyValueSource.Override));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_UsesDefaultWhenNoOverride()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        defaultStore.SetValue("Name", "default");

        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            new Fixture(),
            DefaultCircularDependencyService,
            out var value,
            out var source);

        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.EqualTo("default"));
            Assert.That(source, Is.EqualTo(PropertyValueSource.Default));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_ReturnsGeneratedWhenNotConfigured()
    {
        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            new PropertyValueStore(StringComparer.OrdinalIgnoreCase),
            new PropertyValueStore(StringComparer.OrdinalIgnoreCase),
            new Fixture(),
            DefaultCircularDependencyService,
            out var value,
            out var source);

        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.False);
            Assert.That(value, Is.Null);
            Assert.That(source, Is.EqualTo(PropertyValueSource.Generated));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_ValidatesArguments()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();

        Assert.Multiple(() =>
        {
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("", overrideStore, defaultStore, fixture, DefaultCircularDependencyService, out _, out _), Throws.ArgumentException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", null!, defaultStore, fixture, DefaultCircularDependencyService, out _, out _), Throws.ArgumentNullException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", overrideStore, null!, fixture, DefaultCircularDependencyService, out _, out _), Throws.ArgumentNullException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", overrideStore, defaultStore, null!, DefaultCircularDependencyService, out _, out _), Throws.ArgumentNullException);
            Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue("Name", overrideStore, defaultStore, fixture, null!, out _, out _), Throws.ArgumentNullException);
        });
    }

    [Test]
    public void TryResolveConfiguredValue_WithCircularDependency_Throws()
    {
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();
        overrideStore.SetValue("Name", new ConfiguredValueFactory(f => f.Create<string>()));
        DefaultCircularDependencyService.StartResolving("Name");

        Assert.That(() => PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            fixture,
            DefaultCircularDependencyService,
            out _,
            out _), Throws.TypeOf<CircularDependencyException>());
    }

    [Test]
    public void TryResolveConfiguredValue_WithCircularDependency_CallsCheck_ThenHandle_ReturnsHandles()
    {
        // Arrange
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();
        overrideStore.SetValue("Name", new ConfiguredValueFactory(f => f.Create<string>()));
        var mockCircularDependencyService = new MockCircularDependencyService();

        // Act
        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            fixture,
            mockCircularDependencyService,
            out var value,
            out var source);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.EqualTo("handled value"));
            Assert.That(source, Is.EqualTo(PropertyValueSource.Override));
            Assert.That(mockCircularDependencyService.CalledMethods, Is.EquivalentTo(new[] { "CheckCircularDependency", "HandleCircularDependency" }));
            Assert.That(mockCircularDependencyService.CheckedProperties, Is.EquivalentTo(new[] { "Name" }));
            Assert.That(mockCircularDependencyService.HandledProperties, Is.EquivalentTo(new[] { "Name" }));
        });
    }

    [Test]
    public void TryResolveConfiguredValue_WithNoCircularDependeny_CallsStartAndStopResolving()
    {
        // Arrange
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();
        overrideStore.SetValue("Name", new ConfiguredValueFactory(f => f.Create<string>()));
        var mockCircularDependencyService = new MockCircularDependencyService();
        mockCircularDependencyService.CheckCircularDependencyReturnValue = false; // Simulate no circular dependency

        // Act
        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            fixture,
            mockCircularDependencyService,
            out var value,
            out var source);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.Not.Null);
            Assert.That(source, Is.EqualTo(PropertyValueSource.Override));
            Assert.That(mockCircularDependencyService.CalledMethods, Is.EquivalentTo(new[] { "CheckCircularDependency", "StartResolving", "StopResolving" }));
            Assert.That(mockCircularDependencyService.CheckedProperties, Is.EquivalentTo(new[] { "Name" }));
            Assert.That(mockCircularDependencyService.HandledProperties, Is.Empty);
        });
    }

    [Test]
    public void TryResolveConfiguredValue_WithNormalValue_DoesNotCallCircularDependencyService()
    {
        // Arrange
        var overrideStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var defaultStore = new PropertyValueStore(StringComparer.OrdinalIgnoreCase);
        var fixture = new Fixture();
        overrideStore.SetValue("Name", "configured value");
        var mockCircularDependencyService = new MockCircularDependencyService();

        // Act
        var resolved = PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            "Name",
            overrideStore,
            defaultStore,
            fixture,
            mockCircularDependencyService,
            out var value,
            out var source);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resolved, Is.True);
            Assert.That(value, Is.EqualTo("configured value"));
            Assert.That(source, Is.EqualTo(PropertyValueSource.Override));
            Assert.That(mockCircularDependencyService.CalledMethods, Is.Empty);
            Assert.That(mockCircularDependencyService.CheckedProperties, Is.Empty);
            Assert.That(mockCircularDependencyService.HandledProperties, Is.Empty);
        });
    }

    private class MockCircularDependencyService : ICircularDependencyService
    {
        public List<string> CheckedProperties { get; } = new();
        public List<string> HandledProperties { get; } = new();
        public List<string> CalledMethods { get; } = new();
        public bool CheckCircularDependencyReturnValue { get; set; } = true;

        public void StartResolving(string propertyName)
        {
            CalledMethods.Add(nameof(StartResolving));
        }

        public void StopResolving(string propertyName)
        {
            CalledMethods.Add(nameof(StopResolving));
        }

        public bool CheckCircularDependency(string propertyName)
        {
            CalledMethods.Add(nameof(CheckCircularDependency));
            CheckedProperties.Add(propertyName);
            return CheckCircularDependencyReturnValue;
        }

        public object? HandleCircularDependency(string propertyName, Func<IFixture, object?> valueFactory)
        {
            CalledMethods.Add(nameof(HandleCircularDependency));
            HandledProperties.Add(propertyName);
            return "handled value"; // Simulate handling circular dependency
        }
    }
}
