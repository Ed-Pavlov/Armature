using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the method parameter marked with <see cref="InjectAttribute"/> using <see cref="InjectAttribute.Tag"/> as
/// the <see cref="UnitId.Tag"/>
/// </summary>
public record BuildArgumentByParameterTypeAndTag : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var parameterInfo = (ParameterInfo) buildSession.BuildChain.TargetUnit.Kind!;

    foreach(var attribute in parameterInfo.GetCustomAttributes<InjectAttribute>())
    {
      Log.WriteLine(LogLevel.Trace, () => $"Attribute: {attribute.ToHoconString()}");

      var unitInfo    = new UnitId(parameterInfo.ParameterType, attribute.Tag);
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
