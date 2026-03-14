using System.Linq.Expressions;

namespace ConstructorCustomization.AutoFixture.Customization.Application.Ports;

/// <summary>
/// Parses property expressions into property names.
/// </summary>
public interface IPropertyExpressionParser
{
    /// <summary>
    /// Gets the property name represented by the provided expression.
    /// </summary>
    /// <param name="propertyExpression">A lambda expression that targets a property on the customized type.</param>
    /// <returns>The property name extracted from the expression.</returns>
    string GetPropertyName(LambdaExpression propertyExpression);
}
