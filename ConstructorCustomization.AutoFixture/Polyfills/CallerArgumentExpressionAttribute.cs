// Polyfill: CallerArgumentExpressionAttribute was added to the BCL in .NET 6.
// The C# compiler uses it at compile time; defining it here enables the feature on older targets.
#if !NET6_0_OR_GREATER
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
    public CallerArgumentExpressionAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
}
#endif
