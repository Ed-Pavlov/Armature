using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  ///   Matches <see cref="T:Armature.Core.UnitInfo" /> with an open generic type
  /// </summary>
  public class OpenGenericTypeMatcher : UnitInfoMatcher
  {
    [DebuggerStepThrough]
    public OpenGenericTypeMatcher(UnitInfo unitInfo) : base(unitInfo) { }

    public override bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType != null && unitType.IsGenericType && Equals(unitType.GetGenericTypeDefinition(), UnitInfo.Id);
    }
  }
}