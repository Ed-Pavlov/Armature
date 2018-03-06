using System;
using Armature.Framework;

namespace Armature.Core
{
  /// <inheritdoc />
  /// <summary>
  /// Matches a unit representing type constructor
  /// </summary>
  public class ConstructorMatcher : IUnitMatcher
  {
    public static readonly ConstructorMatcher Instance = new ConstructorMatcher();

    private ConstructorMatcher()
    {}

    public bool Matches(UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      return unitInfo.Token == SpecialToken.Constructor;
    }

    public bool Equals(IUnitMatcher other)
    {
      return other is ConstructorMatcher;
    }
    
    public override string ToString()
    {
      return typeof(ConstructorMatcher).Name;
    }
  }
}