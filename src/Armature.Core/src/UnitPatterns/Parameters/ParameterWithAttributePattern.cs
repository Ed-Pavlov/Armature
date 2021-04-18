using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a method parameter marked with attribute which satisfies conditions checked by the optional predicate.
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record ParameterWithAttributePattern<T> : InjectPointByAttributePattern<T> where T : Attribute
  {
    [DebuggerStepThrough]
    public ParameterWithAttributePattern(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetAttribute(UnitId unitId) => GetParameterAttribute(unitId);

    public static T? GetParameterAttribute(UnitId unitId) => unitId.Kind is ParameterInfo parameterInfo ? parameterInfo.GetCustomAttribute<T>() : default;
  }
}
