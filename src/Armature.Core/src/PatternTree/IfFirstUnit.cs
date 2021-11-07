using Armature.Core.Sdk;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if the first unit in the building unit sequence matches the specified pattern.
  /// </summary>
  public class IfFirstUnit : UnitPatternTreeNodeBase
  {
    public IfFirstUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildingUnitSequencePattern.IfFirstUnit) { }
    public IfFirstUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

    /// <summary>
    ///   Checks if the first unit in the building unit sequence matches the specified patter.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      using(Log.NamedBlock(LogLevel.Verbose, nameof(IfFirstUnit)))
      {
        Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight}");

        var matches = UnitPattern.Matches(unitSequence[0]);
        Log.WriteLine(LogLevel.Verbose, LogConst.Matched, matches);
        return matches ? GetOwnOrChildrenBuildActions(unitSequence, inputWeight) : null;
      }
    }
  }
}