using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if <see cref="UnitId.Kind"/> is a type which can be instantiated.
/// </summary>
public record CanBeInstantiated : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.GetUnitTypeSafe() is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};

  public override string ToString() => nameof(CanBeInstantiated);
}
