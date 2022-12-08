using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// This class provides methods to tune up how to build arguments to inject to object properties.
/// </summary>
public static class ForProperty
{
  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type <paramref name="type"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> OfType(Type type)
    => new PropertyArgumentTuner<object?>(
      (tuner, weight) =>
      {
        Property.OfType(type).ApplyTo(tuner);

        return tuner.GetInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyOfType(new UnitPattern(type)),
                         weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type<typeparamref name="T" />
  /// </summary>
  public static PropertyArgumentTuner<T> OfType<T>()
    => new PropertyArgumentTuner<T>(
      (tuner, weight) =>
      {
        Property.OfType<T>().ApplyTo(tuner);

        return tuner.GetInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyOfType(new UnitPattern(typeof(T))),
                         weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property named <paramref name="propertyName"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> Named(string propertyName)
    => new PropertyArgumentTuner<object?>(
      (tuner, weight) =>
      {
        Property.Named(propertyName).ApplyTo(tuner);

        return tuner.GetInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyNamed(propertyName),
                         weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });

  /// <summary>
  /// Tunes up how to build and argument to inject into a property marked with <see cref="InjectAttribute"/>
  /// with the optional <paramref name="injectPointTag"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> WithInjectPoint(object? injectPointTag)
    => new PropertyArgumentTuner<object?>(
      (tuner, weight) =>
      {
        Property.ByInjectPointTag(injectPointTag).ApplyTo(tuner);

        return tuner.GetInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyAttributed(injectPointTag),
                         weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });
}
