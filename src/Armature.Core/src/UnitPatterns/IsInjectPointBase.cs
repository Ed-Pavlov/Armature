using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for patterns check if a unit is an inject point marked with with <see cref="InjectAttribute" />
/// with an optional <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />
/// </summary>
public abstract record IsInjectPointBase : IUnitPattern, ILogString
{
  private readonly object? _injectPointTag;

  /// <param name="injectPointTag">An optional tag of the inject point. <see cref="InjectAttribute"/> for details.</param>
  [DebuggerStepThrough]
  protected IsInjectPointBase(object? injectPointTag = null) => _injectPointTag = injectPointTag;

  public bool Matches(UnitId unitId)
  {
    if(unitId.Tag != SpecialTag.Argument) return false;

    var attributes = GetAttributes(unitId);
    return attributes.Any(attr => Equals(attr.Tag, _injectPointTag));
  }

  protected abstract IEnumerable<InjectAttribute> GetAttributes(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ InjectPointId: {_injectPointTag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}