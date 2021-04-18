using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for an object property marked with <see cref="InjectAttribute"/> attribute
  /// with an optional <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record PropertyWithInjectIdPattern : InjectPointAttributePattern
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public PropertyWithInjectIdPattern(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId) => PropertyWithAttributePattern<InjectAttribute>.GetPropertyAttribute(unitId);
  }
}
