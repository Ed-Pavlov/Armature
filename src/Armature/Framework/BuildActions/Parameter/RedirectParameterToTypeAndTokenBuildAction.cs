using System;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework.BuildActions.Parameter
{
  public class RedirectParameterToTypeAndTokenBuildAction : RedirectToTypeAndTokenBuildAction
  {
    public RedirectParameterToTypeAndTokenBuildAction(object token = null) : base(token)
    {
    }

    protected override Type GetValueType(UnitInfo unitInfo) => ((ParameterInfo)unitInfo.Id).ParameterType;
  }
}