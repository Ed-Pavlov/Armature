using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature.UnitPatterns.UnitType;

/// <summary>
/// Checks if a unit is an inheritor of the specified type.
/// </summary>
public record IsInheritorOf : TypePatternBase, IUnitPattern, IInternal<bool>
{
  [PublicAPI]
  protected readonly bool _isInterface;

  public IsInheritorOf(Type type, object? tag) : base(type, tag)
  {
    if(type.IsGenericTypeDefinition) throw new ArgumentException("Type should not be open generic", nameof(type));
    _isInterface = type.IsInterface;
  }

  public bool Matches(UnitId unitId)
    => Tag.Matches(unitId.Tag)
    && unitId.GetUnitTypeSafe() is { } unitType
    && (_isInterface
          ? Type.IsAssignableFrom(unitType)
          : unitType.IsSubclassOf(Type));

  public bool Member1 => _isInterface;
}
