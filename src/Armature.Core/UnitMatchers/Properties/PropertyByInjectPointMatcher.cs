using System.Diagnostics;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  /// Matches property marked with <see cref="InjectAttribute"/> with specified <see cref="InjectAttribute.InjectionPointId"/>
  /// </summary>
  public class PropertyByInjectPointMatcher : InjectPointByIdMatcher
  {
    [DebuggerStepThrough]
    public PropertyByInjectPointMatcher(object injectPointId = null) : base(injectPointId) {}

    protected override InjectAttribute GetInjectPointAttribute(UnitInfo unitInfo) => PropertyByAttributeMatcher<InjectAttribute>.GetPropertyAttribute(unitInfo);
  }
}