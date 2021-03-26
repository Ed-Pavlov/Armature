using System;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Matches <see cref="UnitInfo" /> if it is a type inherits specified base type
  /// </summary>
  /// <remarks>
  /// Inheriting UnitInfoMatcher is not very good thing, because _genericType is not an _id in all this machinery,
  /// but it allows reusing MatchesToken, Equals, and ToString implementations
  /// </remarks>
  public class BaseTypeMatcher : UnitInfoMatcher
  {
    private readonly Type _type;
    
    public BaseTypeMatcher([NotNull]Type baseType, [CanBeNull] object token) : base(baseType, token) => 
      _type = baseType ?? throw new ArgumentNullException(nameof(baseType));

    public override bool Matches(UnitInfo unitInfo) => _type.IsAssignableFrom(unitInfo.GetUnitTypeSafe()) && MatchesToken(unitInfo);
  }
}