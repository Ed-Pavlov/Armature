using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for an object property marked with attribute which satisfies conditions checked by the optional predicate.
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record PropertyWithAttributePattern<T> : InjectPointByAttributePattern<T> where T : Attribute
  {
    [DebuggerStepThrough]
    public PropertyWithAttributePattern(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetAttribute(UnitId unitId) => GetPropertyAttribute(unitId);

    public static T? GetPropertyAttribute(UnitId unitId) => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttribute<T>() : default;
  }
}
