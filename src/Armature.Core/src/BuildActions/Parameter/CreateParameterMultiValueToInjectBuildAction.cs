using System;
using System.Reflection;

namespace Armature.Core.BuildActions.Parameter
{
  public class CreateParameterMultiValueToInjectBuildAction : CreateMultiValueToInjectBuildAction
  {
    public static readonly CreateParameterMultiValueToInjectBuildAction Instance = new CreateParameterMultiValueToInjectBuildAction();
    
    public CreateParameterMultiValueToInjectBuildAction(object? token = null) : base(token)
    {
    }

    protected override Type GetValueType(UnitInfo unitInfo) => ((ParameterInfo)unitInfo.Id!).ParameterType;
  }
}