using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for an object property marked with <see cref="InjectAttribute"/> attribute
  /// with an optional <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public record IsPropertyInfoWithAttribute : InjectPointAttributePattern
  {
    /// <inheritdoc />
    [DebuggerStepThrough]
    public IsPropertyInfoWithAttribute(object? injectPointId = null) : base(injectPointId) { }

    protected override InjectAttribute? GetAttribute(UnitId unitId)
      => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttribute<InjectAttribute>() : default;
  }
}
