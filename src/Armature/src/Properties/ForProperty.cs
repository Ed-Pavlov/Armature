using System.Reflection;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers.Properties;


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
      var matcher           = new PropertyByStrictTypeMatcher(typeof(T));

      return new PropertyValueTuner<T>(matcher, getPropertyAction, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyValueTuner Named(string propertyName)
    {
      var getPropertyAction = new GetPropertyByNameBuildAction(propertyName);
      var matcher           = new PropertyByNameMatcher(propertyName);

      return new PropertyValueTuner(matcher, getPropertyAction, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyValueTuner WithInjectPoint(object? injectPointId)
    {
      var getPropertyAction = new GetPropertyByInjectPointBuildAction(injectPointId);
      var matcher           = new PropertyByInjectPointMatcher(injectPointId);

      return new PropertyValueTuner(matcher, getPropertyAction, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}
