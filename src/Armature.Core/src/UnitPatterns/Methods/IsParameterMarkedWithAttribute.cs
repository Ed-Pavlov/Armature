using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a method parameter marked with <see cref="InjectAttribute"/> attribute
  /// with an optional <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record IsParameterMarkedWithAttribute : InjectPointAttributePatternBase
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public IsParameterMarkedWithAttribute(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId)
      => unitId.Kind is ParameterInfo parameterInfo ? parameterInfo.GetCustomAttribute<InjectAttribute>() : default;
  }
}
