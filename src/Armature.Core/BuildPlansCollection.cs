using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   The collection of build plans. Build plan of the unit is the tree of units sequence matchers containing build actions.
  ///   All build plans are contained as a forest of trees.
  ///   See <see cref="IUnitSequenceMatcher" /> for details.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new Builder(...)
  ///   {
  ///   new AnyUnitSequenceMatcher
  ///   {
  ///   new LeafUnitSequenceMatcher(ConstructorMatcher.Instance, 0)
  ///   .AddBuildAction(BuildStage.Create, new GetLongesConstructorBuildAction()),
  ///   new LeafUnitSequenceMatcher(ParameterMatcher.Instance, ParameterMatchingWeight.Lowest)
  ///   .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  ///   }
  ///   };
  /// </remarks>
  public class BuildPlansCollection : IUnitSequenceMatcher, IEnumerable
  {
    private readonly Root _root = new Root();

    public IEnumerator GetEnumerator() => throw new NotSupportedException();

    /// <summary>
    ///   Forest of <see cref="IUnitSequenceMatcher" /> trees
    /// </summary>
    public ICollection<IUnitSequenceMatcher> Children => _root.Children;

    /// <summary>
    ///   Returns build actions which should be performed to build an unit represented by the last item of <paramref name="buildingUnitsSequence" />
    /// </summary>
    /// <param name="buildingUnitsSequence">
    ///   The sequence of units representing a build session, the last one is the unit to be built,
    ///   the previous are the context of the build session. Each next unit info is the dependency of the previous one.
    /// </param>
    /// <param name="inputWeight">
    ///   The weight of matching which used by children matchers to calculate a final weight of matching
    ///   Not applicable to BuildPlansCollection in common case
    /// </param>
    /// <remarks>
    ///   If there is type A which depends on class B, during building A, B should be built and build sequence will be
    ///   [A, B] in this case.
    /// </remarks>
    /// <returns>
    ///   Returns all matched build actions for the <paramref name="buildingUnitsSequence" />. All actions are grouped by a building stage
    ///   and coupled with a "weight of matching". See <see cref="MatchedBuildActions" /> type declaration for details.
    /// </returns>
    public MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight = 0)
    {
      if (buildingUnitsSequence.Length == 0) throw new ArgumentException(nameof(buildingUnitsSequence));

      return _root.GetBuildActions(buildingUnitsSequence, 0);
    }

    public bool Equals(IUnitSequenceMatcher other) => ReferenceEquals(this, other);

    IUnitSequenceMatcher IUnitSequenceMatcher.AddBuildAction(object buildStage, IBuildAction buildAction) => throw new NotSupportedException();

    public void Add([NotNull] IUnitSequenceMatcher unitSequenceMatcher) => Children.Add(unitSequenceMatcher);

    /// <summary>
    ///   Reuse implementation of <see cref="UnitSequenceMathcherWithChildren" /> to implement <see cref="BuildPlansCollection" /> public interface
    /// </summary>
    private class Root : UnitSequenceMathcherWithChildren
    {
      [DebuggerStepThrough]
      public Root() : base(0) { }

      [DebuggerStepThrough]
      public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight) =>
        GetChildrenActions(inputWeight, buildingUnitsSequence);

      [DebuggerStepThrough]
      public override bool Equals(IUnitSequenceMatcher other) => throw new NotSupportedException();
    }
  }
}