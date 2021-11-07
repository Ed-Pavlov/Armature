using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for patterns check if a unit is an inject point marked with with <see cref="InjectAttribute" />
  /// with an optional <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public abstract record InjectPointAttributePatternBase : IUnitPattern, ILogString
  {
    private readonly object? _injectPointId;

    /// <param name="injectPointId">An optional id of the inject point. <see cref="InjectAttribute"/> for details.</param>
    [DebuggerStepThrough]
    protected InjectPointAttributePatternBase(object? injectPointId = null) => _injectPointId = injectPointId;

    public bool Matches(UnitId unitId)
    {
      if(unitId.Key != SpecialKey.Argument) return false;

      var attribute = GetAttribute(unitId);
      return attribute is not null && Equals(attribute.InjectionPointId, _injectPointId);
    }

    protected abstract InjectAttribute? GetAttribute(UnitId unitId);

    [DebuggerStepThrough]
    public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ InjectPointId: {_injectPointId.ToHoconString()} }} }}";
    [DebuggerStepThrough]
    public sealed override string ToString() => ToHoconString();
  }
}