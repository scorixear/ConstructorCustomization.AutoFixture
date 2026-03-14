# Getting Started

This guide shows the primary usage pattern for ConstructorCustomization.AutoFixture.

## Install

```powershell
dotnet add package ConstructorCustomization.AutoFixture
```

## Primary pattern — create a typed customization class

The recommended approach is to create a typed subclass of `ConstructorCustomization<T, TSelf>`
for each type you want to customize. This gives you a single place to define how a type is
created across all tests.

### 1. Define a customization class

```csharp
using AutoFixture;
using ConstructorCustomization.AutoFixture;

public class Person
{
    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }
}

public class PersonCustomization : ConstructorCustomization<Person, PersonCustomization>
{
    protected override Person CreateInstance(IFixture fixture)
    {
        // Set stable defaults for every test. Tests can override these with With/Without.
        SetDefault(x => x.FirstName, "Ada");
        SetDefault(x => x.LastName, "Lovelace");
        SetDefault(x => x.Age, 36);
        return base.CreateInstance(fixture);
    }
}
```

### 2. Register the customization

```csharp
var fixture = new Fixture();
fixture.Customize(new PersonCustomization());

var person = fixture.Create<Person>();
// person.FirstName == "Ada", person.LastName == "Lovelace", person.Age == 36
```

### 3. Override specific values per test

Use `With` and `Without` when a single test needs a different value. These always win over
the subclass defaults set with `SetDefault`.

```csharp
var person = fixture.Create<Person>(); // uses PersonCustomization defaults

fixture.Customize(new PersonCustomization()
    .With(x => x.Age, 18));           // only Age changes; FirstName and LastName use defaults

var youngPerson = fixture.Create<Person>();
// youngPerson.Age == 18
```

## Explicit parameter-to-property mapping

When a constructor parameter name does not match a property name, register an explicit mapping
inside `Configure`. You still configure values normally through `With`, `Without`, or `SetDefault`
using the property expression.

```csharp
public class User
{
    public User(string given_name, string family_name)
    {
        FirstName = given_name;
        LastName = family_name;
    }

    public string FirstName { get; }
    public string LastName { get; }
}

public class UserCustomization : ConstructorCustomization<User, UserCustomization>
{
    protected override void Configure()
    {
        MatchParameterToProperty("given_name", x => x.FirstName);
        MatchParameterToProperty("family_name", x => x.LastName);
    }

    protected override User CreateInstance(IFixture fixture)
    {
        SetDefault(x => x.FirstName, "Ada");
        SetDefault(x => x.LastName, "Lovelace");
        return base.CreateInstance(fixture);
    }
}
```

Parameters without an explicit mapping continue to use normal parameter-to-property matching.

## Out-of-the-box usage (no subclass)

When you need a quick customization without creating a class, use `ConstructorCustomization<T>` directly.
This uses AutoFixture defaults for any argument not explicitly configured.

```csharp
fixture.Customize(new ConstructorCustomization<Person>()
    .With(x => x.FirstName, "Ada")
    .With(x => x.LastName, "Lovelace"));
```

## Deferred values

Use a factory delegate when the value should be evaluated at object creation time.

```csharp
public class PersonCustomization : ConstructorCustomization<Person, PersonCustomization>
{
    protected override Person CreateInstance(IFixture fixture)
    {
        SetDefault(x => x.FirstName, () => Guid.NewGuid().ToString("N"));
        SetDefault(x => x.Age, f => f.Create<int>());
        return base.CreateInstance(fixture);
    }
}
```

The same factory forms are available for `With`:

```csharp
fixture.Customize(new PersonCustomization()
    .With(x => x.FirstName, () => $"Test-{Guid.NewGuid():N}")
    .With(x => x.Age, f => f.Create<int>()));
```

## Null override

Call `Without` to set a property to `null`:

```csharp
fixture.Customize(new PersonCustomization()
    .Without(x => x.LastName));
```

## Clear all test overrides

Call `Clear()` to remove all values set via `With` and `Without`, restoring subclass defaults:

```csharp
var customization = new PersonCustomization()
    .With(x => x.Age, 99);

customization.Clear(); // age will use the SetDefault value again
```

## How it works

1. `Configure()` is called once when `fixture.Customize(...)` is invoked. Register plugins and strategies there.
2. For each `fixture.Create<T>()` call, `CreateInstance(fixture)` runs, which calls `SetDefault` to populate defaults.
3. For each constructor parameter, the lookup order is:
   - Test override (`With` / `Without`) — wins unconditionally
   - Subclass default (`SetDefault`)
   - AutoFixture auto-generation

## Next steps

- [Customizing Behavior](Customizing-Behavior) — plugins, type-specific factories, and strategies
- [Extensions Overview](Extensions-Overview) — full extension point map

Go to [Extensions Overview](Extensions-Overview) to customize behavior.
