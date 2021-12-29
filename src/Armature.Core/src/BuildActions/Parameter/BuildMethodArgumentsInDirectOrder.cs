using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Builds arguments for method parameters by building a unit {<see cref="ParameterInfo"/>, <see cref="SpecialTag.Argument"/> one by one.
/// </summary>
public record BuildMethodArgumentsInDirectOrder : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    Log.WriteLine(LogLevel.Verbose, "");

    var parameters = (ParameterInfo[]) buildSession.BuildChain.TargetUnit.Kind!;
    var arguments  = new object?[parameters.Length];

    for(var i = 0; i < parameters.Length; i++)
    {
      var buildResult = buildSession.BuildUnit(new UnitId(parameters[i], SpecialTag.Argument));

      if(!buildResult.HasValue)
      {
        var method = parameters[i].Member;

        throw new ArmatureException($"Argument for parameter '{parameters[i]}' of {method.DeclaringType?.ToLogString()}.{method} is not built")
         .AddData("Method", method);
      }

      arguments[i] = buildResult.Value;
    }

    buildSession.BuildResult = new BuildResult(arguments);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildMethodArgumentsInDirectOrder);
}
