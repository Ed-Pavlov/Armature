using System.Reflection;
using Armature.Core;
using Armature.Core.Logging;

namespace Armature
{
  public static class ForProperty
  {
    /// <summary>
    ///   Matches with property with <see cref="PropertyInfo.PropertyType" /> equals to <typeparamref name="T" />
    /// </summary>
    public static PropertyArgumentTuner<T> OfType<T>()
      => new(parentNode =>
             {
               Property.OfType<T>().Tune(parentNode);

               return parentNode.AddNode(
                 new IfFirstUnit(new IsPropertyWithType(typeof(T), true), InjectPointMatchingWeight.TypedParameter),
                 $"Building of an argument for the property with type {typeof(T).ToLogString()} is already tuned");
             });

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyArgumentTuner<object?> Named(string propertyName)
      => new(parentNode =>
             {
               Property.Named(propertyName).Tune(parentNode);

               return parentNode.AddNode(
                 new IfFirstUnit(new IsPropertyNamed(propertyName), InjectPointMatchingWeight.NamedParameter),
                 $"Building of an argument for the property with name {propertyName} is already tuned");
             });

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyArgumentTuner<object?> WithInjectPoint(object? injectPointId)
      => new(parentNode =>
             {
               Property.ByInjectPoint(injectPointId).Tune(parentNode);

               return parentNode
                .AddNode(
                   new IfFirstUnit(new IsPropertyInfoWithAttribute(injectPointId), InjectPointMatchingWeight.AttributedParameter),
                   $"Building of an argument for the property marked with {nameof(InjectAttribute)}"
                 + $" with {nameof(InjectAttribute.InjectionPointId)} equal to {injectPointId.ToLogString()} is already tuned");
             });
  }
}
