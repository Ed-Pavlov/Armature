namespace Armature.Core
{
  /// <summary>
  ///   Matches Unit representing "property" of the currently building Unit
  /// </summary>
  public record UnitIsPropertyMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new UnitIsPropertyMatcher();

    private UnitIsPropertyMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Property && unitId.GetUnitTypeSafe() is not null;
  }
}
