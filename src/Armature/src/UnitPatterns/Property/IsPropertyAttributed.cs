using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature.UnitPatterns.Property;

/// <summary>
/// Checks if a unit is an argument for an object property marked with <see cref="InjectAttribute"/> attribute
/// with an optional <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />
/// </summary>
public record IsPropertyAttributed : IsInjectPointBase
{
  /// <inheritdoc />
  [DebuggerStepThrough]
  public IsPropertyAttributed(object? injectPointTag = null) : base(injectPointTag) { }

  protected override IEnumerable<InjectAttribute> GetAttributes(UnitId unitId)
    => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttributes<InjectAttribute>() : Empty<InjectAttribute>.Array;
}