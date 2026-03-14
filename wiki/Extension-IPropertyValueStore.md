# Extension: IPropertyValueStore

Use this extension when you need custom storage behavior for configured overrides.
This is an advanced extension point; the built-in in-memory store handles all normal cases.

## When to use it

- Add auditing or diagnostics.
- Use custom key normalization.
- Persist configured values outside memory.

## Minimal implementation

```csharp
using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

public sealed class LoggingPropertyValueStore : IPropertyValueStore
{
    private readonly Dictionary<string, object?> values =
        new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> PropertyNames => values.Keys;

    public bool Contains(string propertyName) => values.ContainsKey(propertyName);

    public bool TryGetValue(string propertyName, out object? value)
        => values.TryGetValue(propertyName, out value);

    public void SetValue(string propertyName, object? value)
    {
        Console.WriteLine($"Set {propertyName}");
        values[propertyName] = value;
    }

    public bool RemoveValue(string propertyName) => values.Remove(propertyName);

    public void Clear() => values.Clear();
}
```

## Tip

Use the same comparer strategy as your matcher and parser to avoid mismatches.

## Wire it in

Call `UsePropertyValueStore` inside your `Configure` override. Pass a factory — it is called
twice per `Customize()` call, once for the test-override store (`With`/`Without`) and once for
the subclass-default store (`SetDefault`):

```csharp
public class MyCustomization : ConstructorCustomization<MyType, MyCustomization>
{
    protected override void Configure()
    {
        UsePropertyValueStore(() => new LoggingPropertyValueStore());
    }
}
```

Existing test overrides set before `Customize()` is called are migrated to the new store
automatically.

