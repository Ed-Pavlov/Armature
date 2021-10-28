using System;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Skips all units in the building unit sequence and pass the last (under construction) unit  to <see cref="IPatternTreeNode.Children" />.
  /// </summary>
  public class SkipAllUnits : PatternTreeNodeWithChildrenBase
  {
    public SkipAllUnits(int weight = WeightOf.SkipAll) : base(weight) { }

    public override BuildActionBag BuildActions
      => throw new NotSupportedException(
           "This pattern is used to skip a building unit sequence to the unit under construction (the last one) and pass it to children."
         + "It can't contain build actions due to they are used to build the unit under construction only."
         );

    /// <summary>
    ///   Decreases the matching weight by each skipped unit then pass unit under construction to children nodes
    /// </summary>
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      var unitsToSkipCount = unitSequence.Length;
      var matchingWeight   = Weight + inputWeight - unitsToSkipCount;

      using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipAllUnits)))
      {
        Log.WriteLine(LogLevel.Verbose, $"Weight = {Weight}");
        var lastUnitAsTail = unitSequence.GetTail(unitSequence.Length - 1);
        return GetChildrenActions(lastUnitAsTail, matchingWeight);
      }
    }
  }
}
