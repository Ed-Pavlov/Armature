using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Builds arguments for constructor/method parameters one by one in the direct order.
/// </summary>
public record BuildMethodArgumentsInCustomOrder : IBuildAction
{
  private readonly Func<ParameterInfo[], IEnumerable<Tuple<int, ParameterInfo>>> _reorder;

  /// <param name="reorder">A method that reorders parameters to control the build order.
  /// It should return a sequence of tuples, each containing the original index of a parameter and the parameter info.
  /// The build process will follow the order of the returned sequence.</param>
  [PublicAPI]
  public BuildMethodArgumentsInCustomOrder(Func<ParameterInfo[], IEnumerable<Tuple<int, ParameterInfo>>> reorder)
    => _reorder = reorder ?? throw new ArgumentNullException(nameof(reorder));

  public void Process(IBuildSession buildSession)
  {
    var parameters = (ParameterInfo[]) buildSession.Stack.TargetUnit.Kind!;
    var arguments  = new object?[parameters.Length];

    foreach(var (index, parameterInfo) in _reorder(parameters))
      arguments[index] = buildSession.BuildArgumentForMethod(parameterInfo);

    buildSession.BuildResult = new BuildResult(arguments);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(BuildMethodArgumentsInCustomOrder);
}