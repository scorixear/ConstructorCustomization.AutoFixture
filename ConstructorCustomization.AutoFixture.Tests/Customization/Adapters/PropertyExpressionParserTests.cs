namespace ConstructorCustomization.AutoFixture.Tests.Customization.Adapters;

[TestFixture]
internal sealed class PropertyExpressionParserTests
{
    [Test]
    public void GetPropertyName_FromMemberExpression_ReturnsMemberName()
    {
        var parser = new PropertyExpressionParser();

        var name = parser.GetPropertyName((Expression<Func<PersonForExpression, string>>)(p => p.Name));

        Assert.That(name, Is.EqualTo("Name"));
    }

    [Test]
    public void GetPropertyName_FromUnaryConvertExpression_ReturnsMemberName()
    {
        var parser = new PropertyExpressionParser();

        var name = parser.GetPropertyName((Expression<Func<PersonForExpression, object>>)(p => p.Age));

        Assert.That(name, Is.EqualTo("Age"));
    }

    [Test]
    public void GetPropertyName_WhenExpressionIsNotMember_ThrowsArgumentException()
    {
        var parser = new PropertyExpressionParser();

        Assert.That(
            () => parser.GetPropertyName((Expression<Func<PersonForExpression, string>>)(p => p.ToString()!)),
            Throws.ArgumentException.With.Message.Contains("member expression"));
    }

    [Test]
    public void GetPropertyName_WhenExpressionIsNull_ThrowsArgumentNullException()
    {
        var parser = new PropertyExpressionParser();

        Assert.That(() => parser.GetPropertyName(null!), Throws.ArgumentNullException);
    }

    private sealed class PersonForExpression
    {
        public string Name { get; init; } = string.Empty;

        public int Age { get; init; }
    }
}
