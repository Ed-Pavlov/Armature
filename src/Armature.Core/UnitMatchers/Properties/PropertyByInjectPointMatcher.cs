using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Properties
{
  public class PropertyByInjectPointMatcher : InjectPointByIdMatcher
  {
    [DebuggerStepThrough]
    public PropertyByInjectPointMatcher(object injectPointId = null) : base(injectPointId) {}

    protected override InjectAttribute GetInjectPointAttribute(UnitInfo unitInfo) => PropertyByAttributeMatcher<InjectAttribute>.GetPropertyAttribute(unitInfo);
  }
}