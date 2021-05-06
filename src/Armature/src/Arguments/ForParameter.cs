using Armature.Core;

namespace Armature
{
  /// <summary>
  /// This class provides methods to tune up how to build arguments for method parameters.
  /// </summary>
  public static class ForParameter
  {
    /// <summary>
    ///   Tunes up what argument inject into method parameter of type <typeparamref name="T" />.
    /// </summary>
    public static ArgumentStaticTuner<T> OfType<T>()
      => new(parentNode =>
               parentNode.GetOrAddNode(
                 new IfLastUnitMatches(new MethodParameterByTypePattern(typeof(T), true), InjectPointMatchingWeight.TypedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter with the specified <paramref name="parameterName"/>.
    /// </summary>
    public static ArgumentStaticTuner Named(string parameterName)
      => new(parentNode =>
               parentNode.GetOrAddNode(new IfLastUnitMatches(new ParameterWithNamePattern(parameterName), InjectPointMatchingWeight.NamedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter marked with <see cref="InjectAttribute"/> with the specified <paramref name="injectPointId"/>. 
    /// </summary>
    public static ArgumentStaticTuner WithInjectPoint(object? injectPointId)
      => new(parentNode =>
               parentNode.GetOrAddNode(
                 new IfLastUnitMatches(new ParameterWithInjectIdPattern(injectPointId), InjectPointMatchingWeight.AttributedParameter)));
  }
}
