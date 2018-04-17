using System.Diagnostics;
using Resharper.Annotations;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  public abstract class InjectPointByIdMatcher : InjectPointByAttributeMatcher<InjectAttribute>
  {
    [CanBeNull]
    private readonly object _injectPointId;

    [DebuggerStepThrough]
    protected InjectPointByIdMatcher(object injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId)) => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.AsLogString());

    #region Equality
    public override bool Equals(IUnitMatcher obj) => obj is InjectPointByIdMatcher other && GetType() == obj.GetType() && Equals(_injectPointId, other._injectPointId);

    public override int GetHashCode() => _injectPointId != null ? _injectPointId.GetHashCode() : 0;
    #endregion
  }
}