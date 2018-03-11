using System;
using Armature.Framework;

namespace Armature.Core
{
  /// <summary>
  ///   Matches a unit representing type constructor
  /// </summary>
  public class ConstructorMatcher : IUnitMatcher
  {
    public static readonly ConstructorMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher() { }

    public bool Matches(UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      return unitInfo.Token == SpecialToken.Constructor;
    }

    public bool Equals(IUnitMatcher other) => other is ConstructorMatcher;

    public override string ToString() => typeof(ConstructorMatcher).Name;
  }
}