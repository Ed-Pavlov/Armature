﻿using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Moves along the build chain skipping units until it encounters a matching unit. Behaves like string search with wildcard.
/// </summary>
public class SkipTillUnit : BuildChainPatternByUnitBase
{
  public SkipTillUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildChainPattern.SkipTillUnit) { }
  public SkipTillUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

  /// <summary>
  /// Moves along the build chain skipping units until it finds the matching unit.
  /// If it is the target unit, returns build actions for it, if no, pass the rest of the chain to each child and returns merged actions.
  /// </summary>
  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    var hasActions = false;
    // ReSharper disable once AccessToModifiedClosure - yes, I need it to be captured
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipTillUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      for(var i = 0; i < buildChain.Length; i++)
      {
        var unitInfo = buildChain[i];

        var isPatternMatches = UnitPattern.Matches(unitInfo);
        if(isPatternMatches)
        {
          Log.WriteLine(LogLevel.Verbose, LogConst.Matched, true);
          hasActions = GetOwnOrChildrenBuildActions(buildChain.GetTail(i), inputWeight, out actionBag);
          return hasActions;
        }
      }

      Log.WriteLine(LogLevel.Trace, LogConst.Matched, false);
    }

    actionBag = null;
    return false;
  }
}