using System;
using Armature.Core;
using Armature.Core.Sdk;

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
    => new(
      (parentNode, weight) =>
        parentNode
         .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
         .AddNode(
            new IfFirstUnit(
              new IsMethodParameterWithType(new UnitPattern(type)),
              weight + WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByExactType),
            $"Building of an argument for the method parameter of type {type.ToLogString()} is already tuned"));

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter of type <typeparamref name="T" />.
  /// </summary>
  public static MethodArgumentTuner<T> OfType<T>()
    => new(
      (parentNode, weight) =>
        parentNode
         .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
         .AddNode(
            new IfFirstUnit(
              new IsMethodParameterWithType(new UnitPattern(typeof(T))),
              weight + WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByExactType),
            $"Building of an argument for the method parameter of type {typeof(T).ToLogString()} is already tuned"));

  /// <summary>
  /// Tunes up how to build an argument to inject into method parameter named <paramref name="parameterName"/>.
  /// </summary>
  public static MethodArgumentTuner Named(string parameterName)
    => new(
      (parentNode, weight) =>
        parentNode
         .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
         .AddNode(
            new IfFirstUnit(
              new IsMethodParameterNamed(parameterName),
              weight + WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByName),
            $"Building of an argument for the method parameter with name {parameterName} is already tuned"));

  /// <summary>
  /// Tunes up how to build and argument to inject into a method parameter marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static MethodArgumentTuner WithInjectPoint(object? injectPointId)
    => new(
      (parentNode, weight) =>
        parentNode
         .GetOrAddNode(new SkipWhileUnit(Static.Of<IsServiceUnit>(), 0))
         .AddNode(
            new IfFirstUnit(
              new IsParameterMarkedWithAttribute(injectPointId),
              weight + WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByInjectPointId),
            $"Building of an argument for the method parameter marked with {nameof(InjectAttribute)}"
          + $" with {nameof(InjectAttribute.InjectionPointId)} equal to {injectPointId.ToHoconString()} is already tuned"));
}
