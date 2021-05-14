using System;
using System.Collections.Generic;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// A pattern used to match a unit in the building unit sequence.
  /// </summary>
  public interface IPatternTreeNode : IEquatable<IPatternTreeNode>, ILogable
  {
    /// <summary>
    /// The collection of all children nodes used to find existing one, add new, or replace one with another.
    /// All nodes with their children are a pattern tree.  
    /// </summary>
    ICollection<IPatternTreeNode> Children { get; }
    
    BuildActionBag BuildActions { get; }

    /// <summary>
    ///   Returns build actions which should be performed to build a unit represented by the last item of <paramref name="unitSequence" />
    /// </summary>
    /// <param name="unitSequence">
    ///   The sequence of units representing a build session, the last one is the unit to be built,
    ///   the previous are the context of the build session. Each next unit is the dependency of the previous one.
    /// </param>
    /// <param name="inputWeight">
    ///   The weight of matching which used by children matchers to calculate a final weight of matching
    /// </param>
    /// <remarks>
    ///   If there is type A which depends on class B, during building A, B should be built and build sequence will be [A, B] in this case.
    /// </remarks>
    /// <returns>
    ///   Returns all matched build actions for the <paramref name="unitSequence" />. All actions are grouped by a building stage
    ///   and coupled with a "weight of matching". See <see cref="WeightedBuildActionBag" /> for details.
    /// </returns>
    WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight);
  }
}
