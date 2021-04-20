using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  /// <summary>
  ///   Stores and applies a plan how to build value to inject.
  /// </summary>
  public class LastUnitTuner : BuildActionExtensibility, ITuner
  {
    public LastUnitTuner(IUnitPattern unitPattern, IBuildAction action, int weight)
      : base(unitPattern, action, weight) { }

    void ITuner.Apply(IPatternTreeNode patternTreeNode)
    {
      patternTreeNode
       .GetOrAddNode(new IfLastUnitMatches(UnitPattern, Weight))
       .UseBuildAction(BuildStage.Create, BuildAction);

      Apply(patternTreeNode);
    }

    /// <summary>
    ///   Can be overriden to add extra logic in addition to implemented in <see cref="ITuner.Apply" />
    /// </summary>
    protected virtual void Apply(IPatternTreeNode patternTreeNode) { }
  }
}
