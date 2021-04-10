using System.Diagnostics;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches <see cref="UnitInfo" /> with an open generic type
  /// </summary>
  public record OpenGenericTypeMatcher : UnitInfoMatcher
  {
    [DebuggerStepThrough]
    public OpenGenericTypeMatcher(UnitInfo unitInfo) : base(unitInfo) { }

    public override bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();

      return unitType is {IsGenericType: true} && Equals(unitType.GetGenericTypeDefinition(), UnitInfo.Id);
    }
  }
}
