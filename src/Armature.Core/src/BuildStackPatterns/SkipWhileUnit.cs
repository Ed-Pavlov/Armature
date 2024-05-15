namespace BeatyBit.Armature.Core;

/// <summary>
/// Skips units from the build stack while unit matches specified pattern till the target unit. The target unit is never skipped.
/// </summary>
public class SkipWhileUnit : BuildStackPatternByUnitBase
{
  public SkipWhileUnit(IUnitPattern unitPattern, int weight = 0) : base(unitPattern, weight) { }

  public override bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
  {
    using(var condition = Log.UnderCondition(LogLevel.Verbose))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipWhileUnit)))
    {
      if(Log.IsEnabled(LogLevel.Verbose))
        Log.WriteLine(LogLevel.Verbose, $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      var i = 0;
      for(; i < stack.Count; i++)
        if(!UnitPattern.Matches(stack[i]))
          break;

      var weight     = inputWeight + i * WeightOf.BuildStackPattern.SkipWhileUnit;
      var hasActions = GetOwnAndChildrenBuildActions(stack.GetTail(i), weight, out actionBag);
      condition.IsMet = hasActions;
      return hasActions;
    }
  }
}
