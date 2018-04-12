namespace Armature.Core.BuildActions.Constructor
{
  public class GetInjectPointConstructorBuildAction : GetConstructorBytAttributeBuildAction<InjectAttribute>
  {
    public GetInjectPointConstructorBuildAction(object injectPointId = null) : base(inject => Equals(inject.InjectionPointId, injectPointId))
    {
    }
  }
}