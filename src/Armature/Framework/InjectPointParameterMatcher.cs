using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Framework.BuildActions;
using Armature.Interface;

namespace Armature.Framework
{
  public class InjectPointParameterMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new InjectPointParameterMatcher();
    public static IBuildAction BuildAction { get; } = new RedirectParameterInfoToTypeAndTokenBuildAction();

    private InjectPointParameterMatcher() { }

    public bool Matches(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is ParameterInfo parameterInfo) || unitInfo.Token != SpecialToken.ParameterValue)
        return false;

      var attribute = parameterInfo
        .GetCustomAttributes(typeof(InjectAttribute), true)
        .OfType<InjectAttribute>()
        .SingleOrDefault();
      return attribute != null;
    }

    public bool Equals(IUnitMatcher matcher) => matcher is InjectPointParameterMatcher;
  }
}