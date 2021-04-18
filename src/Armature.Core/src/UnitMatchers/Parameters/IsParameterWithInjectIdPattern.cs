using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit is an argument for a parameter marked with <see cref="InjectAttribute"/> attribute
  /// with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record IsParameterWithInjectIdPattern : IsInjectPointAttributePattern
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public IsParameterWithInjectIdPattern(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId) => IsParameterWithAttributePattern<InjectAttribute>.GetParameterAttribute(unitId);
  }
}
