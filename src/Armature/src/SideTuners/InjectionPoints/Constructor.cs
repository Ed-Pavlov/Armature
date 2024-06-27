using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

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
        => tuner.GetTunerInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByTypeAssignability + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                .ApplyTuner(tuner)
                .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointTag" />).
  /// </summary>
  public static IInjectionPointSideTuner MarkedWithInjectAttribute(object? injectionPointTag)
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetTunerInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByInjectPointId + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                .ApplyTuner(tuner)
                .UseBuildAction(new GetConstructorByInjectPoint(injectionPointTag), BuildStage.Create));

  /// <summary>
  /// Instantiate a Unit using constructor without parameters.
  /// </summary>
  public static IInjectionPointSideTuner Parameterless() => WithParameters();

  /// <summary>
  /// Instantiate a Unit using constructor with an exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1>() => WithParameters(typeof(T1));

  /// <summary>
  /// Instantiate a Unit using constructor with an exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2>() => WithParameters(typeof(T1), typeof(T2));

  /// <summary>
  /// Instantiate a Unit using constructor with an exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2, T3>() => WithParameters(typeof(T1), typeof(T2), typeof(T3));

  /// <summary>
  /// Instantiate a Unit using constructor with an exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters<T1, T2, T3, T4>() => WithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

  /// <summary>
  /// Instantiate a Unit using constructor with an exact set of parameters which match specified types.
  /// </summary>
  public static IInjectionPointSideTuner WithParameters(params Type[] parameterTypes)
    => new InjectionPointSideTuner(
      tuner
        => tuner.GetTunerInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     Static.Of<IsConstructor>(),
                     WeightOf.InjectionPoint.ByName + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                .ApplyTuner(tuner)
                .UseBuildAction(new GetConstructorByParameterTypes(parameterTypes), BuildStage.Create));
}
