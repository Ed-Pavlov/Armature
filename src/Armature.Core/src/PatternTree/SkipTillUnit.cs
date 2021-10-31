using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Moves along the building units sequence from the unit passed to the <see cref="BuildSession.BuildUnit(UnitId)"/> to its dependencies skipping units
  ///   until it encounters a matching unit. Behaves like string search with wildcard.
  /// </summary>
  public class SkipTillUnit : PatternTreeNodeBase
  {
    private readonly IUnitPattern _pattern;
    public SkipTillUnit(IUnitPattern pattern) : this(pattern, WeightOf.BuildingUnitSequencePattern.Neutral) { }
    public SkipTillUnit(IUnitPattern pattern, int weight) : base(weight)
      => _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));

    /// <summary>
    ///   Moves along the building unit sequence skipping units until it finds the matching unit.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      using(Log.NamedBlock(LogLevel.Verbose, nameof(SkipTillUnit)))
      {
        Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {_pattern.ToLogString()}, Weight = {Weight}");

        for(var i = 0; i < unitSequence.Length; i++)
        {
          var unitInfo = unitSequence[i];

          var isPatternMatches = _pattern.Matches(unitInfo);

          if(isPatternMatches)
          {
            Log.WriteLine(LogLevel.Verbose, LogConst.Matched, true);
            return GetOwnOrChildrenBuildActions(unitSequence.GetTail(i), inputWeight);
          }
        }
        Log.WriteLine(LogLevel.Verbose, LogConst.Matched, false);
      }
      return null;
    }

    public override void PrintToLog()
    {
      using(Log.NamedBlock(LogLevel.Info, GetType().GetShortName()))
      {
        Log.WriteLine(LogLevel.Info, $"Pattern = {_pattern.ToLogString()}, Weight = {Weight:n0}");
        PrintChildrenToLog();
        PrintBuildActionsToLog();
      }
    }

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().GetShortName()}( {_pattern.ToLogString()} ){{ Weight={Weight:n0} }}";

    #region Equality

    public override bool Equals(IPatternTreeNode? other) => Equals(other as SkipTillUnit);
    public override bool Equals(object? obj) => Equals(obj as SkipTillUnit);

    private bool Equals(SkipTillUnit? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Equals(_pattern, other._pattern) && Weight == other.Weight;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (_pattern.GetHashCode() * 397) ^ Weight;
      }
    }

    #endregion
  }
}