using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Parameters
{
  public class ParameterByNameMatcher : InjectPointByNameMatcher, IUnitMatcher
  {
    [DebuggerStepThrough]
    public ParameterByNameMatcher([NotNull] string name) : base(name){}
    
    protected override string GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.Name;
  }
}