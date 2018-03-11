using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework
{
  public class InjectPointParameterMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new InjectPointParameterMatcher();
    public static IBuildAction BuildAction { get; } = new BuildActionImpl();

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

    public class BuildActionImpl : IBuildAction
    {
      public void Process(UnitBuilder unitBuilder)
      {
        var parameterInfo = (ParameterInfo)unitBuilder.GetUnitUnderConstruction().Id;

        var attribute = parameterInfo
          .GetCustomAttributes(typeof(InjectAttribute), true)
          .OfType<InjectAttribute>()
          .Single();

        var unitInfo = new UnitInfo(parameterInfo.ParameterType, attribute.InjectionPointId);
        Log.Verbose("{0}: {1}", GetType().Name, unitInfo);
        unitBuilder.BuildResult = unitBuilder.Build(unitInfo);
      }

      public void PostProcess(UnitBuilder unitBuilder) { }
    }
  }
}