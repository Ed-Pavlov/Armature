using System.Reflection;
using Armature.Framework;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public static class For
  {
    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.ParameterType" /> equals to <see cref="T" />
    /// </summary>
    public static ParameterMatcherSugar<T> Parameter<T>()
    {
      var matcher = new ParameterByStrictTypeMatcher(typeof(T));
      return new ParameterMatcherSugar<T>(matcher, ParameterMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.Name" /> equals to <see cref="parameterName" />
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <param name="weight">Weight of such match</param>
    /// <returns></returns>
    public static ParameterMatcherSugar ParameterName([NotNull] string parameterName)
    {
      var matcher = new ParameterByNameMatcher(parameterName);
      return new ParameterMatcherSugar(matcher, ParameterMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with parameter marked with <see cref="InjectAttribute" />(<see cref="injectPointId" />)
    /// </summary>
    /// <param name="injectPointId">
    ///   Matches parameter marked with <see cref="InjectAttribute" /> with <see cref="InjectAttribute.InjectionPointId" />
    ///   equals to <paramref name="injectPointId" />
    /// </param>
    /// <param name="weight">Weight of such match</param>
    public static ParameterMatcherSugar ParameterId([CanBeNull] object injectPointId)
    {
      var matcher = new ParameterByInjectPointMatcher(injectPointId);
      return new ParameterMatcherSugar(matcher, ParameterMatchingWeight.AttributedParameter);
    }
  }
}