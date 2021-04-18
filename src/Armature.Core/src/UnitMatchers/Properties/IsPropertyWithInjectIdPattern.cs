using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit is an argument for a property marked with <see cref="InjectAttribute"/> attribute
  /// with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record IsPropertyWithInjectIdPattern : IsInjectPointAttributePattern
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public IsPropertyWithInjectIdPattern(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId) => IsPropertyWithAttributePattern<InjectAttribute>.GetPropertyAttribute(unitId);
  }
}
