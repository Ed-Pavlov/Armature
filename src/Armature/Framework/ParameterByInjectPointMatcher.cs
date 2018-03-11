using System.Diagnostics;
using Armature.Interface;

namespace Armature.Framework
{
  public class ParameterByInjectPointMatcher : ParameterByAttributeMatcher<InjectAttribute>
  {
    [DebuggerStepThrough]
    public ParameterByInjectPointMatcher(object injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId)) { }
  }
}