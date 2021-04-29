using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  /// <summary>
  ///  Adds a <see cref="IfLastUnitMatches"/>  pattern with a build action passed into a constructor to the tree passed to the
  /// <see cref="Apply(IPatternTreeNode)"/> method
  /// </summary>
  public abstract class LastUnitTuner : BuildActionExtensibility, ITuner
  {
    protected LastUnitTuner(IUnitPattern unitPattern, IBuildAction action, int weight) : base(unitPattern, action, weight) { }

    void ITuner.Apply(IPatternTreeNode patternTreeNode)
    {
      patternTreeNode
       .GetOrAddNode(new IfLastUnitMatches(UnitPattern, Weight))
       .UseBuildAction(BuildAction, BuildStage.Create);

      Apply(patternTreeNode);
    }

    /// <summary>
    /// Can be overriden to add extra logic in addition to implemented in this class
    /// </summary>
    protected virtual void Apply(IPatternTreeNode patternTreeNode) { }
  }
}
