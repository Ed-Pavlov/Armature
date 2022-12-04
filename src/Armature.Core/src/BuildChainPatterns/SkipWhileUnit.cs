using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Skips units from the build stack while unit matches specified pattern till the target unit. The target unit is never skipped.
/// </summary>
public class SkipWhileUnit : BuildStackPatternByUnitBase
{
  public SkipWhileUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildStackPattern.SkipWhileUnit) { }
  public SkipWhileUnit(IUnitPattern unitPattern, int weight) : base(unitPattern, weight) { }

  public override bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
  {
    var hasActions = false;

    // ReSharper disable once AccessToModifiedClosure - yes, I need it to be captured
    using(Log.ConditionalMode(LogLevel.Verbose, () => hasActions))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipWhileUnit)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      var i = 0;
      for(; i < stack.Length; i++)
        if(!UnitPattern.Matches(stack[i]))
          break;

      hasActions = GetOwnAndChildrenBuildActions(stack.GetTail(i), inputWeight, out actionBag);
      return hasActions;
    }
  }
}