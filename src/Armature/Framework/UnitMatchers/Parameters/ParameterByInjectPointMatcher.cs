using System.Diagnostics;
using Armature.Core;
using Armature.Interface;

namespace Armature.Framework.UnitMatchers.Parameters
{
  public class ParameterByInjectPointMatcher : InjectPointByIdMatcher
  {
    [DebuggerStepThrough]
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(injectPointId){}

    protected override InjectAttribute GetInjectPointAttribute(UnitInfo unitInfo) => ParameterByAttributeMatcher<InjectAttribute>.GetParameterAttribute(unitInfo);
  }
}