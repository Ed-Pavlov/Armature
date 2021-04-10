using System;
using System.Diagnostics;

namespace Armature.Core.UnitMatchers.UnitType
{
  /// <summary>
  ///   Matches <see cref="UnitInfo" /> with an open generic type
  /// </summary>
  public record OpenGenericTypeMatcher : UnitInfoByTypeMatcherBase, IUnitMatcher
  {
    [DebuggerStepThrough]
    public OpenGenericTypeMatcher(Type type, object? token) : base(type, token)
    {
      if(!type.IsGenericTypeDefinition) throw new ArgumentException("Provide open generic type", nameof(type));
    }

    public bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == UnitType && Token.Matches(unitInfo.Token);
    }
  }
}
