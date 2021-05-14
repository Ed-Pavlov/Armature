﻿using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a property.
  /// </summary>
  public record BuildListArgumentForProperty : BuildListArgumentBase
  {
    public BuildListArgumentForProperty() { }
    public BuildListArgumentForProperty(object? key) : base(key) { }

    protected override Type GetArgumentType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
    
    public override string ToString() => base.ToString();
  }
}
