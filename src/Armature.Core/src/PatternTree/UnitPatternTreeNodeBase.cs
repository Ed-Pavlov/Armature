using System;
using Armature.Core.Logging;

namespace Armature.Core;

/// <summary>
/// Some base logic to reuse in unit building sequence patterns based on <see cref="IUnitPattern"/>
/// </summary>
public abstract class UnitPatternTreeNodeBase : PatternTreeNodeBase
{
  protected readonly IUnitPattern UnitPattern;

  protected UnitPatternTreeNodeBase(IUnitPattern unitPattern, int weight) : base(weight)
    => UnitPattern = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));

  protected override void PrintContentToLog(LogLevel logLevel)
  {
    Log.WriteLine(LogLevel.Info, $"Pattern: {UnitPattern.ToHoconString()}");
    base.PrintContentToLog(logLevel);
  }

  private bool Equals(UnitPatternTreeNodeBase? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;

    return Weight == other.Weight && GetType() == other.GetType() && Equals(UnitPattern, other.UnitPattern);
  }

  public override bool Equals(IPatternTreeNode? other) => Equals(other as UnitPatternTreeNodeBase);
  public override bool Equals(object?           obj)   => Equals(obj as UnitPatternTreeNodeBase);

  public override int GetHashCode()
  {
    unchecked
    {
      return (UnitPattern.GetHashCode() * 397) ^ Weight;
    }
  }
}