﻿using System;
using Armature.Core;
using Armature.Core.Logging;

namespace Armature
{
  /// <summary>
  /// This class provides methods to tune up how to build arguments for method parameters.
  /// </summary>
  public static class ForParameter
  {
    /// <summary>
    ///   Tunes up how to build an argument to inject into method parameter of type <paramref name="type"/>.
    /// </summary>
    public static MethodArgumentTuner OfType(Type type)
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipWhileUnit(Static<IsServiceUnit>.Instance, 0))
                .AddNode(new IfFirstUnit(new IsMethodParameterWithType(new Pattern(type)), WeightOf.BuildingUnitSequencePattern.IfFirstUnit + WeightOf.InjectionPoint.ByExactType),
                         $"Building of an argument for the method parameter of type {type.ToLogString()} is already tuned"));

    /// <summary>
    ///   Tunes up what argument inject into method parameter of type <typeparamref name="T" />.
    /// </summary>
    public static MethodArgumentTuner<T> OfType<T>()
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipWhileUnit(Static<IsServiceUnit>.Instance, 0))
                .AddNode(new IfFirstUnit(new IsMethodParameterWithType(new Pattern(typeof(T))), WeightOf.BuildingUnitSequencePattern.IfFirstUnit + WeightOf.InjectionPoint.ByExactType),
                         $"Building of an argument for the method parameter of type {typeof(T).ToLogString()} is already tuned"));

    /// <summary>
    ///   Tunes up what argument inject into method parameter with the specified <paramref name="parameterName"/>.
    /// </summary>
    public static MethodArgumentTuner Named(string parameterName)
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipWhileUnit(Static<IsServiceUnit>.Instance, 0))
                .AddNode(new IfFirstUnit(new IsMethodParameterNamed(parameterName), WeightOf.BuildingUnitSequencePattern.IfFirstUnit + WeightOf.InjectionPoint.ByName),
                         $"Building of an argument for the method parameter with name {parameterName} is already tuned"));

    /// <summary>
    ///   Tunes up what argument inject into method parameter marked with <see cref="InjectAttribute"/> with the specified <paramref name="injectPointId"/>.
    /// </summary>
    public static MethodArgumentTuner WithInjectPoint(object? injectPointId)
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipWhileUnit(Static<IsServiceUnit>.Instance, 0))
                .AddNode(new IfFirstUnit(new IsParameterMarkedWithAttribute(injectPointId), WeightOf.BuildingUnitSequencePattern.IfFirstUnit + WeightOf.InjectionPoint.ByInjectPointId),
                         $"Building of an argument for the method parameter marked with {nameof(InjectAttribute)}"
                       + $" with {nameof(InjectAttribute.InjectionPointId)} equal to {injectPointId.ToHoconString()} is already tuned"));
  }
}