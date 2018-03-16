using Armature.Interface;

namespace Armature.Framework.BuildActions
{
  public class GetInjectPointConstructorBuildAction : GetAttributedConstructorBuildAction<InjectAttribute>
  {
    public GetInjectPointConstructorBuildAction(object injectPointId = null) : base(inject => Equals(inject.InjectionPointId, injectPointId))
    {
    }
  }
}