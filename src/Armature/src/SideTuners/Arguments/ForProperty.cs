using System;
using System.Reflection;
using Armature.Core;
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
        Property.OfType(type).Tune(tuner);

        return tuner.TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyOfType(new UnitPattern(type)),
                         weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.IfFirstUnit))
                    .AppendContextBranch(tuner);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type<typeparamref name="T" />
  /// </summary>
  public static PropertyArgumentTuner<T> OfType<T>()
    => new PropertyArgumentTuner<T>(
      (tuner, weight) =>
      {
        Property.OfType<T>().Tune(tuner);

        return tuner.TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyOfType(new UnitPattern(typeof(T))),
                         weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.IfFirstUnit))
                    .AppendContextBranch(tuner);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property named <see cref="MemberInfo.Name" />
  /// </summary>
  public static PropertyArgumentTuner<object?> Named(string propertyName)
    => new PropertyArgumentTuner<object?>(
      (tuner, weight) =>
      {
        Property.Named(propertyName).Tune(tuner);

        return tuner.TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyNamed(propertyName),
                         weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildChainPattern.IfFirstUnit))
                    .AppendContextBranch(tuner);
      });

  /// <summary>
  /// Tunes up how to build and argument to inject into a property marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static PropertyArgumentTuner<object?> WithInjectPoint(object? injectPointId)
    => new PropertyArgumentTuner<object?>(
      (tuner, weight) =>
      {
        Property.ByInjectPointId(injectPointId).Tune(tuner);

        return tuner.TreeRoot
                    .GetOrAddNode(
                       new IfFirstUnit(
                         new IsPropertyMarkedWithAttribute(injectPointId),
                         weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildChainPattern.IfFirstUnit))
                    .AppendContextBranch(tuner);
      });
}
