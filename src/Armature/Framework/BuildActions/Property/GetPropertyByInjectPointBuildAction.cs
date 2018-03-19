using Armature.Interface;

namespace Armature.Framework.BuildActions.Property
{
  public class GetPropertyByInjectPointBuildAction : GetPropertyByAttributeBuildAction<InjectAttribute>
  {
    public GetPropertyByInjectPointBuildAction(object pointId = null) : base(_ => _.InjectionPointId == pointId)
    {
    }
  }
}