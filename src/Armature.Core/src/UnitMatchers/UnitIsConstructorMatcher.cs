namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches Unit representing "constructor" of the currently building Unit
  /// </summary>
  public record UnitIsConstructorMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new UnitIsConstructorMatcher();

    private UnitIsConstructorMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Constructor && unitId.GetUnitTypeSafe() is not null;
  }
}
