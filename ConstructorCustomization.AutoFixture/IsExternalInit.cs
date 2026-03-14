// Required for 'init' property accessors on netstandard2.1 (.NET 5+ already includes this type).
#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif
