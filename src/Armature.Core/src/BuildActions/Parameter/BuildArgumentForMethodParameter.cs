using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the method parameter.
  /// </summary>
  public class BuildArgumentForMethodParameter : BuildArgumentBase
  {
    public static readonly IBuildAction Instance = new BuildArgumentForMethodParameter();

    public BuildArgumentForMethodParameter(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
