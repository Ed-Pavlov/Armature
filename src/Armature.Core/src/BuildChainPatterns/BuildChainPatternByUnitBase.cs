using System;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Some base logic to reuse in build chain patterns based on <see cref="IUnitPattern"/>
/// </summary>
public abstract class BuildChainPatternByUnitBase : BuildChainPatternBase, IInternal<IUnitPattern>
{
  protected readonly IUnitPattern UnitPattern;

  protected BuildChainPatternByUnitBase(IUnitPattern unitPattern, int weight) : base(weight)
    => UnitPattern = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));

  protected override void PrintContentToLog(LogLevel logLevel)
  {
    Log.WriteLine(LogLevel.Info, $"Pattern: {UnitPattern.ToHoconString()}");
    base.PrintContentToLog(logLevel);
  }

  private bool Equals(BuildChainPatternByUnitBase? other)
  {
    if(ReferenceEquals(null, other)) return false;
    if(ReferenceEquals(this, other)) return true;

    return Weight == other.Weight && GetType() == other.GetType() && Equals(UnitPattern, other.UnitPattern);
  }

  public override bool Equals(IBuildChainPattern? other) => Equals(other as BuildChainPatternByUnitBase);
  public override bool Equals(object? obj)
  {
    var result = Equals(obj as BuildChainPatternByUnitBase);
    return result;
  }

  public override int GetHashCode()
  {
    unchecked
    {
      return (UnitPattern.GetHashCode() * 397) ^ Weight;
    }
  }

  IUnitPattern IInternal<IUnitPattern>.Member1 => UnitPattern;
}