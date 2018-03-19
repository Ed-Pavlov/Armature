using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Parameters
{
  public class ParameterByAttributeMatcher<T> : InjectPointByAttributeMatcher<T>
    where T : Attribute
  {
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public ParameterByAttributeMatcher([CanBeNull] Predicate<T> predicate) : base(predicate){}

    protected override T GetInjectPointAttribute(UnitInfo unitInfo) => GetParameterAttribute(unitInfo);

    public static T GetParameterAttribute(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is ParameterInfo parameterInfo)) return default;
      return parameterInfo.GetCustomAttribute<T>();
    }
  }
}