using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a property.
  /// </summary>
  public class BuildListArgumentForProperty : BuildListArgumentBase
  {
    public BuildListArgumentForProperty(object? key = null) : base(key) { }

    protected override Type GetArgumentType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
