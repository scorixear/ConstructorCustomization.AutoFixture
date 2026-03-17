
<h1>
<img src="./Docs/logo.png" alt="Icon" width="80" />&nbsp;&nbsp;ConstructorCustomization.AutoFixture</h1>

[![License](https://img.shields.io/badge/license-MIT-green)](https://raw.githubusercontent.com/scorixear/ConstructorCustomization.AutoFixture/main/LICENSE)
[![Release](https://github.com/scorixear/ConstructorCustomization.AutoFixture/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/scorixear/ConstructorCustomization.AutoFixture/actions/workflows/publish-nuget.yml)
[![NuGet Version](https://img.shields.io/nuget/v/ConstructorCustomization.AutoFixture)](https://www.nuget.org/packages/ConstructorCustomization.AutoFixture/)
[![NuGet downloads](https://img.shields.io/nuget/dt/ConstructorCustomization.AutoFixture)](https://www.nuget.org/packages/ConstructorCustomization.AutoFixture/)

Constructor-first customization for AutoFixture with clean defaults, per-test overrides, and extension points for advanced object creation.

## Why This Package?

`AutoFixture` provides powerful Fixture creation and customization capabilities.
But it creates objects by setting properties after the constructor is called.

*But what if you have Guards and follow the Always-Valid-Model principle?*

`ConstructorCustomization.AutoFixture` provides a constructor-first customization model for `AutoFixture`. This is a package only built on `AutoFixture` and provides an `ICustomization` implementation with minimal effort to get started but highly extensible for advanced scenarios.

### ✨ Features
- Create simple objects in a one-liner.
- Use Fluent API to override defaults on a per-test basis.
- Explicitly map constructor parameters to properties.
- Pluggable extension model for matching and specimen/value creation behavior.
- Fully compatible with AutoFixture ecosystem and existing customizations.

### 🔧 How it works
Under the hood, `ConstructorCustomization` uses reflection to retrieve a constructor and matches property names to parameter names.
Values are created using `AutoFixture`'s existing value creation.

Constructor selection, parameter matching and value creation can all be customized using the provided extension models.


## 📦 Install

```bash
dotnet add package ConstructorCustomization.AutoFixture
```

## ⚡ Quick Entry

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

## 🧩 Simple Usage Pattern

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

## ✅ Capabilities at a Glance

- Constructor-first object creation for test models.
- Stable defaults with `SetDefault(...)`.
- Per-test overrides with `With(...)` and `Without(...)`.
- Explicit parameter-to-property mapping with `MatchParameterToProperty(...)`.
- Deferred value generation with factory delegates.
- Pluggable extension model for matching and specimen/value creation behavior.

## 📚 Documentation and Wiki

- Wiki home: [Home](https://github.com/scorixear/ConstructorCustomization.AutoFixture/wiki)
- Getting started: [Getting Started](https://github.com/scorixear/ConstructorCustomization.AutoFixture/wiki/Getting-Started)
- Behavior customization: [Customizing Behavior](https://github.com/scorixear/ConstructorCustomization.AutoFixture/wiki/Customizing-Behavior)
- Extension points: [Extensions Overview](https://github.com/scorixear/ConstructorCustomization.AutoFixture/wiki/Extensions-Overview)
- How it Works: [Creation Pipeline](https://github.com/scorixear/ConstructorCustomization.AutoFixture/wiki/Creation-Pipeline)
