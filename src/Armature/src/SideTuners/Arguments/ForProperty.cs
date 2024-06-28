using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

/// <summary>
/// This class provides methods to tune up how to build arguments to inject to object properties.
/// </summary>
public static class ForProperty
{
  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type <paramref name="type"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> OfType(Type type)
    => new(
      (tuner, weight) =>
      {
        Property.OfType(type).ApplyTo(tuner);

        return tuner.GetTunerInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyOfType(new UnitPattern(type)),
                         weight + WeightOf.InjectionPoint.ByExactType + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type<typeparamref name="T" />
  /// </summary>
  public static PropertyArgumentTuner<T> OfType<T>()
    => new(
      (tuner, weight) =>
      {
        Property.OfType<T>().ApplyTo(tuner);

        return tuner.GetTunerInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyOfType(new UnitPattern(typeof(T))),
                         weight + WeightOf.InjectionPoint.ByExactType + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property named <paramref name="propertyName"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> Named(string propertyName)
    => new(
      (tuner, weight) =>
      {
        Property.Named(propertyName).ApplyTo(tuner);

        return tuner.GetTunerInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyNamed(propertyName),
                         weight + WeightOf.InjectionPoint.ByName + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });

  /// <summary>
  /// Tunes up how to build and argument to inject into a property marked with <see cref="InjectAttribute"/>
  /// with the optional <paramref name="injectPointTag"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> WithInjectPoint(object? injectPointTag)
    => new(
      (tuner, weight) =>
      {
        Property.ByInjectPointTag(injectPointTag).ApplyTo(tuner);

        return tuner.GetTunerInternals()
                    .TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyAttributed(injectPointTag),
                         weight + WeightOf.InjectionPoint.ByInjectPointId + Core.WeightOf.BuildStackPattern.IfFirstUnit))
                    .ApplyTuner(tuner);
      });
}
