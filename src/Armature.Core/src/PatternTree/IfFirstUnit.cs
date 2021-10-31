using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if the first unit in the building unit sequence matches the specified pattern.
  /// </summary>
  public class IfFirstUnit : PatternTreeNodeBase
  {
    private readonly IUnitPattern _pattern;

    public IfFirstUnit(IUnitPattern pattern, int weight = 0) : base(weight)
      => _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));

    /// <summary>
    ///   Checks if the first unit in the building unit sequence matches the specified patter.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      using(Log.NamedBlock(LogLevel.Verbose, nameof(IfFirstUnit)))
      {
        Log.WriteLine(LogLevel.Verbose, () => $"Pattern = {_pattern.ToLogString()}, Weight = {Weight}");

        var matches = _pattern.Matches(unitSequence[0]);
        Log.WriteLine(LogLevel.Verbose, LogConst.Matched, matches);
        return matches ? GetOwnOrChildrenBuildActions(unitSequence, inputWeight) : null;
      }
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

    public override bool Equals(IPatternTreeNode? other) => Equals(other as IfFirstUnit);
    public override bool Equals(object?           obj)   => Equals(obj as IfFirstUnit);

    private bool Equals(IfFirstUnit? other)
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