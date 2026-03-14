using System.Linq.Expressions;

using ConstructorCustomization.AutoFixture.Customization.Application.Ports;

namespace ConstructorCustomization.AutoFixture.Customization.Adapters;

/// <summary>
/// Default parser that extracts property names from member expressions.
/// </summary>
internal sealed class PropertyExpressionParser : IPropertyExpressionParser
{
    /// <inheritdoc />
    public string GetPropertyName(LambdaExpression propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);

        var body = propertyExpression.Body;
        if (body is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
        {
            body = unaryExpression.Operand;
        }

        if (body is not MemberExpression memberExpression)
        {
            throw new ArgumentException("The expression must be a member expression.", nameof(propertyExpression));
        }

        return memberExpression.Member.Name;
    }
}
