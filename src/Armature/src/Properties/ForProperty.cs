using System.Reflection;
using Armature.Core;
using Armature.Core.BuildActions.Property;

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
      var matcher           = new IsPropertyOfTypeMatcher(typeof(T), true);

      return new PropertyValueTuner<T>(matcher, getPropertyAction, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyValueTuner Named(string propertyName)
    {
      var getPropertyAction = new GetPropertyByNameBuildAction(propertyName);
      var matcher           = new IsPropertyWithNameMatcher(propertyName);

      return new PropertyValueTuner(matcher, getPropertyAction, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyValueTuner WithInjectPoint(object? injectPointId)
    {
      var getPropertyAction = new GetPropertyByInjectPointBuildAction(injectPointId);
      var matcher           = new IsPropertyWithInjectIdMatcher(injectPointId);

      return new PropertyValueTuner(matcher, getPropertyAction, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}
