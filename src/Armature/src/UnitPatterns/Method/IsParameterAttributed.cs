using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;

namespace Armature.UnitPatterns.Method;

/// <summary>
/// Checks if a unit is an argument for a constructor/method parameter marked with <see cref="InjectAttribute"/> attribute
/// with an optional <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />
/// </summary>
public record IsParameterAttributed : IsInjectPointBase
{
  /// <inheritdoc />
  [DebuggerStepThrough]
  public IsParameterAttributed(object? injectPointTag = null) : base(injectPointTag) { }

  protected override IEnumerable<InjectAttribute> GetAttributes(UnitId unitId)
    => unitId.Kind is ParameterInfo parameterInfo ? parameterInfo.GetCustomAttributes<InjectAttribute>() : Enumerable.Empty<InjectAttribute>();
}