using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter marked with <see cref="InjectAttribute" /> with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public class ParameterByInjectPointMatcher : InjectPointByIdMatcher
  {
    [DebuggerStepThrough]
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute GetInjectPointAttribute(UnitInfo unitInfo) =>
      ParameterByAttributeMatcher<InjectAttribute>.GetParameterAttribute(unitInfo);
  }
}