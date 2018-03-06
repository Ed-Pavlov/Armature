using System;
using System.Collections.Generic;
using Armature.Common;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// The collection of build plans. Build plan of the unit is units sequence matchers containing build action factories.
  /// All build plans are contained in a forest of trees form. See <see cref="IUnitSequenceMatcher"/> for details. 
  /// </summary>
  public class BuildPlansCollection
  {
    private readonly Root _root = new Root();

    /// <summary>
    /// Returns build actions which should be performed to build an unit represented by the last item of <paramref name="buildSequence"/>
    /// </summary>
    /// <param name="buildSequence">The sequence of units representing a build session, the last one is the unit to be built, 
    /// the previous are the context of the build session. Each next unit info is the dependency of the previous one. </param>
    /// <remarks>If there is type A which depends on class B, during building A, B should be built and build sequence will be
    /// [A, B] in this case.</remarks>
    /// <returns>Returns all matched build actions for the <see cref="buildSequence"/>. All actions are grouped by a building stage
    /// and coupled with a "weight of matching". See <see cref="MatchedBuildActions"/> type declaration for details.</returns>
    public MatchedBuildActions GetBuildActions([NotNull] IList<UnitInfo> buildSequence)
    {
      if (buildSequence == null) throw new ArgumentNullException("buildSequence");
      return _root.GetBuildActions(buildSequence.GetTail(0), 0);
    }

    /// <summary>
    /// Adds a root build step (tree) into the forest of trees 
    /// </summary>
    /// <param name="unitSequenceMatcher">The build step to add, it can have child build steps of can be filled with them later</param>
    public void AddUnitMatcher([NotNull] IUnitSequenceMatcher unitSequenceMatcher)
    {
      if (unitSequenceMatcher == null) throw new ArgumentNullException("unitSequenceMatcher");
      _root.Children.Add(unitSequenceMatcher);
    }

    /// <summary>
    /// Collection of root build steps
    /// </summary>
    public ICollection<IUnitSequenceMatcher> Children
    {
      get { return _root.Children; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Reuse implementation of <see cref="T:Armature.Core.UnitSequenceMatcherBase" />
    /// to implement <see cref="T:Armature.Core.BuildPlansCollection" /> public interface
    /// </summary>
    private class Root : UnitSequenceMatcherBase
    {
      public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputMatchingWeight)
      {
        return GetChildrenActions(inputMatchingWeight, buildingUnitsSequence);
      }

      public override bool Equals(IUnitSequenceMatcher other)
      {
        throw new NotSupportedException();
      }
    }
  }
}