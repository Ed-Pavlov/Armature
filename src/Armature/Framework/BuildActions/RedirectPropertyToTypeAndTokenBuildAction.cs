using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework.BuildActions
{
  public class RedirectPropertyToTypeAndTokenBuildAction : RedirectToTypeAndTokenBuildAction
  {
    [DebuggerStepThrough]
    public RedirectPropertyToTypeAndTokenBuildAction(object token = null) : base(token){}

    protected override Type GetValueType(UnitInfo unitInfo) => ((PropertyInfo)unitInfo.Id).PropertyType;
  }
}