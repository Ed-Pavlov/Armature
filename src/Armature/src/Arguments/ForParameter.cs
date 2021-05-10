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
    public static MethodArgumentTuner<T> OfType<T>()
      => new(parentNode =>
               parentNode.GetOrAddNode(
                 new IfLastUnit(new IsMethodParameterWithType(typeof(T), true), InjectPointMatchingWeight.TypedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter with the specified <paramref name="parameterName"/>.
    /// </summary>
    public static MethodArgumentTuner<object?> Named(string parameterName)
      => new(parentNode => parentNode.GetOrAddNode(new IfLastUnit(new IsMethodParameterNamed(parameterName), InjectPointMatchingWeight.NamedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter marked with <see cref="InjectAttribute"/> with the specified <paramref name="injectPointId"/>. 
    /// </summary>
    public static MethodArgumentTuner<object?> WithInjectPoint(object? injectPointId)
      => new(parentNode =>
               parentNode.GetOrAddNode(
                 new IfLastUnit(new IsParameterInfoWithAttribute(injectPointId), InjectPointMatchingWeight.AttributedParameter)));
  }
}
