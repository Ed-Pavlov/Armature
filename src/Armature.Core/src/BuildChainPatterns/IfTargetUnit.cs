using Armature.Core.Sdk;

namespace Armature.Core;

public class IfTargetUnit : BuildChainPatternByUnitBase
{
  public IfTargetUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    actionBag = null;

    var hasActions = false;
    // ReSharper disable once AccessToModifiedClosure - yes, I need it to be captured
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(IfTargetUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      var isPatternMatches = UnitPattern.Matches(buildChain.TargetUnit);
      Log.WriteLine(LogLevel.Verbose, LogConst.Matched, isPatternMatches);

      hasActions = isPatternMatches && GetOwnOrChildrenBuildActions(buildChain.GetTail(1), inputWeight, out actionBag);
      return hasActions;
    }
  }
}