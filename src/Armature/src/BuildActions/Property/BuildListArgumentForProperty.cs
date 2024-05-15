using System;
using System.Reflection;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a property.
/// </summary>
public record BuildListArgumentForProperty : BuildListArgumentBase
{
  public BuildListArgumentForProperty() { }
  public BuildListArgumentForProperty(object? tag) : base(tag) { }

  protected override Type GetArgumentType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
}
