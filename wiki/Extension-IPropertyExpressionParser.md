# Extension: IPropertyExpressionParser

Use this extension when you want special rules for parsing `With(...)` and `Without(...)` expressions.
This is an advanced extension point; the built-in parser handles all normal member expressions.

## When to use it

- Support custom member naming conventions.
- Normalize aliases.
- Enforce project-specific expression rules.

## Minimal implementation

```csharp
using System.Linq.Expressions;
using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

public sealed class LowerCasePropertyParser : IPropertyExpressionParser
{
    public string GetPropertyName(LambdaExpression propertyExpression)
    {
        if (propertyExpression.Body is not MemberExpression m)
        {
            throw new ArgumentException("Expected member expression", nameof(propertyExpression));
        }

        return m.Member.Name.ToLowerInvariant();
    }
}
```

## Tip

If you customize parsing, your matcher and value store should use compatible naming logic.

## Wire it in

Call `UsePropertyExpressionParser` inside your `Configure` override:

```csharp
public class MyCustomization : ConstructorCustomization<MyType, MyCustomization>
{
    protected override void Configure()
    {
        UsePropertyExpressionParser(new LowerCasePropertyParser());
    }
}
```

