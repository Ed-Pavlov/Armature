using System;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for build stack patterns using <see cref="IUnitPattern"/> to match the passed <see cref="UnitId"/>.
/// </summary>
public abstract class BuildStackPatternByUnitBase : BuildStackPatternBase, IInternal<IUnitPattern>
{
  protected readonly IUnitPattern UnitPattern;

  protected BuildStackPatternByUnitBase(IUnitPattern unitPattern, int weight) : base(weight)
    => UnitPattern = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));

  protected override void PrintContentToLog(LogLevel logLevel)
  {
    Log.WriteLine(LogLevel.Info, $"Pattern: {UnitPattern.ToHoconString()}");
    base.PrintContentToLog(logLevel);
  }

  private bool Equals(BuildStackPatternByUnitBase? other) => base.Equals(other) && Equals(UnitPattern, other.UnitPattern);

  public override bool Equals(IBuildStackPattern? other) => Equals(other as BuildStackPatternByUnitBase);
  public override bool Equals(object?             obj)   => Equals(obj as BuildStackPatternByUnitBase);

  public override int GetHashCode()
  {
    unchecked
    {
      return (UnitPattern.GetHashCode() * 397) ^ base.GetHashCode();
    }
  }

  IUnitPattern IInternal<IUnitPattern>.Member1 => UnitPattern;

  public override string ToString() => $"{ToHoconString()}{{ Pattern: {UnitPattern} }}";
}