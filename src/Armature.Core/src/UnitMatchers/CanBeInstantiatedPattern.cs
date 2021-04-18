namespace Armature.Core
{
  /// <summary>
  /// Matches that unit is of type which can be instantiated
  /// </summary>
  public record CanBeInstantiatedPattern : SimpleToStringImpl, IUnitIdPattern
  {
    public static readonly IUnitIdPattern Instance = new CanBeInstantiatedPattern();

    private CanBeInstantiatedPattern() { }

    public bool Matches(UnitId unitId)
    {
      var type = unitId.GetUnitTypeSafe();
      return !unitId.Key.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }
  }
}
