using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit is an argument for a parameter marked with attribute which satisfies conditions checked by the predicate
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record IsParameterWithAttributeMatcher<T> : IsInjectPointByAttributeMatcher<T> where T : Attribute
  {
    [DebuggerStepThrough]
    public IsParameterWithAttributeMatcher(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetAttribute(UnitId unitId) => GetParameterAttribute(unitId);

    public static T? GetParameterAttribute(UnitId unitId) => unitId.Kind is ParameterInfo parameterInfo ? parameterInfo.GetCustomAttribute<T>() : default;
  }
}
