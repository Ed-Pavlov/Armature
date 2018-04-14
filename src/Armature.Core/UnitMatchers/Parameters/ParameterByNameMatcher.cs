using System.Diagnostics;
using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Parameters
{
  public class ParameterByNameMatcher : InjectPointByNameMatcher
  {
    [DebuggerStepThrough]
    public ParameterByNameMatcher([NotNull] string name) : base(name){}
    
    protected override string GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.Name;
  }
}