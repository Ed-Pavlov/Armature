using System;
using System.Reflection;

namespace Armature.Core.BuildActions.Parameter
{
  /// <summary>
  ///   "Builds" value to inject by using <see cref="ParameterInfo.ParameterType" /> and provided token
  /// </summary>
  public class CreateParameterValueBuildAction : CreateValueToInjectBuildAction
  {
    public static readonly IBuildAction Instance = new CreateParameterValueBuildAction();

    public CreateParameterValueBuildAction(object? token = null) : base(token) { }

    protected override Type GetValueType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
