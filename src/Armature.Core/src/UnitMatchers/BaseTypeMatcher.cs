using System;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Matches <see cref="UnitInfo" /> if it is a type inherits specified base type
  /// </summary>
  public class BaseTypeMatcher : UnitInfoByTypeMatcherBase, IUnitMatcher
  {
    public BaseTypeMatcher([NotNull]Type baseType, [CanBeNull] object token) : base(baseType, token) {} 

    public bool Matches(UnitInfo unitInfo) => UnitType.IsAssignableFrom(unitInfo.GetUnitTypeSafe()) && MatchesToken(unitInfo);
  }
}