using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;
using WeightOf = Armature.Sdk.WeightOf;

namespace Armature;

/// <summary>
/// Tunes which constructor should be called to create an instance.
/// </summary>
[PublicAPI]
public static class Constructor
{
  /// <summary>
  /// Instantiate a Unit using a constructor with the largest number of parameters.
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
                .ApplyTuner(tuner)
                .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointTag" />).
  /// </summary>
  public static IInjectionPointSideTuner MarkedWithInjectAttribute(object? injectionPointTag)
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildStackPattern.IfFirstUnit))
                .ApplyTuner(tuner)
                .UseBuildAction(new GetConstructorByInjectPoint(injectionPointTag), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using constructor without parameters.
  /// </summary>
  public static IInjectionPointSideTuner Parameterless() => WithParameters();

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1>() => WithParameters(typeof(T1));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2>() => WithParameters(typeof(T1), typeof(T2));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2, T3>() => WithParameters(typeof(T1), typeof(T2), typeof(T3));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2, T3, T4>() => WithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

  /// <summary>
  /// Instantiate a Unit using constructor with exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters(params Type[] parameterTypes)
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByName + WeightOf.BuildStackPattern.IfFirstUnit))
                .ApplyTuner(tuner)
                .UseBuildAction(new GetConstructorByParameterTypes(parameterTypes), BuildStage.Create));
}
