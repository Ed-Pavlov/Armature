﻿using System;
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
  public static MethodArgumentTuner OfType(Type type)
    => new MethodArgumentTuner(
      (tuningContext, weight)
        => tuningContext.TreeRoot.GetOrAddNode(
                      new IfTargetUnit(
                        new IsMethodParameterWithType(new UnitPattern(type)),
                        weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.TargetUnit))
                   .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
                       .TryAddContext(tuningContext.GetContextNode));

                            // $"Building of an argument for the method parameter of type {type.ToLogString()} is already tuned");

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter of type <typeparamref name="T" />.
  /// </summary>
  public static MethodArgumentTuner<T> OfType<T>()
    => new MethodArgumentTuner<T>(
      (tuningContext, weight)
        => tuningContext.TreeRoot
          .GetOrAddNode(
             new IfTargetUnit(
               new IsMethodParameterWithType(new UnitPattern(typeof(T))),
               weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.TargetUnit))
          .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
          .TryAddContext(tuningContext.GetContextNode));

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter named <paramref name="parameterName"/>.
  /// </summary>
  public static MethodArgumentTuner Named(string parameterName)
    => new MethodArgumentTuner(
      (tuningContext, weight)
        => tuningContext.TreeRoot
          .GetOrAddNode(
             new IfTargetUnit(
               new IsMethodParameterNamed(parameterName),
               weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildChainPattern.TargetUnit))
          .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
          .TryAddContext(tuningContext.GetContextNode));

  /// <summary>
  /// Tunes up how to build and argument to inject into a method parameter marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static MethodArgumentTuner WithInjectPoint(object? injectPointId)
    => new MethodArgumentTuner(
      (tuningContext, weight)
        => tuningContext.TreeRoot
          .GetOrAddNode(
             new IfTargetUnit(
               new IsParameterMarkedWithAttribute(injectPointId),
               weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildChainPattern.TargetUnit))
          .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
          .TryAddContext(tuningContext.GetContextNode));
}