using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a method parameter.
  /// </summary>
  public record BuildListArgumentForMethodParameter : BuildListArgumentBase
  {
    public BuildListArgumentForMethodParameter() { }
    public BuildListArgumentForMethodParameter(object? key) : base(key) { }

    protected override Type GetArgumentType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
    
    public override string ToString() => base.ToString();
  }
}
