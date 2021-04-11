using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter marked with <see cref="InjectAttribute" /> with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record UnitIsParameterWithInjectIdMatcher : UnitByInjectPointIdMatcherBase
  {
    [DebuggerStepThrough]
    public UnitIsParameterWithInjectIdMatcher(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId)
      => UnitIsParameterWithAttributeMatcher<InjectAttribute>.GetParameterAttribute(unitId);
  }
}
