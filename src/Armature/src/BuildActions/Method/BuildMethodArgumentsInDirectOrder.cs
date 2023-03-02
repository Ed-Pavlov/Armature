using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Builds arguments for constructor/method parameters one by one in the direct order.
/// </summary>
public record BuildMethodArgumentsInDirectOrder : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var parameters = (ParameterInfo[]) buildSession.Stack.TargetUnit.Kind!;
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