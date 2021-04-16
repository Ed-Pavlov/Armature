using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit is an argument for a parameter marked with <see cref="InjectAttribute"/> attribute
  /// with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record IsParameterWithInjectIdMatcher : IsInjectPointAttributeMatcher
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public IsParameterWithInjectIdMatcher(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId) => IsParameterWithAttributeMatcher<InjectAttribute>.GetParameterAttribute(unitId);
  }
}
