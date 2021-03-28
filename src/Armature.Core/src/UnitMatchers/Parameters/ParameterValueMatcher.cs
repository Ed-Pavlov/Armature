using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches Unit representing "value for parameter" for the currently building Unit
  /// </summary>
  public sealed record ParameterValueMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new ParameterValueMatcher();

    private ParameterValueMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && unitInfo.Id is ParameterInfo;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}