using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Skips units from the building unit sequence while unit matches specified pattern till the last (under construction) unit.
  /// </summary>
  public class SkipWhileUnit : UnitPatternTreeNodeBase
  {
    public SkipWhileUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildingUnitSequencePattern.Neutral) {}
    public SkipWhileUnit(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      var i = 0;

      using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipWhileUnit)))
      {
        Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}");

        for(; i < unitSequence.Length - 1; i++)
        {
          if(!UnitPattern.Matches(unitSequence[i]))
          {
            Log.WriteLine(LogLevel.Verbose, LogConst.Matched, false);
            break;
          }
        }
        return GetChildrenActions(unitSequence.GetTail(i), inputWeight + Weight);
      }
    }
  }
}