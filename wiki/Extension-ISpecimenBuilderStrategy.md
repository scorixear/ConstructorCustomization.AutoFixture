# Extension: ISpecimenBuilderStrategy

Use this extension for type-specific generation rules (for example immutable collections or
custom wrappers). Register custom strategies inside the `Configure` override of your
customization class using `UseStrategy`.

## When to use it

- Generate known collection shapes.
- Build special value objects.
- Override generation for one specific type family.

## Minimal implementation

```csharp
using AutoFixture;
using ConstructorCustomization.AutoFixture;
using ConstructorCustomization.AutoFixture.SpecimenGeneration.Ports;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

public sealed class ImmutableArrayStrategy : ISpecimenBuilderStrategy
{
    public bool CanBuild(Type type)
        => type.IsGenericType &&
           type.GetGenericTypeDefinition().FullName == "System.Collections.Immutable.ImmutableArray`1";

    public object? Build(
        Type type,
        IFixture fixture,
        IValueCreationService valueCreationService,
        ConstructorCustomizationOptions options)
    {
        var elementType = type.GetGenericArguments()[0];
        var array = Array.CreateInstance(elementType, options.CollectionItemCount);

        for (var i = 0; i < options.CollectionItemCount; i++)
        {
            array.SetValue(valueCreationService.CreateValue(fixture, elementType), i);
        }

        var createRange = type.GetMethod("CreateRange", new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
        return createRange?.Invoke(null, new object?[] { array.Cast<object?>() });
    }
}
```

## Register with UseStrategy

Call `UseStrategy` inside your `Configure` override. Custom strategies are evaluated before
built-in ones.

```csharp
public class MyCustomization : ConstructorCustomization<MyType, MyCustomization>
{
    protected override void Configure()
    {
        UseStrategy(new ImmutableArrayStrategy());
    }
}
```

