# Extension: IParameterPropertyMatcher

Use this extension when your constructor parameter names do not map directly to property names.
This is an advanced extension point; the built-in case-insensitive matcher handles most cases.

## Typical case

Constructor uses `first_name`, property is `FirstName`.

## Minimal implementation

```csharp
using System.Reflection;
using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

public sealed class SnakeCaseMatcher : IParameterPropertyMatcher
{
    public bool TryGetPropertyName(
        ParameterInfo parameter,
        IEnumerable<string> configuredPropertyNames,
        out string propertyName)
    {
        var normalized = parameter.Name?.Replace("_", "") ?? string.Empty;

        var match = configuredPropertyNames.FirstOrDefault(x =>
            string.Equals(x, normalized, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            propertyName = string.Empty;
            return false;
        }

        propertyName = match;
        return true;
    }
}
```

## Wire it in

Call `UseParameterPropertyMatcher` inside your `Configure` override:

```csharp
public class MyCustomization : ConstructorCustomization<MyType, MyCustomization>
{
    protected override void Configure()
    {
        UseParameterPropertyMatcher(new SnakeCaseMatcher());
    }
}
```

## Interaction with explicit mappings

If you register `MatchParameterToProperty("param", x => x.Property)` in `Configure`, that mapping
is used first for that parameter. Your custom matcher still handles all parameters that are not
explicitly mapped.

## Default behavior

The built-in matcher compares names case-insensitively and also checks PascalCase variants.

