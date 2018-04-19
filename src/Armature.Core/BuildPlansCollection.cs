using System;
using System.Collections.Generic;
using System.Diagnostics;
using Resharper.Annotations;
using Armature.Core.Common;
using Armature.Core.UnitSequenceMatcher;

namespace Armature.Core
{
  /// <summary>
  ///   The collection of build plans. Build plan of the unit is the tree of units sequence matchers containing build actions.
  ///   All build plans are contained as a forest of trees.
  /// See <see cref="IUnitSequenceMatcher" /> for details.
  /// </summary>
  public class BuildPlansCollection
  {
    private readonly Root _root = new Root();

    /// <summary>
    ///   Forest of <see cref="IUnitSequenceMatcher"/> trees
    /// </summary>
    public ICollection<IUnitSequenceMatcher> Children => _root.Children;

    /// <summary>
    ///   Returns build actions which should be performed to build an unit represented by the last item of <paramref name="buildSequence" />
    /// </summary>
    /// <param name="buildSequence">
    ///   The sequence of units representing a build session, the last one is the unit to be built,
    ///   the previous are the context of the build session. Each next unit info is the dependency of the previous one.
    /// </param>
    /// <remarks>
    ///   If there is type A which depends on class B, during building A, B should be built and build sequence will be
    ///   [A, B] in this case.
    /// </remarks>
    /// <returns>
    ///   Returns all matched build actions for the <paramref name="buildSequence"/>. All actions are grouped by a building stage
    ///   and coupled with a "weight of matching". See <see cref="MatchedBuildActions" /> type declaration for details.
    /// </returns>
    public MatchedBuildActions GetBuildActions([NotNull] IList<UnitInfo> buildSequence)
    {
      if (buildSequence == null) throw new ArgumentNullException(nameof(buildSequence));

      return _root.GetBuildActions(buildSequence.GetTail(0), 0);
    }

    /// <summary>
    ///   Reuse implementation of <see cref="UnitSequenceMathcherWithChildren" /> to implement <see cref="BuildPlansCollection" /> public interface
    /// </summary>
    private class Root : UnitSequenceMathcherWithChildren
    {
      [DebuggerStepThrough]
      public Root() : base(0)
      {
      }

      [DebuggerStepThrough]
      public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight) =>
        GetChildrenActions(inputWeight, buildingUnitsSequence);

      [DebuggerStepThrough]
      public override bool Equals(IUnitSequenceMatcher other) => throw new NotSupportedException();
    }
  }
}