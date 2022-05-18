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
  public static PropertyArgumentTuner OfType(Type type)
    => new PropertyArgumentTuner(
      (tuningContext, weight) =>
      {
        Property.OfType(type).Tune(tuningContext);

        return tuningContext.TreeRoot
                            .GetOrAddNode(
                               new IfFirstUnit(
                                 new IsPropertyWithType(new UnitPattern(type)),
                                 weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.TargetUnit))
                            .TryAddContext(tuningContext.GetContextNode);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type<typeparamref name="T" />
  /// </summary>
  public static PropertyArgumentTuner<T> OfType<T>()
    => new PropertyArgumentTuner<T>(
      (tuningContext, weight) =>
      {
        Property.OfType<T>().Tune(tuningContext);

        return tuningContext.TreeRoot
                            .GetOrAddNode(
                               new IfFirstUnit(
                                 new IsPropertyWithType(new UnitPattern(typeof(T))),
                                 weight + WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.TargetUnit))
                            .TryAddContext(tuningContext.GetContextNode);
      });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property named <see cref="MemberInfo.Name" />
  /// </summary>
  public static PropertyArgumentTuner Named(string propertyName)
    => new PropertyArgumentTuner(
      (tuningContext, weight) =>
      {
        Property.Named(propertyName).Tune(tuningContext);

        return tuningContext.TreeRoot
                            .GetOrAddNode(
                               new IfFirstUnit(
                                 new IsPropertyNamed(propertyName),
                                 weight + WeightOf.InjectionPoint.ByName + WeightOf.BuildChainPattern.TargetUnit))
                            .TryAddContext(tuningContext.GetContextNode);
      });

  /// <summary>
  /// Tunes up how to build and argument to inject into a property marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static PropertyArgumentTuner WithInjectPoint(object? injectPointId)
    => new PropertyArgumentTuner(
      (tuningContext, weight) =>
      {
        Property.ByInjectPoint(injectPointId).Tune(tuningContext);

        return tuningContext.TreeRoot
                            .GetOrAddNode(
                               new IfFirstUnit(
                                 new IsPropertyMarkedWithAttribute(injectPointId),
                                 weight + WeightOf.InjectionPoint.ByInjectPointId + WeightOf.BuildChainPattern.TargetUnit))
                            .TryAddContext(tuningContext.GetContextNode);
      });
}