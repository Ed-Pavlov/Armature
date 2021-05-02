using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class exposing the collection of children nodes, gathering, and merging build actions from children.
  /// </summary>
  public abstract class PatternTreeNodeWithChildren : PatternTreeNode
  {
    private HashSet<IPatternTreeNode>? _children;
    private HashSet<IPatternTreeNode>  LazyChildren => _children ??= new HashSet<IPatternTreeNode>();

    protected PatternTreeNodeWithChildren(int weight) : base(weight) { }

    public override ICollection<IPatternTreeNode> Children => LazyChildren;

    protected WeightedBuildActionBag? GetOwnOrChildrenBuildActions(ArrayTail<UnitId> unitSequence, long inputWeight)
    {
      WeightedBuildActionBag? buildActionBag = null;

      if(unitSequence.Length == 1)
      {
        buildActionBag = GetOwnBuildActions(Weight + inputWeight);

        if(buildActionBag is null)
          Log.WriteLine(LogLevel.Trace, () => $"{this}{LogConst.NoMatch}");
        else
          using(Log.Block(LogLevel.Verbose, ToString)) // pass group method, do not call ToString
          {
            // ReSharper disable once RedundantArgumentDefaultValue
            buildActionBag.ToLog(LogLevel.Verbose);
          }
      }
      else
      {
        using(Log.Deferred(
          LogLevel.Verbose,
          writeDeferredLog =>
          {
            var logLevel = buildActionBag is null ? LogLevel.Trace : LogLevel.Verbose;

            string GetLogLine() => $"{this} => pass further";

            if(writeDeferredLog is null)
              Log.WriteLine(logLevel, GetLogLine);
            else
              using(Log.Block(logLevel, GetLogLine))
                writeDeferredLog();
          }))
        {
          // pass the rest of the sequence to children and return their actions
          buildActionBag = GetChildrenActions(unitSequence.GetTail(1), inputWeight + Weight);
        }
      }

      return buildActionBag;
    }

    /// <summary>
    ///   Gathers and merges build actions from all children nodes.
    /// </summary>
    /// <param name="unitSequence">The sequence of units building in this build session.</param>
    /// <param name="inputMatchingWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
    [DebuggerStepThrough]
    protected WeightedBuildActionBag? GetChildrenActions(ArrayTail<UnitId> unitSequence, long inputMatchingWeight)
      => _children?.Aggregate(
        (WeightedBuildActionBag?)null,
        (current, child) => current.Merge(child.GatherBuildActions(unitSequence, inputMatchingWeight)));
  }
}
