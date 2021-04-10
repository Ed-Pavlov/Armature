using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter marked with attribute which satisfies user provided conditions
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record ParameterByAttributeMatcher<T> : InjectPointByAttributeMatcher<T> where T : Attribute
  {
    [DebuggerStepThrough]
    public ParameterByAttributeMatcher(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetInjectPointAttribute(UnitInfo unitInfo) => GetParameterAttribute(unitInfo);

    public static T? GetParameterAttribute(UnitInfo unitInfo)
      => unitInfo.Id is ParameterInfo parameterInfo ? parameterInfo.GetCustomAttribute<T>() : default;
  }
}
