﻿using System;
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
  public static IInjectPointTuner WithMaxParametersCount()
    => new InjectPointTuner(
      (tuningContext, weight)
        => tuningContext.TreeRoot
                        .GetOrAddNode(
                           new IfTargetUnit(
                             Static.Of<IsConstructor>(),
                             weight + WeightOf.InjectionPoint.ByTypeAssignability + WeightOf.BuildChainPattern.TargetUnit))
                        .TryAddContext(tuningContext.GetContextNode)
                        .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointId" />).
  /// </summary>
  public static IInjectPointTuner MarkedWithInjectAttribute(object? injectionPointId)
    => new InjectPointTuner(
      (tuningContext, weight)
        => tuningContext.TreeRoot
                        .GetOrAddNode(
                           new IfTargetUnit(
                             Static.Of<IsConstructor>(),
                             weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildChainPattern.TargetUnit))
                        .TryAddContext(tuningContext.GetContextNode)
                        .UseBuildAction(new GetConstructorByInjectPointId(injectionPointId), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using constructor without parameters.
  /// </summary>
  public static IInjectPointTuner Parameterless() => WithParameters();

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectPointTuner WithParameters<T1>() => WithParameters(typeof(T1));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectPointTuner WithParameters<T1, T2>() => WithParameters(typeof(T1), typeof(T2));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectPointTuner WithParameters<T1, T2, T3>() => WithParameters(typeof(T1), typeof(T2), typeof(T3));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters provided as generic arguments.
  /// </summary>
  public static IInjectPointTuner WithParameters<T1, T2, T3, T4>() => WithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters specified in <paramref name="parameterTypes" />.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static IInjectPointTuner WithParameters(params Type[] parameterTypes)
    => new InjectPointTuner(
      (tuningContext, weight)
        => tuningContext.TreeRoot
                        .GetOrAddNode(
                           new IfTargetUnit(
                             Static.Of<IsConstructor>(),
                             weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildChainPattern.TargetUnit))
                        .TryAddContext(tuningContext.GetContextNode)
                        .UseBuildAction(new GetConstructorByParameterTypes(parameterTypes), BuildStage.Create));
}