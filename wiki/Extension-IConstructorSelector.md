# Extension: IConstructorSelector

Use this extension when your type has multiple constructors and you want full control over
which one is used.

## When to use it

- Prefer parameterless constructors for specific tests.
- Prefer constructors marked with custom attributes.
- Avoid very large constructors for expensive object graphs.

## Minimal implementation

```csharp
using System.Reflection;
using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

public sealed class SmallestConstructorSelector : IConstructorSelector
{
    public ConstructorInfo SelectConstructor(Type targetType, ConstructorInfo[] constructors)
    {
        if (constructors.Length == 0)
        {
            throw new InvalidOperationException($"No public constructors for {targetType.FullName}");
        }

        return constructors.OrderBy(c => c.GetParameters().Length).First();
    }
}
```

## Wire it in

Call `UseConstructorSelector` inside your `Configure` override:

```csharp
public class MyCustomization : ConstructorCustomization<MyType, MyCustomization>
{
    protected override void Configure()
    {
        UseConstructorSelector(new SmallestConstructorSelector());
    }
}
```

## Default behavior

The built-in selector chooses the constructor with the largest parameter count.

