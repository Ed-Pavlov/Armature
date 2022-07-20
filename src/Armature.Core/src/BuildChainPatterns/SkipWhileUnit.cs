using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Skips units from the build chain while unit matches specified pattern till the target unit. The target unit is never skipped.
/// </summary>
public class SkipWhileUnit : BuildChainPatternByUnitBase
{
  public SkipWhileUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildChainPattern.SkipWhileUnit) { }
  public SkipWhileUnit(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    var hasActions = false;

    // ReSharper disable once AccessToModifiedClosure - yes, I need it to be captured
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipWhileUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      var i = 0;
      for(; i < buildChain.Length; i++)
        if(!UnitPattern.Matches(buildChain[i]))
          break;

      hasActions = GetOwnAndChildrenBuildActions(buildChain.GetTail(i), inputWeight, out actionBag);
      return hasActions;
    }
  }
}