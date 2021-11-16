// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class NotNullWhenAttribute : Attribute
{
  public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

  public bool ReturnValue { get; }
}