using Armature.Interface;

namespace Armature.Framework
{
  public class ParameterByInjectPointMatcher : ParameterByAttributeMatcher<InjectAttribute>
  {
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId))
    {}
  }
}