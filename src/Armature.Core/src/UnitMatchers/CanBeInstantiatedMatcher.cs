namespace Armature.Core
{
  /// <summary>
  /// Matches that unit is of type which can be instantiated
  /// </summary>
  public record CanBeInstantiatedMatcher : SimpleToStringImpl, IUnitIdMatcher
  {
    public static readonly IUnitIdMatcher Instance = new CanBeInstantiatedMatcher();

    private CanBeInstantiatedMatcher() { }

    public bool Matches(UnitId unitId)
    {
      var type = unitId.GetUnitTypeSafe();
      return !unitId.Key.IsSpecial() && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }
  }
}
