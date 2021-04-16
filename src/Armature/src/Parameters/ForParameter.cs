using System.Reflection;
using Armature.Core;

namespace Armature
{
  public static class ForParameter
  {
    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.ParameterType" /> equals to <typeparamref name="T" />
    /// </summary>
    public static ParameterValueTuner<T> OfType<T>()
    {
      var matcher = new IsParameterOfTypeMatcher(typeof(T), true);
      return new ParameterValueTuner<T>(matcher, InjectPointMatchingWeight.TypedParameter);
    }

    /// <summary>
    ///   Matches with parameter with <see cref="ParameterInfo.Name" /> equals to <paramref name="parameterName" />
    /// </summary>
    /// <param name="parameterName">Matches parameter with this name</param>
    /// <returns></returns>
    public static ParameterValueTuner Named(string parameterName)
    {
      var matcher = new UnitIsParameterWithNameMatcher(parameterName);

      return new ParameterValueTuner(matcher, InjectPointMatchingWeight.NamedParameter);
    }

    /// <summary>
    ///   Matches with parameter marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    /// <param name="injectPointId">
    ///   Matches parameter marked with <see cref="InjectAttribute" /> with <see cref="InjectAttribute.InjectionPointId" />
    ///   equals to <paramref name="injectPointId" />
    /// </param>
    public static ParameterValueTuner WithInjectPoint(object? injectPointId)
    {
      var matcher = new IsParameterWithInjectIdMatcher(injectPointId);

      return new ParameterValueTuner(matcher, InjectPointMatchingWeight.AttributedParameter);
    }
  }
}
