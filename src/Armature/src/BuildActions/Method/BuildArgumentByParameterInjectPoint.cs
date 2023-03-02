using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the constructor/method parameter which is marked with <see cref="InjectAttribute"/> using <see cref="ParameterInfo.Name"/> and
/// <see cref="InjectAttribute.Tag"/> as /// as <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByParameterTypeAndTag : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var parameterInfo = (ParameterInfo) buildSession.Stack.TargetUnit.Kind!;

    foreach(var attribute in parameterInfo.GetCustomAttributes<InjectAttribute>())
    {
      if(Log.IsEnabled(LogLevel.Trace))
        Log.WriteLine(LogLevel.Trace, $"Attribute: {attribute.ToHoconString()}");

      var unitInfo    = Unit.Of(parameterInfo.ParameterType, attribute.Tag);
      var buildResult = buildSession.BuildUnit(unitInfo);

      if(buildResult.HasValue)
      {
        buildSession.BuildResult = buildResult;
        break;
      }
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  public override string ToString() => nameof(BuildArgumentByParameterTypeAndTag);
}
