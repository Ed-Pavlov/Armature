namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is a constructor needed to build some other unit.
  /// </summary>
  public record ConstructorPattern : SimpleToStringImpl, IUnitPattern
  {
    public static readonly IUnitPattern Instance = new ConstructorPattern();

    private ConstructorPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Constructor && unitId.GetUnitTypeSafe() is not null;
  }
}
