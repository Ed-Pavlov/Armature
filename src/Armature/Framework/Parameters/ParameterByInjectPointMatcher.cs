using System.Diagnostics;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework.Parameters
{
  public class ParameterByInjectPointMatcher : ParameterByAttributeMatcher<InjectAttribute>
  {
    private readonly object _injectPointId;

    [DebuggerStepThrough]
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId)) => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.AsLogString());

    #region Equality
    public override bool Equals(IUnitMatcher obj) => obj is ParameterByInjectPointMatcher other && Equals(_injectPointId, other._injectPointId);

    public override bool Equals(object obj) => Equals(obj as ParameterByInjectPointMatcher);

    public override int GetHashCode() => (_injectPointId != null ? _injectPointId.GetHashCode() : 0);
    #endregion
  }
}