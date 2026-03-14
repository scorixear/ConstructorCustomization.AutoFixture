# Extensions Overview

The library is built around a subclass-first pattern and a set of small, optional extension
points. Most real-world needs are covered by the first three tools below. Reach for deeper
extension points only when a concrete test need appears.

## Primary extension points

### 1. Subclass defaults — `SetDefault` in `CreateInstance`

Override `CreateInstance` to declare stable defaults for your type. Tests can override any
of these with `With` / `Without`.

```csharp
public class OrderCustomization : ConstructorCustomization<Order, OrderCustomization>
{
    protected override Order CreateInstance(IFixture fixture)
    {
        SetDefault(x => x.Status, OrderStatus.Pending);
        return base.CreateInstance(fixture);
    }
}
```

### 2. Type-specific value factories — `UseValueFor` in `Configure`

Control how specific argument types are created. Runs before built-in collection strategies
and before the AutoFixture fallback.

```csharp
protected override void Configure()
{
    UseValueFor<Currency>(fixture => Currency.USD);
    UseValueFor<Money>((fixture, svc) =>
        new Money((decimal)svc.CreateValue(fixture, typeof(decimal))!, "USD"));
}
```

### 3. Predicate-based plugins — `UsePlugin` in `Configure`

Handle whole type families (all enums, all records, etc.)

```csharp
protected override void Configure()
{
    UsePlugin(
        type => type.IsEnum,
        (type, fixture, svc) => Enum.GetValues(type).GetValue(0)!);
}
```

Or register a reusable `IValueCreationPlugin` implementation:

```csharp
public sealed class CurrencyPlugin : IValueCreationPlugin
{
    public bool CanCreate(Type type) => type == typeof(Currency);
    public object? Create(Type type, IFixture fixture, IValueCreationService svc)
        => Currency.USD;
}

protected override void Configure()
{
    UsePlugin(new CurrencyPlugin());
}
```

## Secondary extension points

| Extension                   | Where to hook in                                  | When to use                                       |
| --------------------------- | ------------------------------------------------- | ------------------------------------------------- |
| `ISpecimenBuilderStrategy`  | `UseStrategy(...)` in `Configure`                 | Custom collection shapes or value-object wrappers |
| `IValueCreationService`     | `UseValueCreationService(...)` in `Configure`     | Replace the entire three-stage pipeline           |
| `IConstructorSelector`      | `UseConstructorSelector(...)` in `Configure`      | Non-default constructor selection logic           |
| `IParameterPropertyMatcher` | `UseParameterPropertyMatcher(...)` in `Configure` | Snake_case or other non-standard naming           |
| `IPropertyExpressionParser` | `UsePropertyExpressionParser(...)` in `Configure` | Custom expression/member naming rules             |
| `IPropertyValueStore`       | `UsePropertyValueStore(...)` in `Configure`       | Auditing, diagnostics, custom key normalization   |

## Full example

```csharp
public class InvoiceCustomization : ConstructorCustomization<Invoice, InvoiceCustomization>
{
    protected override void Configure()
    {
        UseValueFor<Currency>(fixture => Currency.EUR);
        UsePlugin(type => type.IsEnum, (type, f, svc) => Enum.GetValues(type).GetValue(0)!);
        UseStrategy(new ImmutableArrayStrategy());
    }

    protected override Invoice CreateInstance(IFixture fixture)
    {
        SetDefault(x => x.IsPaid, false);
        SetDefault(x => x.IssuedAt, f => f.Create<DateTimeOffset>());
        return base.CreateInstance(fixture);
    }
}

// In a test:
fixture.Customize(new InvoiceCustomization()
    .With(x => x.IsPaid, true));
```

## Read next

- [Extension: IValueCreationPlugin](Extension-IValueCreationPlugin)
- [Extension: ISpecimenBuilderStrategy](Extension-ISpecimenBuilderStrategy)
- [Extension: IValueCreationService](Extension-IValueCreationService)

