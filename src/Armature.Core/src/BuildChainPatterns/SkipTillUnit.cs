using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Moves along the build chain skipping units until it encounters a matching unit. Behaves like string search with wildcard.
/// </summary>
public class SkipTillUnit : BuildChainPatternByUnitBase
{
  public SkipTillUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildContextPattern.Neutral) { }
  public SkipTillUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

  /// <summary>
  /// Moves along the build chain skipping units until it finds the matching unit.
  /// If it is the unit under construction, returns build actions for it, if no, pass the rest of the chain to each child and returns merged actions.
  /// </summary>
  public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> buildChain, int inputWeight)
  {
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipTillUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight}");

      for(var i = 0; i < buildChain.Length; i++)
      {
        var unitInfo = buildChain[i];

        var isPatternMatches = UnitPattern.Matches(unitInfo);

        if(isPatternMatches)
        {
          Log.WriteLine(LogLevel.Verbose, LogConst.Matched, true);
          return GetOwnOrChildrenBuildActions(buildChain.GetTail(i), inputWeight);
        }
      }
      Log.WriteLine(LogLevel.Verbose, LogConst.Matched, false);
    }
    return null;
  }
}