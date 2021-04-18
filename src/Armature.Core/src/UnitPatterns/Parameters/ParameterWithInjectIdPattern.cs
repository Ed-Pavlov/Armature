using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a method parameter marked with <see cref="InjectAttribute"/> attribute
  /// with an optional <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record ParameterWithInjectIdPattern : InjectPointAttributePattern
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public ParameterWithInjectIdPattern(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId) => ParameterWithAttributePattern<InjectAttribute>.GetParameterAttribute(unitId);
  }
}
