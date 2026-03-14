# ConstructorCustomization.AutoFixture

Constructor-first customization for AutoFixture with clean defaults, per-test overrides, and extension points for advanced object creation.

## Install

```bash
dotnet add package ConstructorCustomization.AutoFixture
```

## Quick Entry

Use this when you want a fast start in a test.

```csharp
using AutoFixture;
using ConstructorCustomization.AutoFixture;

var fixture = new Fixture();

fixture.Customize(new ConstructorCustomization<Person>()
	.With(x => x.FirstName, "Ada")
	.With(x => x.LastName, "Lovelace")
	.Without(x => x.MiddleName));

var person = fixture.Create<Person>();
```

## Simple Usage Pattern

For reusable test behavior, create a typed customization once and use it across many tests.

```csharp
using AutoFixture;
using ConstructorCustomization.AutoFixture;

public class PersonCustomization : ConstructorCustomization<Person, PersonCustomization>
{
	protected override Person CreateInstance(IFixture fixture)
	{
		SetDefault(x => x.FirstName, "Ada");
		SetDefault(x => x.LastName, "Lovelace");
		SetDefault(x => x.Age, 36);
		return base.CreateInstance(fixture);
	}
}

var fixture = new Fixture();
var customization = new PersonCustomization();
fixture.Customize(customization);

var defaultPerson = fixture.Create<Person>();

customization.With(x => x.Age, 18);
var youngPerson = fixture.Create<Person>();

customization.Clear();
```

## Capabilities at a Glance

- Constructor-first object creation for test models.
- Stable defaults with `SetDefault(...)`.
- Per-test overrides with `With(...)` and `Without(...)`.
- Explicit parameter-to-property mapping with `MatchParameterToProperty(...)`.
- Deferred value generation with factory delegates.
- Pluggable extension model for matching and specimen/value creation behavior.

## Documentation and Wiki

- Wiki home: [Home](wiki/Home.md)
- Getting started: [Getting Started](wiki/Getting-Started.md)
- Behavior customization: [Customizing Behavior](wiki/Customizing-Behavior.md)
- Extension points: [Extensions Overview](wiki/Extensions-Overview.md)

## Build and Test

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```
