using ConstructorCustomization.AutoFixture.Tests.Shared;

namespace ConstructorCustomization.AutoFixture.Tests.Customization;

[TestFixture]
internal sealed class ConstructorCustomizationTests
{
    [Test]
    public void ConvenienceCustomization_CanBeUsedDirectly()
    {
        var fixture = new Fixture();
        var customization = new ConstructorCustomization<PersonModel>()
            .With(p => p.Name, "direct")
            .With(p => p.Age, 10)
            .With(p => p.City, "Oslo");

        customization.Customize(fixture);
        var created = fixture.Create<PersonModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("direct"));
            Assert.That(created.Age, Is.EqualTo(10));
            Assert.That(created.City, Is.EqualTo("Oslo"));
        });
    }

    [Test]
    public void WithWithoutAndClear_ManageOverrides()
    {
        var customization = new ProbeCustomization()
            .With(p => p.Name, "first")
            .Without(p => p.City)
            .With(p => p.Age, 30);

        Assert.Multiple(() =>
        {
            Assert.That(customization.HasValue(p => p.Name), Is.True);
            Assert.That(customization.GetValue(p => p.City), Is.Null);
            Assert.That(customization.GetValue(p => p.Age), Is.EqualTo(30));
        });

        customization.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(customization.HasValue(p => p.Name), Is.False);
            Assert.That(customization.HasValue(p => p.City), Is.False);
            Assert.That(customization.HasValue(p => p.Age), Is.False);
        });
    }

    [Test]
    public void Customize_UsesDefaultsAndOverridesWithCorrectPrecedence()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            BeforeCreate = c => c.SetDefaultValue(p => p.Name, "default-name")
        }
        .With(p => p.Name, "override-name")
        .With(p => p.Age, 40)
        .With(p => p.City, "Berlin");

        customization.Customize(fixture);

        var created = fixture.Create<PersonModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("override-name"));
            Assert.That(created.Age, Is.EqualTo(40));
            Assert.That(created.City, Is.EqualTo("Berlin"));
        });
    }

    [Test]
    public void Customize_WhenUsingPropertyValueStoreFactory_MigratesExistingOverrides()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c => c.UseStoreFactory(() => new PropertyValueStore(StringComparer.OrdinalIgnoreCase))
        }
        .With(p => p.Name, "migrated")
        .With(p => p.Age, 27)
        .With(p => p.City, "Rome");

        customization.Customize(fixture);

        var created = fixture.Create<PersonModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("migrated"));
            Assert.That(created.Age, Is.EqualTo(27));
            Assert.That(created.City, Is.EqualTo("Rome"));
        });
    }

    [Test]
    public void Customize_ClearsAndRebuildsPluginAndStrategyRegistrationsPerRun()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c =>
            {
                c.UsePluginInternal(new SimplePlugin(typeof(string), "from-plugin"));
                c.UseStrategyInternal(new SimpleStrategy(typeof(string), "from-strategy"));
            }
        };

        customization.Customize(fixture);
        var firstPluginCount = customization.PluginCount;
        var firstStrategyCount = customization.StrategyCount;

        customization.Customize(fixture);

        Assert.Multiple(() =>
        {
            Assert.That(firstPluginCount, Is.EqualTo(1));
            Assert.That(firstStrategyCount, Is.EqualTo(1));
            Assert.That(customization.PluginCount, Is.EqualTo(1));
            Assert.That(customization.StrategyCount, Is.EqualTo(1));
        });
    }

    [Test]
    public void Configure_CanReplaceValueCreationService()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c => c.UseService(new FixedValueCreationService())
        };

        customization.Customize(fixture);
        var created = fixture.Create<PersonModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("svc-string"));
            Assert.That(created.Age, Is.EqualTo(123));
            Assert.That(created.City, Is.EqualTo("svc-string"));
        });
    }

    [Test]
    public void Configure_CanUseExplicitParameterPropertyMapping()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c => c.Map("city", p => p.Name)
        }
        .With(p => p.Name, "mapped-name")
        .With(p => p.Age, 55);

        customization.Customize(fixture);
        var created = fixture.Create<PersonModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("mapped-name"));
            Assert.That(created.City, Is.EqualTo("mapped-name"));
            Assert.That(created.Age, Is.EqualTo(55));
        });
    }

    [Test]
    public void Configure_CanUseValueFactoriesAndStrategies()
    {
        var fixture = new Fixture();
        var customization = new ProbeCollectionCustomization(new ConstructorCustomizationOptions { CollectionItemCount = 1 })
        {
            ConfigureAction = c =>
            {
                c.UseValueFactory<Guid>(_ => Guid.Parse("11111111-1111-1111-1111-111111111111"));
                c.UseStrategyInternal(new SimpleStrategy(typeof(string), "fixed-name"));
            }
        };

        customization.Customize(fixture);
        var created = fixture.Create<CollectionModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Scores, Has.Length.EqualTo(1));
            Assert.That(created.Names, Has.Count.EqualTo(1));
            Assert.That(created.Counts, Has.Count.EqualTo(1));
            Assert.That(created.Ids, Is.All.EqualTo(Guid.Parse("11111111-1111-1111-1111-111111111111")));
        });
    }

    [Test]
    public void Configure_CanUsePluginDelegateFactory()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c =>
            {
                c.UsePluginDelegate(
                    t => t == typeof(int),
                    static (_, _, _) => 777);
            }
        }
        .With(p => p.Name, "N")
        .With(p => p.City, "C");

        customization.Customize(fixture);
        var created = fixture.Create<PersonModel>();

        Assert.That(created.Age, Is.EqualTo(777));
    }

    [Test]
    public void Configure_CanUseValueFactoryWithServiceOverload()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c =>
            {
                c.UseValueFactoryWithService<int>((_, _) => 808);
            }
        }
        .With(p => p.Name, "N")
        .With(p => p.City, "C");

        customization.Customize(fixture);
        var created = fixture.Create<PersonModel>();

        Assert.That(created.Age, Is.EqualTo(808));
    }

    [Test]
    public void Configure_CanUseCustomParameterMatcher()
    {
        var fixture = new Fixture();
        var customization = new ProbeTripleStringCustomization
        {
            ConfigureAction = c => c.UseMatcherInternal(new AllToFirstMatcher())
        }
        .With(p => p.First, "same");

        customization.Customize(fixture);
        var created = fixture.Create<TripleStringModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.First, Is.EqualTo("same"));
            Assert.That(created.Second, Is.EqualTo("same"));
            Assert.That(created.Third, Is.EqualTo("same"));
        });
    }

    [Test]
    public void Configure_CanUseCustomExpressionParser()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            ConfigureAction = c => c.UseParserInternal(new ConstantPropertyParser("Name"))
        };

        customization.Customize(fixture);
        customization.With(p => p.City, "parsed");
        var created = fixture.Create<PersonModel>();

        Assert.That(created.Name, Is.EqualTo("parsed"));
    }

    [Test]
    public void Configure_CanUseCustomConstructorSelector()
    {
        var fixture = new Fixture();
        var customization = new ProbeMultiCtorCustomization
        {
            ConfigureAction = c => c.UseSelectorInternal(new SmallestConstructorSelector())
        };

        customization.Customize(fixture);
        var created = fixture.Create<MultiCtorModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.Null);
            Assert.That(created.Age, Is.EqualTo(0));
        });
    }

    [Test]
    public void CreateInstance_CanUseDefaultValueFactories()
    {
        var fixture = new Fixture();
        var customization = new ProbeCustomization
        {
            BeforeCreate = c =>
            {
                c.SetDefaultFromFactory(p => p.Name, () => "factory-name");
                c.SetDefaultFromFixtureFactory(p => p.City, _ => "fixture-city");
                c.SetDefaultValue(p => p.Age, 11);
            }
        };

        customization.Customize(fixture);
        var created = fixture.Create<PersonModel>();

        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("factory-name"));
            Assert.That(created.City, Is.EqualTo("fixture-city"));
            Assert.That(created.Age, Is.EqualTo(11));
        });
    }

    [Test]
    public void RegistrationMethods_ValidateNullArguments()
    {
        var customization = new ProbeCustomization();

        Assert.Multiple(() =>
        {
            Assert.That(() => customization.UseStoreFactory(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseService(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.Map(" ", p => p.Name), Throws.ArgumentException);
            Assert.That(() => customization.UsePluginInternal(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseStrategyInternal(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UsePluginDelegate(null!, static (_, _, _) => null), Throws.ArgumentNullException);
            Assert.That(() => customization.UsePluginDelegate(static _ => true, null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseValueFactory<int>(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseValueFactoryWithService<int>(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseSelectorInternal(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseMatcherInternal(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.UseParserInternal(null!), Throws.ArgumentNullException);
            Assert.That(() => customization.With(p => p.Name, (Func<string>)null!), Throws.ArgumentNullException);
            Assert.That(() => customization.With(p => p.Name, (Func<IFixture, string>)null!), Throws.ArgumentNullException);
        });
    }

    [Test]
    public void Customization_WithCaseSensitiveMatcher_UsesPascalFallback()
    {
        var fixture = new Fixture();
        var customization = new ConstructorCustomization<PersonWithFirstNameModel>(new ConstructorCustomizationOptions { PropertyNameComparer = StringComparer.Ordinal })
            .With(p => p.FirstName, "Pascal")
            .With(p => p.Age, 5)
            .With(p => p.City, "Town");

        customization.Customize(fixture);
        var created = fixture.Create<PersonWithFirstNameModel>();

        Assert.That(created.FirstName, Is.EqualTo("Pascal"));
    }

    [Test]
    public void ProtectedHelpers_CanGetSetRemoveAndCreateValues()
    {
        var customization = new ProbeCustomization();
        customization.SetValue(p => p.Name, "helper-value");

        var fromHelper = customization.GetValue(p => p.Name);
        customization.RemoveValue(p => p.Name);
        var afterRemove = customization.GetValue(p => p.Name);
        var generated = customization.GetOrCreateValue(p => p.City, new Fixture());

        Assert.Multiple(() =>
        {
            Assert.That(fromHelper, Is.EqualTo("helper-value"));
            Assert.That(afterRemove, Is.Null);
            Assert.That(generated, Is.Not.Null);
        });
    }

    private sealed class ProbeCustomization : ConstructorCustomization<PersonModel, ProbeCustomization>
    {
        public ProbeCustomization(ConstructorCustomizationOptions? options = null)
            : base(options)
        {
        }

        public Action<ProbeCustomization>? ConfigureAction { get; set; }

        public Action<ProbeCustomization>? BeforeCreate { get; set; }

        public int PluginCount => Plugins.Count;

        public int StrategyCount => UserStrategies.Count;

        protected override void Configure()
        {
            ConfigureAction?.Invoke(this);
        }

        protected override PersonModel CreateInstance(IFixture fixture)
        {
            BeforeCreate?.Invoke(this);
            return base.CreateInstance(fixture);
        }

        public ProbeCustomization SetDefaultValue<TProperty>(Expression<Func<PersonModel, TProperty>> expression, TProperty value)
        {
            SetDefault(expression, value);
            return this;
        }

        public ProbeCustomization SetDefaultFromFactory<TProperty>(Expression<Func<PersonModel, TProperty>> expression, Func<TProperty> factory)
        {
            SetDefault(expression, factory);
            return this;
        }

        public ProbeCustomization SetDefaultFromFixtureFactory<TProperty>(Expression<Func<PersonModel, TProperty>> expression, Func<IFixture, TProperty> factory)
        {
            SetDefault(expression, factory);
            return this;
        }

        public bool HasValue(Expression<Func<PersonModel, object?>> expression) => HasValueForProperty(expression);

        public object? GetValue(Expression<Func<PersonModel, object?>> expression) => GetValueForProperty(expression);

        public object? GetOrCreateValue(Expression<Func<PersonModel, object?>> expression, IFixture fixture)
            => GetValueOrCreateForProperty(expression, fixture);

        public void SetValue(Expression<Func<PersonModel, object?>> expression, object? value)
            => SetValueForProperty(expression, value);

        public void RemoveValue(Expression<Func<PersonModel, object?>> expression)
            => RemoveValueForProperty(expression);

        public void UseStoreFactory(Func<IPropertyValueStore> factory) => UsePropertyValueStore(factory);

        public void UseService(IValueCreationService service) => UseValueCreationService(service);

        public void Map<TProperty>(string parameterName, Expression<Func<PersonModel, TProperty>> property)
            => MatchParameterToProperty(parameterName, property);

        public void UsePluginInternal(IValueCreationPlugin plugin) => UsePlugin(plugin);

        public void UsePluginDelegate(Func<Type, bool> predicate, Func<Type, IFixture, IValueCreationService, object?> factory)
            => UsePlugin(predicate, factory);

        public void UseValueFactory<TType>(Func<IFixture, TType?> factory)
            => UseValueFor(factory);

        public void UseValueFactoryWithService<TType>(Func<IFixture, IValueCreationService, TType?> factory)
            => UseValueFor(factory);

        public void UseStrategyInternal(ISpecimenBuilderStrategy strategy) => UseStrategy(strategy);

        public void UseSelectorInternal(IConstructorSelector selector) => UseConstructorSelector(selector);

        public void UseMatcherInternal(IParameterPropertyMatcher matcher) => UseParameterPropertyMatcher(matcher);

        public void UseParserInternal(IPropertyExpressionParser parser) => UsePropertyExpressionParser(parser);
    }

    private sealed class ProbeCollectionCustomization : ConstructorCustomization<CollectionModel, ProbeCollectionCustomization>
    {
        public ProbeCollectionCustomization(ConstructorCustomizationOptions? options = null)
            : base(options)
        {
        }

        public Action<ProbeCollectionCustomization>? ConfigureAction { get; set; }

        protected override void Configure()
        {
            ConfigureAction?.Invoke(this);
        }

        public void UseValueFactory<TType>(Func<IFixture, TType?> factory)
            => UseValueFor(factory);

        public void UseStrategyInternal(ISpecimenBuilderStrategy strategy) => UseStrategy(strategy);
    }

    private sealed class ProbeTripleStringCustomization : ConstructorCustomization<TripleStringModel, ProbeTripleStringCustomization>
    {
        public Action<ProbeTripleStringCustomization>? ConfigureAction { get; set; }

        protected override void Configure()
        {
            ConfigureAction?.Invoke(this);
        }

        public void UseMatcherInternal(IParameterPropertyMatcher matcher) => UseParameterPropertyMatcher(matcher);
    }

    private sealed class ProbeMultiCtorCustomization : ConstructorCustomization<MultiCtorModel, ProbeMultiCtorCustomization>
    {
        public Action<ProbeMultiCtorCustomization>? ConfigureAction { get; set; }

        protected override void Configure()
        {
            ConfigureAction?.Invoke(this);
        }

        public void UseSelectorInternal(IConstructorSelector selector) => UseConstructorSelector(selector);
    }

    private sealed class SimplePlugin(Type matchType, object? value) : IValueCreationPlugin
    {
        public bool CanCreate(Type type) => type == matchType;

        public object? Create(Type type, IFixture fixture, IValueCreationService valueCreationService) => value;
    }

    private sealed class SimpleStrategy(Type matchType, object? value) : ISpecimenBuilderStrategy
    {
        public bool CanBuild(Type type) => type == matchType;

        public object? Build(Type type, IFixture fixture, IValueCreationService valueCreationService, ConstructorCustomizationOptions options)
            => value;
    }

    private sealed class FixedValueCreationService : IValueCreationService
    {
        public object? CreateValue(IFixture fixture, ParameterInfo parameter) => CreateValue(fixture, parameter.ParameterType);

        public object? CreateValue(IFixture fixture, Type type)
        {
            if (type == typeof(int))
            {
                return 123;
            }

            if (type == typeof(string))
            {
                return "svc-string";
            }

            return Activator.CreateInstance(type);
        }
    }

    private sealed class AllToFirstMatcher : IParameterPropertyMatcher
    {
        public bool TryGetPropertyName(ParameterInfo parameter, IEnumerable<string> configuredPropertyNames, out string propertyName)
        {
            propertyName = "First";
            return true;
        }
    }

    private sealed class ConstantPropertyParser(string propertyName) : IPropertyExpressionParser
    {
        public string GetPropertyName(LambdaExpression propertyExpression) => propertyName;
    }

    private sealed class SmallestConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo SelectConstructor(Type targetType, ConstructorInfo[] constructors)
            => constructors.OrderBy(c => c.GetParameters().Length).First();
    }

    private sealed class PersonWithFirstNameModel
    {
        public PersonWithFirstNameModel(string firstName, int age, string city)
        {
            FirstName = firstName;
            Age = age;
            City = city;
        }

        public string FirstName { get; }

        public int Age { get; }

        public string City { get; }
    }
}
