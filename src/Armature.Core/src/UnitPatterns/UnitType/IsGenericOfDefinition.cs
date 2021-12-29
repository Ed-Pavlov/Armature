using System;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if a building unit is a generic type and it's <see cref="Type.GetGenericTypeDefinition"/> is as specified open generic type
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
