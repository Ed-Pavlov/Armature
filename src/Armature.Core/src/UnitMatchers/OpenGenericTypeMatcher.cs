using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  /// Matches <see cref="UnitInfo" /> with an open generic type
  /// </summary>
  /// <remarks>
  /// Inheriting UnitInfoMatcher is not very good thing, because _genericType is not an _id in all this machinery,
  /// but it allows reusing MatchesToken, Equals, and ToString implementations
  /// </remarks>
  public class OpenGenericTypeMatcher : UnitInfoMatcher
  {
    private readonly Type _genericType;

    [DebuggerStepThrough]
    public OpenGenericTypeMatcher([NotNull] Type genericType, object token) : base(genericType, token) => 
      _genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));

    public override bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == _genericType && MatchesToken(unitInfo);
    }
  }
}