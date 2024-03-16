using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using WeightOf = Armature.Sdk.WeightOf;

namespace Armature;

/// <summary>
/// Provides methods to tune up how to build arguments for method parameters.
/// </summary>
public static class ForParameter
{
  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter of type <paramref name="type"/>.
  /// </summary>
  public static MethodArgumentTuner<object?> OfType(Type type)
    => new(
      (tuner, weight)
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     new IsParameterOfType(new UnitPattern(type)),
                     weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildStackPattern.IfFirstUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>()))
                .ApplyTuner(tuner));

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter of type <typeparamref name="T" />.
  /// </summary>
  public static MethodArgumentTuner<T> OfType<T>()
    => new(
      (tuner, weight)
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     new IsParameterOfType(new UnitPattern(typeof(T))),
                     weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildStackPattern.IfFirstUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>()))
                .ApplyTuner(tuner));

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter named <paramref name="parameterName"/>.
  /// </summary>
  public static MethodArgumentTuner<object?> Named(string parameterName)
    => new(
      (tuner, weight)
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     new IsParameterNamed(parameterName),
                     weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildStackPattern.IfFirstUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>()))
                .ApplyTuner(tuner));

  /// <summary>
  /// Tunes up how to build and argument to inject into a method parameter marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static MethodArgumentTuner<object?> WithInjectPoint(object? injectPointId)
    => new(
      (tuner, weight)
        => tuner.GetInternals()
                .TreeRoot
                .GetOrAddNode(
                   new IfFirstUnit(
                     new IsParameterAttributed(injectPointId),
                     weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildStackPattern.IfFirstUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>()))
                .ApplyTuner(tuner));
}
