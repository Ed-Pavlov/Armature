using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit is a generic type and it's <see cref="Type.GetGenericTypeDefinition"/> is as specified open generic type.
/// </summary>
public record IsGenericOfDefinition : TypePatternBase, IUnitPattern
{
  [DebuggerStepThrough]
  public IsGenericOfDefinition(Type genericTypeDefinition, object? tag) : base(genericTypeDefinition, tag)
  {
    if(!genericTypeDefinition.IsGenericTypeDefinition) throw new ArgumentException("Should be an open generic type", nameof(genericTypeDefinition));
  }

  public bool Matches(UnitId unitId)
  {
    var unitType = unitId.GetUnitTypeSafe();
    return Tag.Matches(unitId.Tag) && unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == Type;
  }
}
