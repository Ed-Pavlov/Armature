using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the constructor/method parameter using <see cref="ParameterInfo.ParameterType"/> and specified key as <see cref="UnitId"/>.
  /// </summary>
  public class BuildArgumentByParameterType : BuildArgumentByInjectPointTypeBase
  {
    public static readonly IBuildAction Instance = new BuildArgumentByParameterType();

    public BuildArgumentByParameterType() : base(null) { }
    public BuildArgumentByParameterType(object? key) : base(key) { }

    protected override Type GetInjectPointType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
