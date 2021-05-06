using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the constructor/method parameter using <see cref="ParameterInfo.ParameterType"/> and specified key as <see cref="UnitId"/>.
  /// </summary>
  public class BuildArgumentByParameter : BuildArgumentBase
  {
    public static readonly IBuildAction Instance = new BuildArgumentByParameter();

    public BuildArgumentByParameter(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
