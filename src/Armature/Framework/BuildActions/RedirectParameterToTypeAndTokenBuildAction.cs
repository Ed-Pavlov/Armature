using System;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework.BuildActions
{
  public class RedirectParameterToTypeAndTokenBuildAction : RedirectToTypeAndTokenBuildAction
  {
    public RedirectParameterToTypeAndTokenBuildAction(object token = null) : base(token)
    {
    }

    protected override Type GetValueType(UnitInfo unitInfo) => ((ParameterInfo)unitInfo.Id).ParameterType;
  }
}