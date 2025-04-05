using System;
using System.Collections.Generic;
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
public class Constructor
{
  /// <summary>
  /// Represents a set of rules or criteria for selecting a constructor during dependency injection.
  /// </summary>
  /// <remarks>
  /// It's named that to IDE suggest good name for parameter of action passed to <see cref="Constructor.TryInOrder(Action{Ctor})"/>
  /// </remarks>
  public class Ctor
  {
    internal readonly List<IBuildAction> _buildActions = new();

    /// <summary>
    /// Specifies to use the constructor with the maximum number of parameters.
    /// </summary>
    public void WithMaxParametersCount() => _buildActions.Add(Static.Of<GetConstructorWithMaxParametersCount>());

    /// <summary>
    /// Specifies to use the constructor marked with the <see cref="InjectAttribute" /> and optionally filtered by the given injection point tag.
    /// </summary>
    /// <param name="injectionPointTag">The optional tag that identifies the specific injection point.</param>
    public void MarkedWithInjectAttribute(object? injectionPointTag) => _buildActions.Add(new GetConstructorByInjectPoint(injectionPointTag));

    /// <summary>
    /// Specifies to use a constructor with parameters that match the specified types.
    /// </summary>
    /// <param name="parameterTypes">The types of parameters that the constructor should match.</param>
    public void WithParameters(params Type[] parameterTypes) => _buildActions.Add(new GetConstructorByParameterTypes(parameterTypes));
  }

  /// <summary>
  /// Defines a sequence of rules to select a constructor during dependency injection.
  /// </summary>
  /// <param name="constructor">The action to configure the sequence of rules and criteria for selecting a constructor.</param>
  /// <returns>An <see cref="IInjectionPointSideTuner"/> instance to further configure dependency injection.</returns>
  public static IInjectionPointSideTuner TryInOrder(Action<Ctor> constructor)
  {
    var sugar = new Ctor();
    constructor(sugar);

    return new InjectionPointSideTuner(
      tuner
        => tuner
          .GetTunerInternals()
          .TreeRoot
          .GetOrAddNode(new IfFirstUnit(Static.Of<IsConstructor>(), Core.WeightOf.BuildStackPattern.IfFirstUnit))
          .UseBuildAction(new TryInOrder(sugar._buildActions.ToArray()), BuildStage.Create));
  }

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
