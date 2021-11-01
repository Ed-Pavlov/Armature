namespace Armature.Core
{
  /// <summary>
  /// Checks if <see cref="UnitId.Kind"/> is a type which can be instantiated.
  /// </summary>
  public record CanBeInstantiated : IUnitPattern
  {
    public bool Matches(UnitId unitId)
    {
      var type = unitId.GetUnitTypeSafe();
      return unitId.Key is not SpecialKey && type is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};
    }

    public override string ToString() => nameof(CanBeInstantiated);
  }
}