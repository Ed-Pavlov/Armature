using System;
using Armature.Framework;

namespace Armature.Core
{
  /// <inheritdoc />
  /// <summary>
  /// Matches a unit representing parameter
  /// </summary>
  public class ParameterMatcher : IUnitMatcher
  {
    public static readonly ParameterMatcher Instance = new ParameterMatcher();

    private ParameterMatcher()
    {}

    public bool Matches(UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      return unitInfo.Token == SpecialToken.ParameterValue;
    }

    public bool Equals(IUnitMatcher other)
    {
      return other is ParameterMatcher;
    }

    public override string ToString()
    {
      return typeof(ParameterMatcher).Name;
    }
  }
}