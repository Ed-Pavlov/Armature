using System.Diagnostics;
using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  /// Matches parameter by name
  /// </summary>
  public class ParameterByNameMatcher : InjectPointByNameMatcher
  {
    [DebuggerStepThrough]
    public ParameterByNameMatcher([NotNull] string name) : base(name){}
    
    protected override string GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.Name;
  }
}