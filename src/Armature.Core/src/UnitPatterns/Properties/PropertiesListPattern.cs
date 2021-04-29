namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is the list of properties of a type to inject dependencies.
  /// </summary>
  public record PropertiesListPattern : SimpleToStringImpl, IUnitPattern
  {
    public static readonly IUnitPattern Instance = new PropertiesListPattern();

    private PropertiesListPattern() { }

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.PropertiesList && unitId.GetUnitTypeSafe() is not null;
  }
}
