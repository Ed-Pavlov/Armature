using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit is an argument for a property marked with attribute which satisfies conditions checked by the predicate
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public record IsPropertyWithAttributePattern<T> : IsInjectPointByAttributePattern<T> where T : Attribute
  {
    [DebuggerStepThrough]
    public IsPropertyWithAttributePattern(Predicate<T>? predicate) : base(predicate) { }

    protected override T? GetAttribute(UnitId unitId) => GetPropertyAttribute(unitId);

    public static T? GetPropertyAttribute(UnitId unitId) => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttribute<T>() : default;
  }
}
