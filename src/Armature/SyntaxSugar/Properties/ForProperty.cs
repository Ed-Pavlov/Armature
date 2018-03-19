using System.Reflection;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.BuildActions.Property;
using Armature.Framework.UnitMatchers.Properties;
using Armature.Interface;
using Armature.Properties;

namespace Armature
{
  public static class ForProperty
  {
    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.ParameterType" /> equals to <see cref="T" />
    /// </summary>
    public static PropertyValueSugar<T> OfType<T>()
    {
      var getPropertyAction = new GetPropertyByTypeBuildAction(typeof(T));
      var matcher = new PropertyByStrictTypeMatcher(typeof(T));
      return new PropertyValueSugar<T>(matcher, getPropertyAction, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.Name" /> equals to <see cref="parameterName" />
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <returns></returns>
    public static PropertyValueSugar Named([NotNull] string parameterName)
    {
      var getPropertyAction = new GetPropertyByNameBuildAction(parameterName);
      var matcher = new PropertyByNameMatcher(parameterName);
      return new PropertyValueSugar(matcher, getPropertyAction, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with parameter marked with <see cref="InjectAttribute" />(<see cref="injectPointId" />)
    /// </summary>
    /// <param name="injectPointId">
    ///   Matches parameter marked with <see cref="InjectAttribute" /> with <see cref="InjectAttribute.InjectionPointId" />
    ///   equals to <paramref name="injectPointId" />
    /// </param>
    public static PropertyValueSugar WithInjectPoint([CanBeNull] object injectPointId)
    {
      var getPropertyAction = new GetPropertyByInjectPointBuildAction(injectPointId);
      var matcher = new PropertyByInjectPointMatcher(injectPointId);
      return new PropertyValueSugar(matcher, getPropertyAction, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}