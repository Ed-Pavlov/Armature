using System;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Tunes which constructor should be called to create an object.
/// </summary>
public static class Constructor
{
  /// <summary>
  /// Instantiate a Unit using a constructor with the biggest number of parameters.
  /// </summary>
  public static IInjectionPointSideTuner WithMaxParametersCount()
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByTypeAssignability + WeightOf.BuildStackPattern.IfFirstUnit))
                .AppendChildBuildStackPatternNodes(tuner)
                .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointId" />).
  /// </summary>
  public static IInjectionPointSideTuner MarkedWithInjectAttribute(object? injectionPointId)
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildStackPattern.IfFirstUnit))
                .AppendChildBuildStackPatternNodes(tuner)
                .UseBuildAction(new GetConstructorByInjectPoint(injectionPointId), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using constructor without parameters.
  /// </summary>
  public static IInjectionPointSideTuner Parameterless() => WithParameters();

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1>() => WithParameters(typeof(T1));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2>() => WithParameters(typeof(T1), typeof(T2));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2, T3>() => WithParameters(typeof(T1), typeof(T2), typeof(T3));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2, T3, T4>() => WithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters specified in <paramref name="parameterTypes" />.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static IInjectionPointSideTuner WithParameters(params Type[] parameterTypes)
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByName + WeightOf.BuildStackPattern.IfFirstUnit))
                .AppendChildBuildStackPatternNodes(tuner)
                .UseBuildAction(new GetConstructorByParameterTypes(parameterTypes), BuildStage.Create));
}
