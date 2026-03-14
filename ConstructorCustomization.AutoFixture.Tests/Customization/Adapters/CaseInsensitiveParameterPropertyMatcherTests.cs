using ConstructorCustomization.AutoFixture.Tests.Shared;

namespace ConstructorCustomization.AutoFixture.Tests.Customization.Adapters;

[TestFixture]
internal sealed class CaseInsensitiveParameterPropertyMatcherTests
{
    [Test]
    public void TryGetPropertyName_WhenDirectMatchExists_ReturnsTrueAndMatchedName()
    {
        var matcher = new CaseInsensitiveParameterPropertyMatcher(StringComparer.OrdinalIgnoreCase);
        var parameter = GetParameter("firstName");

        var matched = matcher.TryGetPropertyName(parameter, ["FirstName", "Age"], out var propertyName);

        Assert.Multiple(() =>
        {
            Assert.That(matched, Is.True);
            Assert.That(propertyName, Is.EqualTo("FirstName"));
        });
    }

    [Test]
    public void TryGetPropertyName_WhenNoMatch_ReturnsFalse()
    {
        var matcher = new CaseInsensitiveParameterPropertyMatcher(StringComparer.Ordinal);
        var parameter = GetParameter("age");

        var matched = matcher.TryGetPropertyName(parameter, ["Name"], out var propertyName);

        Assert.Multiple(() =>
        {
            Assert.That(matched, Is.False);
            Assert.That(propertyName, Is.Empty);
        });
    }

    [Test]
    public void TryGetPropertyName_WhenCaseSensitiveComparerUsesPascalFallback_ReturnsTrue()
    {
        var matcher = new CaseInsensitiveParameterPropertyMatcher(StringComparer.Ordinal);
        var parameter = GetParameter("firstName");

        var matched = matcher.TryGetPropertyName(parameter, ["FirstName"], out var propertyName);

        Assert.Multiple(() =>
        {
            Assert.That(matched, Is.True);
            Assert.That(propertyName, Is.EqualTo("FirstName"));
        });
    }

    [Test]
    public void Ctor_WhenComparerIsNull_ThrowsArgumentNullException()
    {
        Assert.That(() => new CaseInsensitiveParameterPropertyMatcher(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void TryGetPropertyName_WhenArgumentsNull_ThrowsArgumentNullException()
    {
        var matcher = new CaseInsensitiveParameterPropertyMatcher(StringComparer.OrdinalIgnoreCase);
        var parameter = GetParameter("firstName");

        Assert.Multiple(() =>
        {
            Assert.That(() => matcher.TryGetPropertyName(null!, ["FirstName"], out _), Throws.ArgumentNullException);
            Assert.That(() => matcher.TryGetPropertyName(parameter, null!, out _), Throws.ArgumentNullException);
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void TryGetPropertyName_WhenParameterNameMissing_ReturnsFalse(string? parameterName)
    {
        var matcher = new CaseInsensitiveParameterPropertyMatcher(StringComparer.OrdinalIgnoreCase);
        var parameter = new NamedParameterInfo(parameterName);

        var matched = matcher.TryGetPropertyName(parameter, ["FirstName"], out var propertyName);

        Assert.Multiple(() =>
        {
            Assert.That(matched, Is.False);
            Assert.That(propertyName, Is.Empty);
        });
    }

    private static ParameterInfo GetParameter(string name)
    {
        var ctor = typeof(CtorParameterCarrier).GetConstructor([typeof(string), typeof(int)])!;
        return ctor.GetParameters().Single(p => p.Name == name);
    }

    private sealed class NamedParameterInfo(string? name) : ParameterInfo
    {
        public override string? Name => name;
    }
}
