using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// This class provides methods to tune up how to build arguments for method parameters.
/// </summary>
public static class ForParameter
{
  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter of type <paramref name="type"/>.
  /// </summary>
  public static MethodArgumentTuner<object?> OfType(Type type)
    => new MethodArgumentTuner<object?>(
      (tuner, weight)
        => tuner.TreeRoot.GetOrAddNode(
                   new IfFirstUnit(
                     new IsMethodParameterOfType(new UnitPattern(type)),
                     weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.IfFirstUnit))
                .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
                .AppendContextBranch(tuner));

                            // $"Building of an argument for the method parameter of type {type.ToLogString()} is already tuned");

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter of type <typeparamref name="T" />.
  /// </summary>
  public static MethodArgumentTuner<T> OfType<T>()
    => new MethodArgumentTuner<T>(
      (tuner, weight)
        => tuner.TreeRoot
          .GetOrAddNode(
             new IfFirstUnit(
               new IsMethodParameterOfType(new UnitPattern(typeof(T))),
               weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.IfFirstUnit))
          .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
          .AppendContextBranch(tuner));

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter named <paramref name="parameterName"/>.
  /// </summary>
  public static MethodArgumentTuner<object?> Named(string parameterName)
    => new MethodArgumentTuner<object?>(
      (tuner, weight)
        => tuner.TreeRoot
          .GetOrAddNode(
             new IfFirstUnit(
               new IsMethodParameterNamed(parameterName),
               weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildChainPattern.IfFirstUnit))
          .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
          .AppendContextBranch(tuner));

  /// <summary>
  /// Tunes up how to build and argument to inject into a method parameter marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static MethodArgumentTuner<object?> WithInjectPoint(object? injectPointId)
    => new MethodArgumentTuner<object?>(
      (tuner, weight)
        => tuner.TreeRoot
          .GetOrAddNode(
             new IfFirstUnit(
               new IsParameterMarkedWithAttribute(injectPointId),
               weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildChainPattern.IfFirstUnit))
          .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
          .AppendContextBranch(tuner));
}