using System.Reflection;

using AutoFixture;
using AutoFixture.Kernel;

using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

namespace ConstructorCustomization.AutoFixture.ValueGeneration.Application;

/// <summary>
/// Default implementation of <see cref="IValueCreationService"/> that uses a three-stage pipeline:
/// registered plugins first, then built-in specimen strategies, then AutoFixture fallback.
/// </summary>
public sealed class ValueCreationService : IValueCreationService
{
    private readonly IReadOnlyList<IValueCreationPlugin> plugins;
    private readonly IReadOnlyList<ISpecimenBuilderStrategy> strategies;
    private readonly ConstructorCustomizationOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCreationService"/> class.
    /// </summary>
    /// <param name="plugins">
    /// The ordered plugin list to evaluate first. Pass the same <see cref="List{T}"/> instance
    /// that <see cref="ConstructorCustomization{T}"/> manages so that plugins registered via
    /// fluent methods after construction are automatically reflected here.
    /// </param>
    /// <param name="strategies">The ordered specimen strategies to evaluate when no plugin matches.</param>
    /// <param name="options">The active customization options.</param>
    public ValueCreationService(
        IReadOnlyList<IValueCreationPlugin> plugins,
        IEnumerable<ISpecimenBuilderStrategy> strategies,
        ConstructorCustomizationOptions options)
    {
        ThrowIfNull(plugins);
        ThrowIfNull(strategies);
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.plugins = plugins;
        this.strategies = [.. strategies];
    }

    /// <inheritdoc />
    public object? CreateValue(IFixture fixture, ParameterInfo parameter)
    {
        ThrowIfNull(parameter);
        return CreateValue(fixture, parameter.ParameterType);
    }

    /// <inheritdoc />
    public object? CreateValue(IFixture fixture, Type type)
    {
        ThrowIfNull(fixture);
        ThrowIfNull(type);

        foreach (var plugin in plugins)
        {
            if (plugin.CanCreate(type))
            {
                return plugin.Create(type, fixture, this);
            }
        }

        foreach (var strategy in strategies)
        {
            if (strategy.CanBuild(type))
            {
                return strategy.Build(type, fixture, this, options);
            }
        }

        return fixture.Create(type, new SpecimenContext(fixture));
    }
}
