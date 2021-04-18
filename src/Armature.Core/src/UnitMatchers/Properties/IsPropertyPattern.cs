namespace Armature.Core
{
  /// <summary>
  ///   Matches that a building unit is a list of properties of a type specified in <see cref="UnitId.Kind"/>
  /// </summary>
  public record IsPropertyPattern : SimpleToStringImpl, IUnitIdPattern
  {
    public static readonly IUnitIdPattern Instance = new IsPropertyPattern();

    private IsPropertyPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Property && unitId.GetUnitTypeSafe() is not null;
  }
}
