using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit is an argument for a property marked with <see cref="InjectAttribute"/> attribute
  /// with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record IsPropertyWithInjectIdMatcher : IsInjectPointAttributeMatcher
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public IsPropertyWithInjectIdMatcher(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId) => IsPropertyWithAttributeMatcher<InjectAttribute>.GetPropertyAttribute(unitId);
  }
}
