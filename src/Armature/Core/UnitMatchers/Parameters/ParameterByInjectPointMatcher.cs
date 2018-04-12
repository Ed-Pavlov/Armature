using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Parameters
{
  public class ParameterByInjectPointMatcher : InjectPointByIdMatcher
  {
    [DebuggerStepThrough]
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(injectPointId){}

    protected override InjectAttribute GetInjectPointAttribute(UnitInfo unitInfo) => ParameterByAttributeMatcher<InjectAttribute>.GetParameterAttribute(unitInfo);
  }
}