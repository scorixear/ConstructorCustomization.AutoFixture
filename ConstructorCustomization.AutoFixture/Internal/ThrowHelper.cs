using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ConstructorCustomization.AutoFixture.Internal;

internal static class ThrowHelper
{
    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
#if NET6_0_OR_GREATER
        global::System.ArgumentNullException.ThrowIfNull(argument, paramName);
#else
        if (argument is null)
            throw new global::System.ArgumentNullException(paramName);
#endif
    }

    public static void ThrowIfNullOrWhiteSpace(
        [NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
#if NET7_0_OR_GREATER
        global::System.ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
#else
        if (string.IsNullOrWhiteSpace(argument))
            throw new global::System.ArgumentException("The value cannot be null or whitespace.", paramName);
#endif
    }
}
