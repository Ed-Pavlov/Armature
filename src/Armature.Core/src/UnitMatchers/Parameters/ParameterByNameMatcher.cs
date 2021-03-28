using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter by name
  /// </summary>
  public  sealed record ParameterByNameMatcher : InjectPointByNameMatcher
  {
    [DebuggerStepThrough]
    public ParameterByNameMatcher(string name) : base(name) { }

    protected override string? GetInjectPointName(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.Name;
  }
}