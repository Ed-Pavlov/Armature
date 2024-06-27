using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit is a type that can be instantiated
/// </summary>
public record CanBeInstantiated : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.GetUnitTypeSafe() is {IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false};

  public override string ToString() => nameof(CanBeInstantiated);
}
