# Extension: IValueCreationPlugin

Plugins control how specific types are created for constructor arguments that have no explicit
`With` override. Register plugins inside the `Configure` override of your customization class.

## Value creation pipeline

For every constructor argument with no test override, the value creation pipeline evaluates in
this order:

1. **Plugins** — registered in order via `UseValueFor<T>(...)` or `UsePlugin(...)` in `Configure`
2. **Built-in strategies** — handles arrays, lists, dictionaries, and sets
3. **AutoFixture fallback** — `fixture.Create<T>()` for everything else

## Registration inside Configure()

### Exact type, fixture access

```csharp
public class OrderCustomization : ConstructorCustomization<Order, OrderCustomization>
{
    protected override void Configure()
    {
        UseValueFor<string>(fixture => "deterministic-string");
        UseValueFor<DateTime>(fixture => new DateTime(2020, 1, 1));
    }
}
```

### Exact type, with recursive creation

Use `IValueCreationService` when the created value itself needs recursively generated parts.

```csharp
public class InvoiceCustomization : ConstructorCustomization<Invoice, InvoiceCustomization>
{
    protected override void Configure()
    {
        UseValueFor<Money>((fixture, svc) =>
        {
            var amount = (decimal)svc.CreateValue(fixture, typeof(decimal))!;
            return new Money(amount, "USD");
        });
    }
}
```

### Predicate-based, for type families

```csharp
public class ReportCustomization : ConstructorCustomization<Report, ReportCustomization>
{
    protected override void Configure()
    {
        UsePlugin(
            type => type.IsEnum,
            (type, fixture, svc) => Enum.GetValues(type).GetValue(0)!);
    }
}
```

## Implement IValueCreationPlugin directly

For stateful or reusable logic, implement `IValueCreationPlugin` and register with `UsePlugin`:

```csharp
using ConstructorCustomization.AutoFixture.ValueGeneration.Ports;

public sealed class CurrencyPlugin : IValueCreationPlugin
{
    public bool CanCreate(Type type) => type == typeof(Currency);

    public object? Create(Type type, IFixture fixture, IValueCreationService valueCreationService)
        => Currency.USD;
}

public class InvoiceCustomization : ConstructorCustomization<Invoice, InvoiceCustomization>
{
    protected override void Configure()
    {
        UsePlugin(new CurrencyPlugin());
    }
}
```

## Precedence

When multiple plugins are registered, the **first one whose `CanCreate` returns `true`** wins.
Register more specific plugins before less specific ones.

```csharp
public class OrderCustomization : ConstructorCustomization<Order, OrderCustomization>
{
    protected override void Configure()
    {
        UseValueFor<string>(fixture => "specific");          // checked first
        UsePlugin(
            type => type == typeof(string),
            (type, fixture, svc) => "never reached");       // never invoked for string
    }
}
```

## Default behavior is preserved

For any type without a matching plugin, the package falls through to its built-in strategies
(array, list, dictionary, set) and ultimately to AutoFixture. You never lose any existing
default behavior by registering plugins.
