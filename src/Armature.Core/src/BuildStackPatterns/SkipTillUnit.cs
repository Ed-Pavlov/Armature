using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Moves along the build stack skipping units until it encounters a matching unit. Behaves like string search with wildcard.
/// </summary>
public class SkipTillUnit : BuildStackPatternByUnitBase
{
  public SkipTillUnit(IUnitPattern pattern, int weight = 0) : base(pattern, weight) { }

  /// <summary>
  /// Moves along the build stack skipping units until it finds the matching unit.
  /// If it is the target unit, returns build actions for it, if no, pass the rest of the stack to each child and returns merged actions.
  /// </summary>
  public override bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
  {
    var hasActions = false;
    // ReSharper disable once AccessToModifiedClosure - yes, I need it to be captured
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipTillUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      for(var i = 0; i < stack.Length; i++)
      {
        var unitInfo = stack[i];

        var isPatternMatches = UnitPattern.Matches(unitInfo);
        if(isPatternMatches)
        {
          Log.WriteLine(LogLevel.Verbose, LogConst.Matched, true);
          var weight = inputWeight + i * WeightOf.BuildStackPattern.SkipTillUnit;
          hasActions = GetOwnAndChildrenBuildActions(stack.GetTail(i + 1), weight, out actionBag);
          return hasActions;
        }
      }

      Log.WriteLine(LogLevel.Trace, LogConst.Matched, false);
    }

    actionBag = null;
    return false;
  }
}