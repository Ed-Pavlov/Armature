using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property marked with <see cref="InjectAttribute" /> with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record PropertyByInjectPointMatcher : InjectPointByIdMatcher
  {
    [DebuggerStepThrough]
    public PropertyByInjectPointMatcher(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetInjectPointAttribute(UnitId unitId)
      => PropertyByAttributeMatcher<InjectAttribute>.GetPropertyAttribute(unitId);
  }
}
