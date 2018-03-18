using System.Diagnostics;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework.Properties
{
  public class PropertyByInjectPointMatcher : PropertyByAttributeMatcher<InjectAttribute>
  {
    private readonly object _injectPointId;

    [DebuggerStepThrough]
    public PropertyByInjectPointMatcher(object injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId)) => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.AsLogString());

    #region Equality
    public override bool Equals(IUnitMatcher obj) => obj is PropertyByInjectPointMatcher other && Equals(_injectPointId, other._injectPointId);

    public override bool Equals(object obj) => Equals(obj as PropertyByInjectPointMatcher);

    public override int GetHashCode() => _injectPointId != null ? _injectPointId.GetHashCode() : 0;
    #endregion
  }
}