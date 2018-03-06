using Armature.Interface;

namespace Armature.Framework
{
  public class ConstructorByInjectPointIdMatcher : ConstructorByAttributeMatcher<InjectAttribute>
  {
    public ConstructorByInjectPointIdMatcher(object injectPointId = null) : base(inject => Equals(inject.InjectionPointId, injectPointId))
    {
    }
  }
}