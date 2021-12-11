using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Skips units from the build chain while unit matches specified pattern till the last (under construction) unit.
/// </summary>
public class SkipWhileUnit : BuildChainPatternByUnitBase
{
  public SkipWhileUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildContextPattern.Neutral) {}
  public SkipWhileUnit(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    var i = 0;

    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipWhileUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}");

      for(; i < buildChain.Length - 1; i++)
      {
        if(!UnitPattern.Matches(buildChain[i]))
        {
          Log.WriteLine(LogLevel.Verbose, LogConst.Matched, false);
          break;
        }
      }

      return GetChildrenActions(buildChain.GetTail(i), inputWeight, out actionBag);
    }
  }
}