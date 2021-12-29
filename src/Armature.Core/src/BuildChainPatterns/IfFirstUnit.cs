using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if the first unit in the build chain matches the specified pattern.
/// </summary>
public class IfFirstUnit : BuildChainPatternByUnitBase
{
  public IfFirstUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildContextPattern.IfFirstUnit) { }
  public IfFirstUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

  /// <summary>
  /// Checks if the first unit in the build chain matches the specified patter.
  /// If it is the unit under construction, returns build actions for it, if no, pass the rest of the chain to each child and returns merged actions.
  /// </summary>
  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    actionBag = null;

    using(Log.NamedBlock(LogLevel.Verbose, nameof(IfFirstUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight}");

      var matches = UnitPattern.Matches(buildChain[0]);
      Log.WriteLine(LogLevel.Verbose, LogConst.Matched, matches);
      return matches && GetOwnOrChildrenBuildActions(buildChain, inputWeight, out actionBag);
    }
  }
}
