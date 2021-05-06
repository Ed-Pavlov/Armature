using Armature.Core;

namespace Armature
{
  public static class AutoBuildByParameter
  {
    public static IArgumentTuner Type { get; } = new ArgumentTuner(
      node =>
      {
        node
         .GetOrAddNode(new IfLastUnitMatches(Static<MethodArgumentPattern>.Instance))
         .UseBuildAction(BuildArgumentByParameter.Instance, BuildStage.Create);

        node
         .GetOrAddNode(new IfLastUnitMatches(Static<MethodArgumentListPattern>.Instance))
         .UseBuildAction(Static<BuildMethodArguments>.Instance, BuildStage.Create);
      }); 
  }
}
