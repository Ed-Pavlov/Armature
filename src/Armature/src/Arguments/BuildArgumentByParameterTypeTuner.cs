using Armature.Core;

namespace Armature
{
  public class BuildArgumentByParameterTypeTuner : IArgumentTuner
  {
    public void Apply(IPatternTreeNode patternTreeNode)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnitMatches(MethodArgumentPattern.Instance))
        .UseBuildAction(BuildArgumentForMethodParameter.Instance, BuildStage.Create);
  }
}
