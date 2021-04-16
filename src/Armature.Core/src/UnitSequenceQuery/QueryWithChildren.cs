using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class exposing the collection of children matchers, gathering and merging build actions from children with its own.
  /// </summary>
  public abstract class QueryWithChildren : Query
  {
    private HashSet<IQuery>? _children;

    protected QueryWithChildren(int weight) : base(weight) { }

    private HashSet<IQuery> LazyChildren => _children ??= new HashSet<IQuery>();

    public override ICollection<IQuery> Children => LazyChildren;

    /// <summary>
    ///   Gets and merges matched actions from all children matchers
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
    /// <param name="unitSequence">The sequence of units building in this build session.</param>
    [DebuggerStepThrough]
    protected BuildActionBag? GetChildrenActions(int inputMatchingWeight, ArrayTail<UnitId> unitSequence)
      => _children?.Aggregate(
        (BuildActionBag?) null,
        (current, child) => current.Merge(child.GatherBuildActions(unitSequence, inputMatchingWeight)));

    protected BuildActionBag? GetActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      BuildActionBag? buildActionBag;

      if(unitSequence.Length == 1)
      {
        buildActionBag = GetOwnActions(Weight + inputWeight);

        if(buildActionBag is null)
          Log.WriteLine(LogLevel.Trace, () => string.Format("{0}{{not matched}}", this));
        else
          using(Log.Block(LogLevel.Verbose, this.ToString)) // pass group method, do not call ToString
          {
            // ReSharper disable once RedundantArgumentDefaultValue
            buildActionBag.ToLog(LogLevel.Verbose);
          }
      }
      else
      {
        Log.WriteLine(LogLevel.Verbose, ToString); // pass group method, do not call ToString

        // pass the rest of the sequence to children and return their actions
        using(Log.AddIndent())
          buildActionBag = GetChildrenActions(inputWeight + Weight, unitSequence.GetTail(1));
      }

      return buildActionBag;
    }
  }
}
