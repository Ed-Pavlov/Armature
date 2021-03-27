using System;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Matches if an unit represented by <see cref="UnitInfo" /> is an open generic type
  /// </summary>
  public class OpenGenericTypeMatcher : UnitInfoByTypeMatcherBase, IUnitMatcher
  {
    public OpenGenericTypeMatcher([NotNull] Type genericType, object token) : base(genericType, token) { }

    public  bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == UnitType && MatchesToken(unitInfo);
    }
  }
}