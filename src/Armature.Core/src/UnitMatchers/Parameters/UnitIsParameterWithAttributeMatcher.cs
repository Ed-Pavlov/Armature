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
  public record UnitIsParameterWithAttributeMatcher<T> : UnitByAttributeMatcherBase<T> where T : Attribute
  {
    [DebuggerStepThrough]
    public UnitIsParameterWithAttributeMatcher(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetAttribute(UnitId unitId) => GetParameterAttribute(unitId);

    public static T? GetParameterAttribute(UnitId unitId)
      => unitId.Kind is ParameterInfo parameterInfo ? parameterInfo.GetCustomAttribute<T>() : default;
  }
}
