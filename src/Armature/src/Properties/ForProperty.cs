using System.Reflection;
using Armature.Core;

namespace Armature
{
  public static class ForProperty
  {
    /// <summary>
    ///   Matches with property with <see cref="PropertyInfo.PropertyType" /> equals to <typeparamref name="T" />
    /// </summary>
    public static PropertyValueTuner<T> OfType<T>()
    {
      var getPropertyAction = new GetPropertyByTypeBuildAction(typeof(T));
      var matcher           = new PropertyByTypePattern(typeof(T), true);

      return new PropertyValueTuner<T>(matcher, getPropertyAction, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyValueTuner Named(string propertyName)
    {
      var getPropertyAction = new GetPropertyByNameBuildAction(propertyName);
      var matcher           = new PropertyWithNamePattern(propertyName);

      return new PropertyValueTuner(matcher, getPropertyAction, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyValueTuner WithInjectPoint(object? injectPointId)
    {
      var getPropertyAction = new GetPropertyListByInjectPointId(injectPointId);
      var matcher           = new PropertyWithInjectIdPattern(injectPointId);

      return new PropertyValueTuner(matcher, getPropertyAction, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}
