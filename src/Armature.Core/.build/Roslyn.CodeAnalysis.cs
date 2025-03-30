// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

#if !NETSTANDARD
[AttributeUsage(AttributeTargets.Method, Inherited=false)]
public sealed class DoesNotReturnAttribute : Attribute {}
#endif