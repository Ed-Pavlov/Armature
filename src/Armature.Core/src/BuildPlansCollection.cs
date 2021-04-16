using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.Logging;


namespace Armature.Core
{
  /// <summary>
  ///   The collection of build plans. Build plan of the unit is the tree of units sequence matchers containing build actions.
  ///   All build plans are contained as a forest of trees.
  ///   See <see cref="IQuery" /> for details.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new Builder(...)
  ///   {
  ///     new AnyUnitSequenceMatcher
  ///     {
  ///       new LeafUnitSequenceMatcher(ConstructorMatcher.Instance, 0)
  ///         .AddBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
  ///       new LeafUnitSequenceMatcher(ParameterMatcher.Instance, ParameterMatchingWeight.Lowest)
  ///         .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  ///     }
  ///   };
  /// </remarks>
  public class BuildPlansCollection : IQuery, IEnumerable
  {
    private readonly Root _root = new();

    /// <summary>
    ///   Forest of <see cref="IQuery" /> trees
    /// </summary>
    public ICollection<IQuery> Children => _root.Children;

    public void Add(IQuery query) => Children.Add(query);

    public BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight = 0)
    {
      if(unitSequence.Length == 0) throw new ArgumentException(nameof(unitSequence));

      return _root.GatherBuildActions(unitSequence, 0);
    }

    public void PrintToLog()
    {
      using(Log.Enabled())
      {
        _root.PrintToLog();
      }
    }

    public bool Equals(IQuery other) => ReferenceEquals(this, other);

    /// <summary>
    ///   Reuse implementation of <see cref="QueryWithChildren" /> to implement <see cref="BuildPlansCollection" /> public interface
    /// </summary>
    private class Root : QueryWithChildren
    {
      [DebuggerStepThrough]
      public Root() : base(0) { }

      [DebuggerStepThrough]
      public override BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
        => GetChildrenActions(inputWeight, unitSequence);

      [DebuggerStepThrough]
      public override bool Equals(IQuery other) => throw new NotSupportedException();
    }

    IEnumerator IEnumerable.  GetEnumerator()                                             => throw new NotSupportedException();
    IQuery IQuery.UseBuildAction(object buildStage, IBuildAction buildAction) => throw new NotSupportedException();
  }
}
