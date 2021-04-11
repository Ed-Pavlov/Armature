using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property marked with <see cref="InjectAttribute" /> with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record UnitIsPropertyWithInjectIdMatcher : UnitByInjectPointIdMatcherBase
  {
    [DebuggerStepThrough]
    public UnitIsPropertyWithInjectIdMatcher(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId)
      => UnitIsPropertyWithAttributeMatcher<InjectAttribute>.GetPropertyAttribute(unitId);
  }
}
