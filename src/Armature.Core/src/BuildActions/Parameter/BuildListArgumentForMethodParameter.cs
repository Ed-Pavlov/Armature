using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a method parameter.
  /// </summary>
  public class BuildListArgumentForMethodParameter : BuildListArgument
  {
    public static readonly BuildListArgumentForMethodParameter Instance = new();

    public BuildListArgumentForMethodParameter(object? key = null) : base(key) { }

    protected override Type GetArgumentType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
