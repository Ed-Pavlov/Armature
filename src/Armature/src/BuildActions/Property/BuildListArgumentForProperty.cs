using System;
using System.Reflection;
using Armature.Core;

namespace Armature.BuildActions.Property;

/// <summary>
/// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a property.
/// </summary>
public record BuildListArgumentForProperty : BuildListArgumentBase
{
  public BuildListArgumentForProperty() { }
  public BuildListArgumentForProperty(object? tag) : base(tag) { }

  protected override Type GetArgumentType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
}
