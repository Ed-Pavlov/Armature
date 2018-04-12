using System.Reflection;
using Armature.Core.UnitMatchers.Parameters;
using Resharper.Annotations;

namespace Armature
{
  public static class ForParameter
  {
    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.ParameterType" /> equals to <see cref="T" />
    /// </summary>
    public static ParameterValueTuner<T> OfType<T>()
    {
      var matcher = new ParameterByStrictTypeMatcher(typeof(T));
      return new ParameterValueTuner<T>(matcher, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.Name" /> equals to <see cref="parameterName" />
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <returns></returns>
    public static ParameterValueTuner Named([NotNull] string parameterName)
    {
      var matcher = new ParameterByNameMatcher(parameterName);
      return new ParameterValueTuner(matcher, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with parameter marked with <see cref="InjectAttribute" />(<see cref="injectPointId" />)
    /// </summary>
    /// <param name="injectPointId">
    ///   Matches parameter marked with <see cref="InjectAttribute" /> with <see cref="InjectAttribute.InjectionPointId" />
    ///   equals to <paramref name="injectPointId" />
    /// </param>
    public static ParameterValueTuner WithInjectPoint([CanBeNull] object injectPointId)
    {
      var matcher = new ParameterByInjectPointMatcher(injectPointId);
      return new ParameterValueTuner(matcher, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}