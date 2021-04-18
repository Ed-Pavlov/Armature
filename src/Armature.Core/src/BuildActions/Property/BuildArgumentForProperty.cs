using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the property.
  /// </summary>
  public class BuildArgumentForProperty : BuildArgumentBase
  {
    [DebuggerStepThrough]
    public BuildArgumentForProperty(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
