namespace Armature.Core
{
  /// <summary>
  ///   Matches that a building unit is a list of properties of a type specified in <see cref="UnitId.Kind"/>
  /// </summary>
  public record IsPropertyMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new IsPropertyMatcher();

    private IsPropertyMatcher() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Property && unitId.GetUnitTypeSafe() is not null;
  }
}
