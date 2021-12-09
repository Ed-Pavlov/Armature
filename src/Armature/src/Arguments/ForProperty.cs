using System;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;

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
    => new(parentNode =>
           {
             Property.OfType(type).Tune(parentNode);

             return parentNode.AddNode(
               new IfFirstUnit(new IsPropertyWithType(new UnitPattern(type)), WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByExactType),
               $"Building of an argument for the property with type {type.ToLogString()} is already tuned");
           });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property of type<typeparamref name="T" />
  /// </summary>
  public static PropertyArgumentTuner<T> OfType<T>()
    => new(parentNode =>
           {
             Property.OfType<T>().Tune(parentNode);

             return parentNode.AddNode(
               new IfFirstUnit(new IsPropertyWithType(new UnitPattern(typeof(T))), WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByExactType),
               $"Building of an argument for the property with type {typeof(T).ToLogString()} is already tuned");
           });

  /// <summary>
  /// Tunes up how to build an argument to inject into a property named <see cref="MemberInfo.Name" />
  /// </summary>
  public static PropertyArgumentTuner Named(string propertyName)
    => new(parentNode =>
           {
             Property.Named(propertyName).Tune(parentNode);

             return parentNode.AddNode(
               new IfFirstUnit(new IsPropertyNamed(propertyName), WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByName),
               $"Building of an argument for the property with name {propertyName} is already tuned");
           });

  /// <summary>
  /// Tunes up how to build and argument to inject into a property marked with <see cref="InjectAttribute"/>
  /// with the specified <paramref name="injectPointId"/>.
  /// </summary>
  public static PropertyArgumentTuner WithInjectPoint(object? injectPointId)
    => new(parentNode =>
           {
             Property.ByInjectPoint(injectPointId).Tune(parentNode);

             return parentNode
              .AddNode(
                 new IfFirstUnit(new IsPropertyMarkedWithAttribute(injectPointId), WeightOf.BuildContextPattern.IfFirstUnit + WeightOf.InjectionPoint.ByInjectPointId),
                 $"Building of an argument for the property marked with {nameof(InjectAttribute)}"
               + $" with {nameof(InjectAttribute.InjectionPointId)} equal to {injectPointId.ToHoconString()} is already tuned");
           });
}