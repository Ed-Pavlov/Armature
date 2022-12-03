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
    var parameters = (ParameterInfo[]) buildSession.BuildChain.TargetUnit.Kind!;
    var arguments  = new object?[parameters.Length];

    for(var i = 0; i < parameters.Length; i++)
      arguments[i] = buildSession.BuildArgumentForMethod(parameters[i]);

    buildSession.BuildResult = new BuildResult(arguments);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildMethodArgumentsInDirectOrder);
}