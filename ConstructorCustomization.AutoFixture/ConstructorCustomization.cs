using System.Linq.Expressions;
using System.Reflection;

using AutoFixture;

using ConstructorCustomization.AutoFixture.Composition;
using ConstructorCustomization.AutoFixture.Customization.Adapters;
using ConstructorCustomization.AutoFixture.Customization.Application.Ports;
using ConstructorCustomization.AutoFixture.Customization.Domain;
using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Adapters;
using ConstructorCustomization.AutoFixture.ValueGeneration.Application;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture;

/// <summary>
/// Base class for creating a typed customization for <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// <para>
/// The intended usage pattern is:
/// <list type="number">
///   <item>Create a class that inherits from <c>ConstructorCustomization&lt;T, TSelf&gt;</c>.</item>
///   <item>
///     Override <see cref="Configure"/> to register plugins, value factories, and strategies once.
///     This method is called once per <see cref="Customize"/> call.
///   </item>
///   <item>
///     Override <see cref="CreateInstance"/> to set per-creation defaults using <c>SetDefault</c>,
///     then call <c>base.CreateInstance(fixture)</c> to construct the instance.
///   </item>
///   <item>
///     In tests, call <see cref="With{TProperty}(Expression{Func{T,TProperty}},TProperty)"/> and
///     <see cref="Without{TProperty}"/> on your customization to apply per-test overrides.
///     Test overrides always win over subclass defaults.
///   </item>
/// </list>
/// </para>
/// <para>
/// When no subclass is needed, use <see cref="ConstructorCustomization{T}"/> directly.
/// </para>
/// </remarks>
/// <typeparam name="T">The target type to customize.</typeparam>
/// <typeparam name="TSelf">The concrete customization type (used for fluent return types).</typeparam>
public class ConstructorCustomization<T, TSelf> : ICustomization
    where T : class
    where TSelf : ConstructorCustomization<T, TSelf>
{
    private ConstructorCustomizationOptions Options { get; }
    private CustomizationDomainOptions DomainOptions { get; }
    private IPropertyExpressionParser PropertyExpressionParser { get; set; }
    private IPropertyValueStore DefaultPropertyValueStore { get; set; }
    private IPropertyValueStore OverridePropertyValueStore { get; set; }
    private IConstructorSelector ConstructorSelector { get; set; }
    private IParameterPropertyMatcher ParameterPropertyMatcher { get; set; }
    private IValueCreationService ValueCreationService { get; set; }
    private Dictionary<string, string> ExplicitParameterPropertyMappings { get; }

    // Registered services from Configure() — reset on every Customize() call
    private Func<IPropertyValueStore>? RegisteredValueStoreFactory { get; set; }
    private IPropertyExpressionParser? RegisteredPropertyExpressionParser { get; set; }
    private IConstructorSelector? RegisteredConstructorSelector { get; set; }
    private IParameterPropertyMatcher? RegisteredParameterPropertyMatcher { get; set; }
    private IValueCreationService? RegisteredValueCreationService { get; set; }

    // Plugins and strategies registered via Configure() are stored here and applied to the default value creation service factory
    protected List<IValueCreationPlugin> Plugins { get; } = [];
    protected List<ISpecimenBuilderStrategy> UserStrategies { get; } = [];


    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorCustomization{T, TSelf}"/> class.
    /// </summary>
    /// <param name="options">Optional behavior options. When omitted, defaults are used.</param>
    protected ConstructorCustomization(ConstructorCustomizationOptions? options = null)
    {
        Options = options ?? new ConstructorCustomizationOptions();
        DomainOptions = CustomizationDomainOptions.From(Options);
        PropertyExpressionParser = new PropertyExpressionParser();
        DefaultPropertyValueStore = new PropertyValueStore(DomainOptions.PropertyNameComparer);
        OverridePropertyValueStore = new PropertyValueStore(DomainOptions.PropertyNameComparer);
        ConstructorSelector = new LargestConstructorSelector();
        ParameterPropertyMatcher = new CaseInsensitiveParameterPropertyMatcher(DomainOptions.PropertyNameComparer);
        ValueCreationService = new ValueCreationService(Plugins, DefaultServiceFactory.CreateSpecimenStrategies(), Options);
        ExplicitParameterPropertyMappings = new Dictionary<string, string>(DomainOptions.PropertyNameComparer);
    }

    /// <summary>
    /// Applies this customization to the provided fixture.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        // Reset registered extensions so Configure() starts from a clean slate
        RegisteredConstructorSelector = null;
        RegisteredParameterPropertyMatcher = null;
        RegisteredPropertyExpressionParser = null;
        RegisteredValueStoreFactory = null;
        RegisteredValueCreationService = null;
        Plugins.Clear();
        UserStrategies.Clear();
        ExplicitParameterPropertyMappings.Clear();

        Configure();

        // Apply optional service registrations from Configure(), falling back to defaults
        if (RegisteredConstructorSelector is not null)
        {
            ConstructorSelector = RegisteredConstructorSelector;
        }
        if (RegisteredParameterPropertyMatcher is not null)
        {
            ParameterPropertyMatcher = RegisteredParameterPropertyMatcher;
        }
        if (RegisteredPropertyExpressionParser is not null)
        {
            PropertyExpressionParser = RegisteredPropertyExpressionParser;
        }
        if (RegisteredValueCreationService is not null)
        {
            ValueCreationService = RegisteredValueCreationService;
        }
        else
        {
            // Rebuild the default value creation pipeline for this customization run so
            // plugins and user strategies registered in Configure() are applied.
            ValueCreationService = new ValueCreationService(
                Plugins,
                UserStrategies.Concat(DefaultServiceFactory.CreateSpecimenStrategies()),
                Options);
        }

        if (RegisteredValueStoreFactory is not null)
        {
            // Migrate existing test overrides (With/Without entries) to the new store
            var newOverrideStore = RegisteredValueStoreFactory();
            foreach (var name in OverridePropertyValueStore.PropertyNames.ToList())
            {
                if (OverridePropertyValueStore.TryGetValue(name, out var val))
                    newOverrideStore.SetValue(name, val);
            }
            OverridePropertyValueStore = newOverrideStore;
            DefaultPropertyValueStore = RegisteredValueStoreFactory();
        }


        fixture.Customize<T>(f => f.FromFactory(() =>
        {
            return CreateInstance(fixture);
        }));
    }

    // ── Subclass registration — for use inside Configure() ──────────────────

    /// <summary>
    /// Override this method to register plugins, value factories, strategies, and service
    /// implementations. Called once per <see cref="Customize"/> call, before any instance is created.
    /// </summary>
    protected virtual void Configure() { }

    #region Registration methods for Configure()
    /// <summary>
    /// Registers a value creation plugin that handles types where <paramref name="predicate"/> returns
    /// <see langword="true"/>. Plugins are evaluated before built-in strategies.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UsePlugin(Func<Type, bool> predicate, Func<Type, IFixture, IValueCreationService, object?> factory)
    {
        ThrowIfNull(predicate);
        ThrowIfNull(factory);
        Plugins.Add(new DelegateValueCreationPlugin(predicate, factory));
    }

    /// <summary>
    /// Registers a pre-built plugin. Plugins are evaluated before built-in strategies.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UsePlugin(IValueCreationPlugin plugin)
    {
        ThrowIfNull(plugin);
        Plugins.Add(plugin);
    }

    /// <summary>
    /// Registers a value factory for an exact type.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    /// <typeparam name="TType">The exact type to handle.</typeparam>
    protected void UseValueFor<TType>(Func<IFixture, TType?> factory)
    {
        ThrowIfNull(factory);
        Plugins.Add(new DelegateValueCreationPlugin(
            t => t == typeof(TType),
            (type, fixture, svc) => factory(fixture)));
    }

    /// <summary>
    /// Registers a value factory for an exact type with access to the value creation service
    /// for recursive creation of nested types.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    /// <typeparam name="TType">The exact type to handle.</typeparam>
    protected void UseValueFor<TType>(Func<IFixture, IValueCreationService, TType?> factory)
    {
        ThrowIfNull(factory);
        Plugins.Add(new DelegateValueCreationPlugin(
            t => t == typeof(TType),
            (type, fixture, svc) => factory(fixture, svc)));
    }

    /// <summary>
    /// Registers a specimen builder strategy that is evaluated before built-in strategies.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UseStrategy(ISpecimenBuilderStrategy strategy)
    {
        ThrowIfNull(strategy);
        UserStrategies.Add(strategy);
    }

    /// <summary>
    /// Registers a constructor selector that controls which constructor is used to build
    /// <typeparamref name="T"/>. Replaces the default largest-constructor selector.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UseConstructorSelector(IConstructorSelector selector)
    {
        ThrowIfNull(selector);
        RegisteredConstructorSelector = selector;
    }

    /// <summary>
    /// Registers a parameter-to-property matcher that maps constructor parameter names to
    /// configured property names. Replaces the default case-insensitive matcher.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UseParameterPropertyMatcher(IParameterPropertyMatcher matcher)
    {
        ThrowIfNull(matcher);
        RegisteredParameterPropertyMatcher = matcher;
    }

    /// <summary>
    /// Registers an explicit mapping from a constructor parameter name to a property.
    /// When a mapping exists for a parameter, that mapped property is used for value lookup.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    /// <typeparam name="TProperty">The mapped property type.</typeparam>
    /// <param name="parameterName">The constructor parameter name.</param>
    /// <param name="propertyExpression">The target property expression.</param>
    protected void MatchParameterToProperty<TProperty>(string parameterName, Expression<Func<T, TProperty>> propertyExpression)
    {
        ThrowIfNullOrWhiteSpace(parameterName);
        ThrowIfNull(propertyExpression);

        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        ExplicitParameterPropertyMappings[parameterName.Trim()] = propertyName;
    }

    /// <summary>
    /// Registers a property expression parser that extracts property names from
    /// <c>With</c> / <c>Without</c> / <c>SetDefault</c> expressions.
    /// Replaces the default member-expression parser.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UsePropertyExpressionParser(IPropertyExpressionParser parser)
    {
        ThrowIfNull(parser);
        RegisteredPropertyExpressionParser = parser;
    }

    /// <summary>
    /// Registers a factory that produces <see cref="IPropertyValueStore"/> instances.
    /// The factory is called twice per <see cref="Customize"/> call — once for the test-override
    /// store used by <c>With</c> / <c>Without</c>, and once for the subclass-default store used
    /// by <c>SetDefault</c>. Existing test overrides are migrated to the new store.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UsePropertyValueStore(Func<IPropertyValueStore> factory)
    {
        ThrowIfNull(factory);
        RegisteredValueStoreFactory = factory;
    }

    /// <summary>
    /// Registers a custom value creation service that replaces the entire three-stage pipeline
    /// (plugins → strategies → AutoFixture fallback). When a custom service is registered,
    /// plugins and strategies registered via <c>UsePlugin</c>, <c>UseValueFor</c>, and
    /// <c>UseStrategy</c> in the same <c>Configure</c> call are not automatically applied.
    /// Call from <see cref="Configure"/>.
    /// </summary>
    protected void UseValueCreationService(IValueCreationService service)
    {
        ThrowIfNull(service);
        RegisteredValueCreationService = service;
    }

    #endregion

    // ── Subclass defaults — for use inside CreateInstance() override ─────────

    #region Subclass defaults — for use inside CreateInstance() override
    /// <summary>
    /// Sets a default value for a constructor argument matched to the given property.
    /// The default is used only when no test override was provided via <see cref="With{TProperty}(Expression{Func{T,TProperty}},TProperty)"/>.
    /// Call from <see cref="CreateInstance"/>, before <c>base.CreateInstance(fixture)</c>.
    /// </summary>
    protected void SetDefault<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty value)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        DefaultPropertyValueStore.SetValue(propertyName, value);
    }

    /// <summary>
    /// Sets a deferred default value factory for a constructor argument.
    /// Call from <see cref="CreateInstance"/>, before <c>base.CreateInstance(fixture)</c>.
    /// </summary>
    protected void SetDefault<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<TProperty> valueFactory)
    {
        ThrowIfNull(valueFactory);
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        DefaultPropertyValueStore.SetValue(propertyName, new ConfiguredValueFactory(_ => valueFactory()));
    }

    /// <summary>
    /// Sets a fixture-aware deferred default value factory for a constructor argument.
    /// Call from <see cref="CreateInstance"/>, before <c>base.CreateInstance(fixture)</c>.
    /// </summary>
    protected void SetDefault<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<IFixture, TProperty> valueFactory)
    {
        ThrowIfNull(valueFactory);
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        DefaultPropertyValueStore.SetValue(propertyName, new ConfiguredValueFactory(fixture => valueFactory(fixture)));
    }

    #endregion

    // ── Test overrides — public fluent API ──────────────────────────────────

    #region Test overrides — public fluent API
    /// <summary>
    /// Configures a fixed value for a constructor argument matched to the given property.
    /// Overrides any subclass default set via <c>SetDefault</c>.
    /// </summary>
    public TSelf With<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty value)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        OverridePropertyValueStore.SetValue(propertyName, value);
        return (TSelf)this;
    }

    /// <summary>
    /// Configures a deferred value factory for a constructor argument.
    /// Overrides any subclass default set via <c>SetDefault</c>.
    /// </summary>
    public TSelf With<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<TProperty> valueFactory)
    {
        ThrowIfNull(valueFactory);
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        OverridePropertyValueStore.SetValue(propertyName, new ConfiguredValueFactory(_ => valueFactory()));
        return (TSelf)this;
    }

    /// <summary>
    /// Configures a fixture-aware deferred value factory for a constructor argument.
    /// Overrides any subclass default set via <c>SetDefault</c>.
    /// </summary>
    public TSelf With<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<IFixture, TProperty> valueFactory)
    {
        ThrowIfNull(valueFactory);
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        OverridePropertyValueStore.SetValue(propertyName, new ConfiguredValueFactory(fixture => valueFactory(fixture)));
        return (TSelf)this;
    }

    /// <summary>
    /// Configures a null override for a constructor argument matched to the given property.
    /// </summary>
    public TSelf Without<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        OverridePropertyValueStore.SetValue(propertyName, null);
        return (TSelf)this;
    }

    /// <summary>
    /// Removes all test overrides configured via <see cref="With{TProperty}(Expression{Func{T,TProperty}},TProperty)"/>
    /// and <see cref="Without{TProperty}"/>.
    /// </summary>
    public void Clear()
    {
        OverridePropertyValueStore.Clear();
    }
    #endregion

    // ── Protected helpers for advanced subclass use ──────────────────────────

    #region Protected helpers for advanced subclass use
    /// <summary>
    /// Determines whether a test override exists for the specified property.
    /// </summary>
    protected virtual bool HasValueForProperty(Expression<Func<T, object?>> propertyExpression)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        return OverridePropertyValueStore.Contains(propertyName);
    }

    /// <summary>
    /// Gets the test override value for the specified property, or <see langword="null"/> when not set.
    /// </summary>
    protected virtual object? GetValueForProperty(Expression<Func<T, object?>> propertyExpression)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        return OverridePropertyValueStore.TryGetValue(propertyName, out var value) ? value : null;
    }

    /// <summary>
    /// Gets the resolved value for the specified property — test override first, then subclass default,
    /// then generates one from the fixture.
    /// </summary>
    protected virtual object? GetValueOrCreateForProperty(Expression<Func<T, object?>> propertyExpression, IFixture fixture)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        if (PropertyValueResolutionPolicy.TryResolveConfiguredValue(
            propertyName,
            OverridePropertyValueStore,
            DefaultPropertyValueStore,
            fixture,
            ResolveConfiguredValue,
            out var resolvedValue,
            out _))
        {
            return resolvedValue;
        }

        return ValueCreationService.CreateValue(fixture, propertyExpression.ReturnType);
    }

    /// <summary>
    /// Sets a test override value for the specified property.
    /// </summary>
    protected virtual void SetValueForProperty(Expression<Func<T, object?>> propertyExpression, object? value)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        OverridePropertyValueStore.SetValue(propertyName, value);
    }

    /// <summary>
    /// Removes the test override for the specified property.
    /// </summary>
    protected virtual void RemoveValueForProperty(Expression<Func<T, object?>> propertyExpression)
    {
        var propertyName = PropertyExpressionParser.GetPropertyName(propertyExpression);
        OverridePropertyValueStore.RemoveValue(propertyName);
    }
    #endregion

    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> by selecting a constructor and resolving argument values.
    /// Override this method to call <c>SetDefault</c> for per-creation defaults, then call
    /// <c>base.CreateInstance(fixture)</c>.
    /// </summary>
    /// <param name="fixture">The active fixture instance.</param>
    /// <returns>A constructed instance of <typeparamref name="T"/>.</returns>
    protected virtual T CreateInstance(IFixture fixture)
    {
        var constructors = typeof(T).GetConstructors();
        var constructor = ConstructorSelector.SelectConstructor(typeof(T), constructors);
        var parameters = constructor.GetParameters();
        var parameterValues = new object?[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (TryGetMappedPropertyName(parameter, out var mappedPropertyName))
            {
                if (PropertyValueResolutionPolicy.TryResolveConfiguredValue(
                    mappedPropertyName,
                    OverridePropertyValueStore,
                    DefaultPropertyValueStore,
                    fixture,
                    ResolveConfiguredValue,
                    out var mappedResolvedValue,
                    out _))
                {
                    parameterValues[i] = mappedResolvedValue;
                }
                else
                {
                    parameterValues[i] = ValueCreationService.CreateValue(fixture, parameter);
                }

                continue;
            }

            var configuredPropertyNames = OverridePropertyValueStore.PropertyNames
                .Concat(DefaultPropertyValueStore.PropertyNames)
                .Distinct(DomainOptions.PropertyNameComparer);

            if (ParameterPropertyMatcher.TryGetPropertyName(parameter, configuredPropertyNames, out var propertyName)
                && PropertyValueResolutionPolicy.TryResolveConfiguredValue(
                    propertyName,
                    OverridePropertyValueStore,
                    DefaultPropertyValueStore,
                    fixture,
                    ResolveConfiguredValue,
                    out var resolvedValue,
                    out _))
            {
                parameterValues[i] = resolvedValue;
            }
            else
            {
                parameterValues[i] = ValueCreationService.CreateValue(fixture, parameter);
            }
        }

        return (T)constructor.Invoke(parameterValues);
    }

    private bool TryGetMappedPropertyName(ParameterInfo parameter, out string propertyName)
    {
        ThrowIfNull(parameter);

        var parameterName = parameter.Name;
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            propertyName = string.Empty;
            return false;
        }

        return ExplicitParameterPropertyMappings.TryGetValue(parameterName, out propertyName!);
    }

    /// <summary>
    /// Resolves a configured value, evaluating deferred factories when necessary.
    /// </summary>
    protected virtual object? ResolveConfiguredValue(object? configuredValue, IFixture fixture)
    {
        if (configuredValue is ConfiguredValueFactory configuredValueFactory)
        {
            return configuredValueFactory.Resolve(fixture);
        }

        return configuredValue;
    }
}

/// <summary>
/// Convenience customization for out-of-the-box use when no subclass is needed.
/// For project-wide type customizations, inherit from <see cref="ConstructorCustomization{T, TSelf}"/> instead.
/// </summary>
/// <typeparam name="T">The target type to customize.</typeparam>
public sealed class ConstructorCustomization<T> : ConstructorCustomization<T, ConstructorCustomization<T>>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorCustomization{T}"/> class.
    /// </summary>
    /// <param name="options">Optional behavior options. When omitted, defaults are used.</param>
    public ConstructorCustomization(ConstructorCustomizationOptions? options = null)
        : base(options)
    {
    }
}
