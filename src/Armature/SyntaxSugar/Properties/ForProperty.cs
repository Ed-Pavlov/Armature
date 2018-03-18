using System.Reflection;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.Properties;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public static class ForProperty
  {
    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.ParameterType" /> equals to <see cref="T" />
    /// </summary>
    public static PropertyValueSugar<T> OfType<T>()
    {
      var matcher = new PropertyByStrictTypeMatcher(typeof(T));
      var getPropertyAction = new GetPropertyByTypeBuildAction(typeof(T));
      return new PropertyValueSugar<T>(matcher, getPropertyAction, ParameterMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.Name" /> equals to <see cref="parameterName" />
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <returns></returns>
    public static PropertyValueSugar Named([NotNull] string parameterName)
    {
      var matcher = new PropertyByNameMatcher(parameterName);
      var getPropertyAction = new GetPropertyByNameBuildAction(parameterName);
      return new PropertyValueSugar(matcher, getPropertyAction, ParameterMatchingWeight.NamedParameter);
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
      var matcher = new PropertyByInjectPointMatcher(injectPointId);
      var getPropertyAction = new GetPropertyByInjectPointBuildAction(injectPointId);
      return new PropertyValueSugar(matcher, getPropertyAction, ParameterMatchingWeight.AttributedParameter);
    }
  }
}