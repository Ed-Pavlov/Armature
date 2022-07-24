using System;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Real implementation of <see cref="IDependencyTuner{T}"/> interface which can be reused by different implementations
/// </summary>
public static class DependencyTuner
{
  /// <summary>
  /// Tunes how dependencies of an unit represented by <paramref name="tuner"/> should be built
  /// </summary>
  /// <param name="tuner">Tuner of an unit building rules</param>
  /// <param name="weight">The weight of registration</param>
  /// <param name="arguments">Arguments should be object instances or implementation of <see cref="IArgumentSideTuner"/></param>
  public static T UsingArguments<T>(T tuner, int weight, params object[] arguments) where T : ITuner
  {
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));
    if(arguments.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(arguments));

    if(arguments.Any(arg => arg is null))
      throw new ArgumentNullException(
        nameof(arguments),
        $"Argument should be either {nameof(IArgumentSideTuner)} or a not null instance. "
      + $"Use {nameof(ForParameter)} or custom {nameof(IArgumentSideTuner)} to provide null as an argument for a parameter.");


    foreach(var argument in arguments)
      if(argument is IArgumentSideTuner argumentTuner)
        argumentTuner.Tune(tuner);
      else if(argument is ISideTuner)
        throw new ArgumentException($"{nameof(IArgumentSideTuner)} or argument instance is expected");
      else
      {
        tuner.TreeRoot
             .GetOrAddNode(
                new IfFirstUnit(
                  new IsAssignableFromType(argument.GetType()),
                  weight + WeightOf.InjectionPoint.ByTypeAssignability + WeightOf.BuildChainPattern.IfFirstUnit))
             .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
             .AppendContextBranch(tuner)
             .UseBuildAction(new Instance<object>(argument), BuildStage.Cache);
      }

    return tuner;
  }

  /// <summary>
  /// Tunes which members should be used as injection points dependencies of an unit represented by <paramref name="tuner"/>
  /// </summary>
  /// <param name="tuner">Tuner of an unit building rules</param>
  /// <param name="injectionPoints">See inheritors of <see cref="IInjectionPointSideTuner"/> and usages in tests for details</param>
  public static T UsingInjectionPoints<T>(T tuner, params IInjectionPointSideTuner[] injectionPoints) where T : ITuner
  {
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));
    if(injectionPoints is null) throw new ArgumentNullException(nameof(injectionPoints));
    if(injectionPoints.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(injectionPoints));

    foreach(var injectPointTuner in injectionPoints)
      injectPointTuner.Tune(tuner);

    return tuner;
  }

  /// <summary>
  /// Applies passed implementation of <see cref="ISideTuner"/> to an unit represented by <paramref name="tuner"/>
  /// </summary>
  /// <param name="tuner">Tuner of an unit building rules</param>
  /// <param name="sideTuners">See implementations of <see cref="ISideTuner"/> and their usages for details.</param>
  public static T Using<T>(T tuner, params ISideTuner[] sideTuners) where T : ITuner
  {
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));
    if(sideTuners is null) throw new ArgumentNullException(nameof(sideTuners));
    if(sideTuners.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(sideTuners));

    foreach(var sideTuner in sideTuners)
      sideTuner.Tune(tuner);

    return tuner;
  }
}