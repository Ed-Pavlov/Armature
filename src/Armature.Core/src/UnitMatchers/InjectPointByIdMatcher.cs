using System.Diagnostics;
using Armature.Core.Logging;


namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" marked with <see cref="InjectAttribute" /> with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public abstract class InjectPointByIdMatcher : InjectPointByAttributeMatcher<InjectAttribute>
  {
    private readonly object? _injectPointId;

    [DebuggerStepThrough]
    protected InjectPointByIdMatcher(object? injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId))
      => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.ToLogString());

#region Equality

    public override bool Equals(IUnitMatcher? obj)
      => obj is InjectPointByIdMatcher other && GetType() == obj.GetType() && Equals(_injectPointId, other._injectPointId);

    public override int GetHashCode() => _injectPointId is not null ? _injectPointId.GetHashCode() : 0;

#endregion
  }
}
