using System.Reflection;
using Armature.Framework;
using Armature.Framework.Parameters;
using Armature.Interface;
using JetBrains.Annotations;

namespace Armature
{
  public static class ForParameter
  {
    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.ParameterType" /> equals to <see cref="T" />
    /// </summary>
    public static ParameterValueSugar<T> OfType<T>()
    {
      var matcher = new ParameterByStrictTypeMatcher(typeof(T));
      return new ParameterValueSugar<T>(matcher, ParameterMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.Name" /> equals to <see cref="parameterName" />
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <returns></returns>
    public static ParameterValueSugar Named([NotNull] string parameterName)
    {
      var matcher = new ParameterByNameMatcher(parameterName);
      return new ParameterValueSugar(matcher, ParameterMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with parameter marked with <see cref="InjectAttribute" />(<see cref="injectPointId" />)
    /// </summary>
    /// <param name="injectPointId">
    ///   Matches parameter marked with <see cref="InjectAttribute" /> with <see cref="InjectAttribute.InjectionPointId" />
    ///   equals to <paramref name="injectPointId" />
    /// </param>
    public static ParameterValueSugar WithInjectPoint([CanBeNull] object injectPointId)
    {
      var matcher = new ParameterByInjectPointMatcher(injectPointId);
      return new ParameterValueSugar(matcher, ParameterMatchingWeight.AttributedParameter);
    }
  }
}