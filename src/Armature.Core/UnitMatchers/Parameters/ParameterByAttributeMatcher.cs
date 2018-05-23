using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter marked with attribute which satisfies user provided conditions
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class ParameterByAttributeMatcher<T> : InjectPointByAttributeMatcher<T>
    where T : Attribute
  {
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public ParameterByAttributeMatcher([CanBeNull] Predicate<T> predicate) : base(predicate) { }

    protected override T GetInjectPointAttribute(UnitInfo unitInfo) => GetParameterAttribute(unitInfo);

    public static T GetParameterAttribute(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is ParameterInfo parameterInfo)) return default(T);

      return parameterInfo.GetCustomAttribute<T>();
    }
  }
}