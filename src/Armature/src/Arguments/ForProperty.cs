using System.Diagnostics.CodeAnalysis;
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
    {
      var buildAction = new GetPropertyByTypeBuildAction(typeof(T));
      var pattern     = new PropertyByTypePattern(typeof(T), true);

      return new PropertyArgumentTuner<T>(pattern, buildAction, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyArgumentTuner Named(string propertyName)
    {
      var getPropertyList   = new GetPropertyListByNameBuildAction(propertyName);
      var isPropertyPattern = new PropertyWithNamePattern(propertyName);

      return new PropertyArgumentTuner(isPropertyPattern, getPropertyList, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyArgumentTuner WithInjectPoint(object? injectPointId)
    {
      var getPropertyAction = new GetPropertyListByInjectPointId(injectPointId);
      var matcher           = new PropertyWithInjectIdPattern(injectPointId);

      return new PropertyArgumentTuner(matcher, getPropertyAction, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}
