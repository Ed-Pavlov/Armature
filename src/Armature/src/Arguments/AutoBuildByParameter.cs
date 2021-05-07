using Armature.Core;

namespace Armature
{
  public static class AutoBuildByParameter
  {
    public static IArgumentTuner Type { get; } = new ArgumentTuner(
      node =>
      {
        node
         .GetOrAddNode(new IfLastUnitMatches(Static<IsMethodParameter>.Instance))
         .UseBuildAction(BuildArgumentByParameterType.Instance, BuildStage.Create);

        node
         .GetOrAddNode(new IfLastUnitMatches(Static<IsMethodParametersList>.Instance))
         .UseBuildAction(Static<BuildMethodArgumentsInDirectOrder>.Instance, BuildStage.Create);
      }); 
  }
}
