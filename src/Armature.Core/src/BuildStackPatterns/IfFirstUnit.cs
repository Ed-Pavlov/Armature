namespace Armature.Core;

/// <summary>
/// Checks if the first unit in the build stack matches the specified pattern.
/// </summary>
public class IfFirstUnit : BuildStackPatternByUnitBase
{
  public IfFirstUnit(IUnitPattern pattern) : base(pattern, WeightOf.BuildStackPattern.IfFirstUnit) { }
  public IfFirstUnit(IUnitPattern pattern, int weight) : base(pattern, weight) { }

  /// <summary>
  /// Checks if the first unit in the build stack matches the specified patter.
  /// If it is the target unit, returns build actions for it, if no, pass the rest of the build stack to each child and returns all actions from children merged
  /// </summary>
  public override bool GatherBuildActions(BuildSession.Stack stack, out WeightedBuildActionBag? actionBag, long inputWeight)
  {
    actionBag = null;

    using(var condition = Log.UnderCondition(LogLevel.Verbose))
    using(Log.NamedBlock(LogLevel.Verbose, nameof(IfFirstUnit)))
    {
      if(Log.IsEnabled(LogLevel.Verbose))
        Log.WriteLine(LogLevel.Verbose, $"Pattern = {UnitPattern.ToHoconString()}, Weight = {Weight.ToHoconString()}");

      var isPatternMatches = UnitPattern.Matches(stack[0]);
      Log.WriteLine(LogLevel.Verbose, LogConst.Matched, isPatternMatches);

      var hasActions = isPatternMatches && GetOwnAndChildrenBuildActions(stack.GetTail(1), inputWeight, out actionBag);
      condition.IsMet = hasActions;
      return hasActions;
    }
  }
}
