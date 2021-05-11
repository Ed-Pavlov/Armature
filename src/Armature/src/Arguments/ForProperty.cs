using System.Reflection;
using Armature.Core;

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
               return parentNode.GetOrAddNode(new IfLastUnit(new IsPropertyWithType(typeof(T), true), InjectPointMatchingWeight.TypedParameter));
             });

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyArgumentTuner<object?> Named(string propertyName)
      => new(parentNode =>
             {
               Property.Named(propertyName).Tune(parentNode);
               return parentNode.GetOrAddNode(new IfLastUnit(new IsPropertyNamed(propertyName), InjectPointMatchingWeight.NamedParameter));
             });

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyArgumentTuner<object?> WithInjectPoint(object? injectPointId)
      => new(parentNode =>
             {
               Property.ByInjectPoint(injectPointId).Tune(parentNode);
               return parentNode.GetOrAddNode(
                 new IfLastUnit(new IsPropertyInfoWithAttribute(injectPointId), InjectPointMatchingWeight.AttributedParameter));
             });
  }
}
