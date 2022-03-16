using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the method parameter marked with <see cref="InjectAttribute"/> using <see cref="InjectAttribute.InjectionPointId"/> as
/// the <see cref="UnitId.Tag"/>
/// </summary>
public record BuildArgumentByParameterInjectPointId : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var parameterInfo = (ParameterInfo) buildSession.BuildChain.TargetUnit.Kind!;

    var attribute = parameterInfo
                   .GetCustomAttributes<InjectAttribute>()
                   .SingleOrDefault();

    Log.WriteLine(LogLevel.Trace, () => $"Attribute: {attribute.ToHoconString()}");

    if(attribute is not null)
    {
      var unitInfo = new UnitId(parameterInfo.ParameterType, attribute.InjectionPointId);
      buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  public override string ToString() => nameof(BuildArgumentByParameterInjectPointId);
}