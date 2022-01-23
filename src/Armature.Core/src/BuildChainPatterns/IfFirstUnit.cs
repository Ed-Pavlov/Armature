using System;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if the first unit in the build chain matches the specified pattern.
/// </summary>
public class IfFirstUnit : BuildChainPatternByUnitBase
{
  public IfFirstUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildChainPattern.IfFirstUnit) { }
  public IfFirstUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

  /// <summary>
  /// Checks if the first unit in the build chain matches the specified patter.
  /// If it is the target unit, returns build actions for it, if no, pass the rest of the build chain to each child and returns all actions from children merged
  /// </summary>
  public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight)
  {
    actionBag = null;

    var hasActions = false;
    // ReSharper disable once AccessToModifiedClosure - yes, I need it to be captured
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(IfFirstUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      if(buildChain.Length == 0)
      {
        Console.WriteLine("x");
      }
      var isPatternMatches = UnitPattern.Matches(buildChain[0]);
      Log.WriteLine(LogLevel.Verbose, LogConst.Matched, isPatternMatches);

      hasActions = isPatternMatches && GetOwnOrChildrenBuildActions(buildChain.GetTail(1), inputWeight, out actionBag);
      return hasActions;
    }
  }
}