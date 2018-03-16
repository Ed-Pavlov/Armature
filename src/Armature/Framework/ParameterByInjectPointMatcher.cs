using System.Diagnostics;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework
{
  public class ParameterByInjectPointMatcher : ParameterByAttributeMatcher<InjectAttribute>
  {
    private readonly object _injectPointId;

    [DebuggerStepThrough]
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId)) => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.AsLogString());
  }
}