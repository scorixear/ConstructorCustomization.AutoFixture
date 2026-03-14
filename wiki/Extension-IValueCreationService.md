# Extension: IValueCreationService

> **For most use cases, use plugins instead.**
> Override `Configure` in your customization class and call `UseValueFor<T>(...)` or
> `UsePlugin(...)`. See [Extension: IValueCreationPlugin](Extension-IValueCreationPlugin).

Replace `IValueCreationService` only when you need to change the entire value creation pipeline,
not just individual types. This is a rarely needed, advanced extension point.

## When to use it

- Completely replace the three-stage pipeline (plugins → strategies → fixture fallback).
- Apply cross-cutting logic before any type matching.
- Integrate an external randomization or determinism framework at the root level.

## Minimal implementation

```csharp
using System.Reflection;
using AutoFixture;
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

public sealed class DeterministicValueCreationService : IValueCreationService
{
    public object? CreateValue(IFixture fixture, ParameterInfo parameter)
        => CreateValue(fixture, parameter.ParameterType);

    public object? CreateValue(IFixture fixture, Type type)
    {
        if (type == typeof(string)) return "fixed-value";
        if (type == typeof(DateTime)) return new DateTime(2000, 1, 1);

        return fixture.Create(type, new AutoFixture.Kernel.SpecimenContext(fixture));
    }
}
```

## Wire it in

Call `UseValueCreationService` inside your `Configure` override:

```csharp
public class MyCustomization : ConstructorCustomization<MyType, MyCustomization>
{
    protected override void Configure()
    {
        UseValueCreationService(new DeterministicValueCreationService());
    }
}
```

> **Note:** When a custom service is registered, any plugins and strategies also registered in
> the same `Configure` call via `UsePlugin`, `UseValueFor`, or `UseStrategy` are **not**
> automatically applied. The custom service takes full ownership of the pipeline.
> 
> You can access configured Plugins and UserStrategies using the protected properties on `ConstructorCustomization` and apply them manually when implementing your service.

