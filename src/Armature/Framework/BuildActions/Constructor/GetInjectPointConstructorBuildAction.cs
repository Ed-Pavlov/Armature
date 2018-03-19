using Armature.Interface;

namespace Armature.Framework.BuildActions.Constructor
{
  public class GetInjectPointConstructorBuildAction : GeConstructorBytAttributeBuildAction<InjectAttribute>
  {
    public GetInjectPointConstructorBuildAction(object injectPointId = null) : base(inject => Equals(inject.InjectionPointId, injectPointId))
    {
    }
  }
}